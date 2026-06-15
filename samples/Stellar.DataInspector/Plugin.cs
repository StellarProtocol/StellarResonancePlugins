using System.Collections.Generic;
using Stellar.Abstractions.Domain;
using Stellar.Abstractions.Plugins;
using Stellar.Abstractions.Services;

namespace Stellar.DataInspector;

/// <summary>
/// Interactive game-data explorer. Paste any ID from a wire log, pick a domain and getter,
/// then hit Lookup to see the resolved data row from <see cref="IGameData"/>. Demonstrates
/// the full <see cref="IGameData"/> surface (combat, items, professions, modules, map) and
/// how a uGUI window can host an input field, dropdown, and result table.
///
/// Hotkey: F7 toggles visibility (declared via IHotkeys; user can rebind).
///
/// Persistence: window state, last selection, and recent-lookup history reset on plugin reload.
/// </summary>
public sealed partial class Plugin : IStellarPlugin
{
    public string Name => "DataInspector";

    private const int MaxRecent = 5;

    private readonly IPluginServices _services;
    private readonly IWindowControl _window;

    // Snapshot of the last result's (property name, formatted value) pairs, filled on each lookup so the
    // result field-table's element Funcs don't run reflection on every poll. Read by the field-list slots.
    private readonly List<(string Name, string Value)> _fields = new(32);

    private Domain _selectedDomain = Domain.Combat;
    private Getter _selectedGetter = Getter.Skill;
    private string _idInput = string.Empty;

    // Last result of a Lookup. Box keeps the POCO record alive even when the
    // domain switches; cleared on next Lookup.
    private object? _lastResult;
    private int   _lastResultId;
    private bool  _lastWasMiss;

    // Most-recent FIFO of distinct lookups. Front = newest.
    private readonly List<RecentEntry> _recent = new(MaxRecent + 1);

    public Plugin(IPluginServices services)
    {
        _services = services;
        _services.Log.Info("[DataInspector] plugin constructed");

        _window = _services.Windows.Register(
            new WindowRegistration(
                new WindowSpec(
                    Id:          "datainspector.main",
                    Title:       "DataInspector",
                    DefaultRect: new WindowRect(60f, 60f, 720f, 0f),
                    Category:    WindowCategory.Tools,
                    Style:       WindowPanelStyle.GlassMenu)
                { StartVisible = false, Closable = true, Draggable = true },
                BuildRoot(),
                OnClose: () => _window!.SetVisible(false)),
            new HotkeyAction(
                Id:              "datainspector.toggle",
                Description:     "Toggle DataInspector",
                SuggestedDefault: new KeyBinding(StellarKeyCode.F7)),
            _services.Hotkeys);
    }

    public void Dispose()
    {
        _window.Remove();
    }

    private bool HasResult => _services.GameData.IsAvailable && _lastResult is not null && !_lastWasMiss;

    // Snapshot the last result's public properties into _fields (name + formatted value). Called after each
    // lookup so the field-table Funcs read a cached list instead of reflecting every poll.
    private void SnapshotFields()
    {
        _fields.Clear();
        if (_lastResult is null) return;
        var props = GetCachedProperties(_lastResult.GetType());
        for (int i = 0; i < props.Length; i++)
        {
            object? v;
            try { v = props[i].GetValue(_lastResult); }
            catch { v = null; }
            _fields.Add((props[i].Name, FormatValue(v)));
        }
    }

    private void DoLookup()
    {
        if (!int.TryParse(_idInput, out var id)) return;

        var (result, wasHit) = RunLookup(_selectedDomain, _selectedGetter, id);
        _lastResult = result;
        _lastResultId = id;
        _lastWasMiss = !wasHit;
        SnapshotFields();

        AddRecent(_selectedDomain, _selectedGetter, id, wasHit);

        _services.Log.Info(
            $"[DataInspector] lookup {_selectedDomain}.{_selectedGetter} #{id} -> {(wasHit ? "hit" : "miss")}");
    }

    // Look up the (domain, getter, id) triple against the framework's
    // IGameData service. Returns the boxed POCO + a hit/miss flag.
    private (object? result, bool wasHit) RunLookup(Domain d, Getter g, int id)
    {
        if (!_services.GameData.IsAvailable) return (null, false);

        object? row = (d, g) switch
        {
            (Domain.Combat,    Getter.Skill)       => _services.GameData.Combat.GetSkill(id),
            (Domain.Combat,    Getter.Buff)        => _services.GameData.Combat.GetBuff(id),
            (Domain.Combat,    Getter.Profession)  => _services.GameData.Combat.GetProfession(id),
            (Domain.Combat,    Getter.Talent)      => _services.GameData.Combat.GetTalent(id),
            (Domain.Combat,    Getter.Attribute)   => _services.GameData.Combat.GetAttribute(id),
            (Domain.Combat,    Getter.DamageAttr)  => _services.GameData.Combat.GetDamageAttr(id),
            (Domain.Inventory, Getter.Item)        => _services.GameData.Inventory.GetItem(id),
            (Domain.Inventory, Getter.Equip)       => _services.GameData.Inventory.GetEquip(id),
            (Domain.Inventory, Getter.Weapon)      => _services.GameData.Inventory.GetWeapon(id),
            (Domain.World,     Getter.Monster)     => _services.GameData.World.GetMonster(id),
            (Domain.World,     Getter.Npc)         => _services.GameData.World.GetNpc(id),
            (Domain.World,     Getter.Scene)       => _services.GameData.World.GetScene(id),
            (Domain.World,     Getter.Map)         => _services.GameData.World.GetMap(id),
            (Domain.Progress,  Getter.Quest)       => _services.GameData.Progress.GetQuest(id),
            (Domain.Progress,  Getter.Dungeon)     => _services.GameData.Progress.GetDungeon(id),
            (Domain.Progress,  Getter.Activity)    => _services.GameData.Progress.GetActivity(id),
            (Domain.Progress,  Getter.Achievement) => _services.GameData.Progress.GetAchievement(id),
            (Domain.Progress,  Getter.Title)       => _services.GameData.Progress.GetTitle(id),
            (Domain.Progress,  Getter.Award)       => _services.GameData.Progress.GetAward(id),
            _ => null,
        };
        return (row, row is not null);
    }

    private void AddRecent(Domain d, Getter g, int id, bool wasHit)
    {
        for (int i = 0; i < _recent.Count; i++)
        {
            if (_recent[i].Domain == d && _recent[i].Getter == g && _recent[i].Id == id)
            {
                _recent.RemoveAt(i);
                break;
            }
        }
        _recent.Insert(0, new RecentEntry(d, g, id, wasHit));
        if (_recent.Count > MaxRecent) _recent.RemoveAt(_recent.Count - 1);
    }

    private void RestoreRecent(RecentEntry entry)
    {
        _selectedDomain = entry.Domain;
        _selectedGetter = entry.Getter;
        _idInput = entry.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var (result, wasHit) = RunLookup(entry.Domain, entry.Getter, entry.Id);
        _lastResult = result;
        _lastResultId = entry.Id;
        _lastWasMiss = !wasHit;
        SnapshotFields();
    }

    private enum Domain { Combat, Inventory, World, Progress }

    private enum Getter
    {
        // Combat
        Skill, Buff, Profession, Talent, Attribute, DamageAttr,
        // Inventory
        Item, Equip, Weapon,
        // World
        Monster, Npc, Scene, Map,
        // Progress
        Quest, Dungeon, Activity, Achievement, Title, Award,
    }

    private readonly record struct RecentEntry(Domain Domain, Getter Getter, int Id, bool WasHit);
}
