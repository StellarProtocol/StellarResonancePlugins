using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Stellar.CombatMeter;

/// <summary>
/// Reflection-free, IL2CPP-safe JSON primitives for history persistence. No <c>System.Text.Json</c>
/// (reflection-based, AOT-stripped under IL2CPP) and no <c>JsonUtility</c> (no Dictionary support) — this is a
/// hand-rolled writer (<see cref="HistoryJsonWriter"/>) plus a hand-rolled tokenizing reader
/// (<see cref="HistoryJsonReader"/>). The history format only ever stores integers (long/int), one enum int, and
/// strings, so the reader handles objects, arrays, quoted strings, and integer numbers — no floats, no bool/null
/// values in payload positions. The reader is intentionally permissive on shape (callers pull keys by name and
/// tolerate absence) and never throws on EOF — it returns a sentinel token so a malformed entry can be skipped.
/// </summary>
internal sealed class HistoryJsonWriter
{
    private readonly StringBuilder _sb = new();
    private bool _needComma;

    public HistoryJsonWriter BeginObject() { Pre(); _sb.Append('{'); _needComma = false; return this; }
    public HistoryJsonWriter EndObject()   { _sb.Append('}'); _needComma = true; return this; }
    public HistoryJsonWriter BeginArray()  { Pre(); _sb.Append('['); _needComma = false; return this; }
    public HistoryJsonWriter EndArray()    { _sb.Append(']'); _needComma = true; return this; }

    /// <summary>Write an object key. The next value call supplies the paired value.</summary>
    public HistoryJsonWriter Name(string key)
    {
        Pre();
        WriteString(key);
        _sb.Append(':');
        _needComma = false;   // value follows directly, no comma
        return this;
    }

    public HistoryJsonWriter Value(long v)    { Pre(); _sb.Append(v.ToString(CultureInfo.InvariantCulture)); _needComma = true; return this; }
    public HistoryJsonWriter Value(int v)     { Pre(); _sb.Append(v.ToString(CultureInfo.InvariantCulture)); _needComma = true; return this; }

    /// <summary>Write a string value (null serialized as an empty JSON string).</summary>
    public HistoryJsonWriter Value(string? v) { Pre(); WriteString(v ?? ""); _needComma = true; return this; }

    public HistoryJsonWriter Value(long[]? arr)
    {
        BeginArray();
        if (arr != null) foreach (var n in arr) Value(n);
        return EndArray();
    }

    public override string ToString() => _sb.ToString();

    private void Pre()
    {
        if (_needComma) _sb.Append(',');
        _needComma = false;
    }

    private void WriteString(string s)
    {
        _sb.Append('"');
        foreach (var c in s)
        {
            switch (c)
            {
                case '"':  _sb.Append("\\\""); break;
                case '\\': _sb.Append("\\\\"); break;
                case '\b': _sb.Append("\\b");  break;
                case '\f': _sb.Append("\\f");  break;
                case '\n': _sb.Append("\\n");  break;
                case '\r': _sb.Append("\\r");  break;
                case '\t': _sb.Append("\\t");  break;
                default:
                    if (c < 0x20) _sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else _sb.Append(c);
                    break;
            }
        }
        _sb.Append('"');
    }
}

/// <summary>Token kinds emitted by <see cref="HistoryJsonReader"/>.</summary>
internal enum JsonTokenKind { ObjectStart, ObjectEnd, ArrayStart, ArrayEnd, Colon, Comma, String, Number, Eof, Error }

/// <summary>
/// Single-pass hand-written tokenizer over a JSON string. Numbers are returned raw (parsed by the caller as long).
/// On malformed input it emits <see cref="JsonTokenKind.Error"/> rather than throwing, so deserialization can skip
/// the offending entry. Strings are unescaped here.
/// </summary>
internal sealed class HistoryJsonReader
{
    private readonly string _s;
    private int _i;

    public HistoryJsonReader(string s) { _s = s ?? ""; _i = 0; }

    public JsonTokenKind Kind { get; private set; }
    public string StringValue { get; private set; } = "";
    public long NumberValue { get; private set; }

    /// <summary>Advance to the next token. Returns its kind (also exposed via <see cref="Kind"/>).</summary>
    public JsonTokenKind Next()
    {
        SkipWhitespace();
        if (_i >= _s.Length) return Set(JsonTokenKind.Eof);
        var c = _s[_i];
        switch (c)
        {
            case '{': _i++; return Set(JsonTokenKind.ObjectStart);
            case '}': _i++; return Set(JsonTokenKind.ObjectEnd);
            case '[': _i++; return Set(JsonTokenKind.ArrayStart);
            case ']': _i++; return Set(JsonTokenKind.ArrayEnd);
            case ':': _i++; return Set(JsonTokenKind.Colon);
            case ',': _i++; return Set(JsonTokenKind.Comma);
            case '"': return ReadString();
            default:
                if (c == '-' || (c >= '0' && c <= '9')) return ReadNumber();
                return Set(JsonTokenKind.Error);
        }
    }

    private JsonTokenKind ReadString()
    {
        _i++;   // opening quote
        var sb = new StringBuilder();
        while (_i < _s.Length)
        {
            var c = _s[_i++];
            if (c == '"') { StringValue = sb.ToString(); return Set(JsonTokenKind.String); }
            if (c == '\\')
            {
                if (_i >= _s.Length) return Set(JsonTokenKind.Error);
                var e = _s[_i++];
                switch (e)
                {
                    case '"':  sb.Append('"');  break;
                    case '\\': sb.Append('\\'); break;
                    case '/':  sb.Append('/');  break;
                    case 'b':  sb.Append('\b'); break;
                    case 'f':  sb.Append('\f'); break;
                    case 'n':  sb.Append('\n'); break;
                    case 'r':  sb.Append('\r'); break;
                    case 't':  sb.Append('\t'); break;
                    case 'u':
                        if (_i + 4 > _s.Length) return Set(JsonTokenKind.Error);
                        if (!int.TryParse(_s.Substring(_i, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var code))
                            return Set(JsonTokenKind.Error);
                        sb.Append((char)code); _i += 4; break;
                    default: return Set(JsonTokenKind.Error);
                }
            }
            else sb.Append(c);
        }
        return Set(JsonTokenKind.Error);   // unterminated string
    }

    private JsonTokenKind ReadNumber()
    {
        var start = _i;
        if (_s[_i] == '-') _i++;
        while (_i < _s.Length && _s[_i] >= '0' && _s[_i] <= '9') _i++;
        var span = _s.Substring(start, _i - start);
        if (!long.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v))
            return Set(JsonTokenKind.Error);
        NumberValue = v;
        return Set(JsonTokenKind.Number);
    }

    private void SkipWhitespace()
    {
        while (_i < _s.Length)
        {
            var c = _s[_i];
            if (c == ' ' || c == '\t' || c == '\n' || c == '\r') _i++;
            else break;
        }
    }

    private JsonTokenKind Set(JsonTokenKind k) { Kind = k; return k; }
}
