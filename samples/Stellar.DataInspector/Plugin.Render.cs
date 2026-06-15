using System;
using System.Collections.Generic;
using System.Reflection;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Services;

namespace Stellar.DataInspector;

/// <summary>
/// uGUI element tree (migrated off IMGUI): domain/getter selection via Active-state buttons, an ID input +
/// Lookup, a scrollable result field-table (reflected POCO properties), and a recent-lookup strip. Built once;
/// element Funcs re-pull live state on the framework's capped refresh.
/// </summary>
public sealed partial class Plugin
{
    private const float LabelColW = 130f;
    private const int MaxFields = 40;   // result rows built; extra properties beyond this aren't shown

    private static readonly string[] DomainLabels = { "Combat", "Inventory", "World", "Progress" };
    private static readonly Domain[] DomainValues = { Domain.Combat, Domain.Inventory, Domain.World, Domain.Progress };

    private static readonly Getter[] CombatGetters    = { Getter.Skill, Getter.Buff, Getter.Profession, Getter.Talent, Getter.Attribute, Getter.DamageAttr };
    private static readonly Getter[] InventoryGetters = { Getter.Item, Getter.Equip, Getter.Weapon };
    private static readonly Getter[] WorldGetters     = { Getter.Monster, Getter.Npc, Getter.Scene, Getter.Map };
    private static readonly Getter[] ProgressGetters  = { Getter.Quest, Getter.Dungeon, Getter.Activity, Getter.Achievement, Getter.Title, Getter.Award };

    private static readonly Getter[] AllGetters =
    {
        Getter.Skill, Getter.Buff, Getter.Profession, Getter.Talent, Getter.Attribute, Getter.DamageAttr,
        Getter.Item, Getter.Equip, Getter.Weapon,
        Getter.Monster, Getter.Npc, Getter.Scene, Getter.Map,
        Getter.Quest, Getter.Dungeon, Getter.Activity, Getter.Achievement, Getter.Title, Getter.Award,
    };

    private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new();

    private HudElement BuildRoot() => new ColumnElement(new HudElement[]
    {
        BuildDomainRow(),
        BuildGetterRow(),
        BuildIdRow(),
        new SeparatorElement(),
        BuildResultBlock(),
        new SeparatorElement(),
        BuildRecentRow(),
    });

    private HudElement BuildDomainRow()
    {
        var items = new List<HudElement> { new TextElement(() => "Domain:", Width: 60f) };
        for (int i = 0; i < DomainValues.Length; i++)
        {
            var d = DomainValues[i];
            var label = DomainLabels[i];
            items.Add(new ButtonElement(() => label, () => SelectDomain(d),
                Enabled: () => _services.GameData.IsAvailable, Active: () => _selectedDomain == d));
        }
        return new RowElement(items.ToArray(), Gap: 4f);
    }

    private void SelectDomain(Domain d)
    {
        _selectedDomain = d;
        _selectedGetter = GettersFor(d)[0];   // default to the new domain's first getter
    }

    // All getters in one Row; each is visible only when it belongs to the selected domain (getters are
    // domain-unique, so the Conditional shows exactly the active domain's set and collapses the rest).
    private HudElement BuildGetterRow()
    {
        var items = new List<HudElement> { new TextElement(() => "Getter:", Width: 60f) };
        foreach (var g in AllGetters)
        {
            var getter = g;
            items.Add(new ConditionalElement(() => DomainOf(getter) == _selectedDomain,
                new ButtonElement(() => getter.ToString(), () => _selectedGetter = getter,
                    Enabled: () => _services.GameData.IsAvailable, Active: () => _selectedGetter == getter)));
        }
        return new RowElement(items.ToArray(), Gap: 4f);
    }

    private HudElement BuildIdRow() => new RowElement(new HudElement[]
    {
        new TextElement(() => "ID:", Width: 60f),
        new InputElement(() => _idInput, s => { _idInput = FilterDigits(s); if (!string.IsNullOrEmpty(_idInput)) DoLookup(); },
            200f, OnChange: s => _idInput = FilterDigits(s)),
        new ButtonElement(() => "Lookup", () => { if (!string.IsNullOrEmpty(_idInput)) DoLookup(); },
            Enabled: () => _services.GameData.IsAvailable),
    }, Gap: 4f);

    // Result: the field-table (scroll of name|value rows) when a hit, else a status caption (not-ready /
    // hint / not-found warning). Rows are CellElement-aligned so the value column starts at a fixed x.
    private HudElement BuildResultBlock()
    {
        var fieldSlots = new HudElement[MaxFields];
        for (int i = 0; i < MaxFields; i++)
        {
            var idx = i;
            fieldSlots[i] = new RowElement(new HudElement[]
            {
                new CellElement(new TextElement(() => idx < _fields.Count ? _fields[idx].Name : "", Emphasis: true), Width: LabelColW),
                new TextElement(() => idx < _fields.Count ? _fields[idx].Value : ""),
            }, Gap: 8f);
        }
        var table = new ScrollElement(new ListElement(() => _fields.Count, fieldSlots), 240f);
        return new ConditionalElement(() => HasResult, table,
            new TextElement(StatusCaption, StatusColor));
    }

    private string StatusCaption()
    {
        if (!_services.GameData.IsAvailable) return "GameData service is not ready yet — Lookup is disabled until IsAvailable.";
        if (_lastWasMiss) return $"⚠ Not found: {_selectedDomain}.{_selectedGetter} #{_lastResultId}  (returned null)";
        return "Pick a domain + getter, enter an ID, press Enter or Lookup.";
    }

    private ColorRgba? StatusColor()
        => _services.GameData.IsAvailable && _lastWasMiss ? _services.Theme.Colors.Warning : (ColorRgba?)null;

    private HudElement BuildRecentRow()
    {
        var items = new List<HudElement> { new TextElement(() => "Recent:", Width: 60f) };
        for (int i = 0; i < MaxRecent; i++)
        {
            var idx = i;
            items.Add(new ConditionalElement(() => idx < _recent.Count,
                new ButtonElement(() => RecentLabel(idx), () => { if (idx < _recent.Count) RestoreRecent(_recent[idx]); })));
        }
        items.Add(new ConditionalElement(() => _recent.Count == 0,
            new TextElement(() => "(no lookups yet)", () => _services.Theme.Colors.TextMuted)));
        return new RowElement(items.ToArray(), Gap: 4f);
    }

    private string RecentLabel(int i)
    {
        if (i >= _recent.Count) return "";
        var e = _recent[i];
        return $"{e.Domain}.{e.Getter} {e.Id}";
    }

    // ===== helpers ========================================================

    private static Domain DomainOf(Getter g) => g switch
    {
        Getter.Skill or Getter.Buff or Getter.Profession or Getter.Talent or Getter.Attribute or Getter.DamageAttr => Domain.Combat,
        Getter.Item or Getter.Equip or Getter.Weapon => Domain.Inventory,
        Getter.Monster or Getter.Npc or Getter.Scene or Getter.Map => Domain.World,
        _ => Domain.Progress,
    };

    private static Getter[] GettersFor(Domain d) => d switch
    {
        Domain.Combat    => CombatGetters,
        Domain.Inventory => InventoryGetters,
        Domain.World     => WorldGetters,
        Domain.Progress  => ProgressGetters,
        _ => CombatGetters,
    };

    private static string FilterDigits(string s)
    {
        bool clean = true;
        for (int i = 0; i < s.Length; i++)
            if (s[i] < '0' || s[i] > '9') { clean = false; break; }
        if (clean) return s;

        var buf = new System.Text.StringBuilder(s.Length);
        for (int i = 0; i < s.Length; i++)
            if (s[i] >= '0' && s[i] <= '9') buf.Append(s[i]);
        return buf.ToString();
    }

    private static PropertyInfo[] GetCachedProperties(Type t)
    {
        if (!PropertyCache.TryGetValue(t, out var arr))
        {
            arr = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            PropertyCache[t] = arr;
        }
        return arr;
    }

    private static string FormatValue(object? v)
    {
        if (v is null) return "(empty)";
        if (v is string s) return s.Length == 0 ? "(empty)" : s;
        if (v is int[] ints) return FormatIntArray(ints);
        if (v is System.Collections.IEnumerable enumerable && v is not string)
        {
            var buf = new System.Text.StringBuilder();
            buf.Append('[');
            bool first = true;
            foreach (var item in enumerable)
            {
                if (!first) buf.Append(", ");
                first = false;
                buf.Append(item?.ToString() ?? "null");
            }
            buf.Append(']');
            return buf.ToString();
        }
        return v.ToString() ?? "(empty)";
    }

    private static string FormatIntArray(int[] arr)
    {
        if (arr.Length == 0) return "[]";
        var buf = new System.Text.StringBuilder(arr.Length * 8);
        buf.Append('[');
        for (int i = 0; i < arr.Length; i++)
        {
            if (i > 0) buf.Append(", ");
            buf.Append(arr[i].ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        buf.Append(']');
        return buf.ToString();
    }
}
