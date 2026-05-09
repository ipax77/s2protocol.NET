namespace s2protocol.NET.Models;

/// <summary>Record <c>DetailsPlayer</c> Parsed replay player infos</summary>
///
public sealed class DetailsPlayer
{
    /// <summary>Record <c>DetailsPlayer</c> constructor</summary>
    ///
    public DetailsPlayer(
        PlayerColor color,
        int control,
        int handicap,
        string hero,
        string name,
        int observe,
        string race,
        int result,
        int teamId,
        Toon toon,
        int workingSetSlotId)
    {
        Color = color;
        Control = control;
        Handicap = handicap;
        Hero = hero;
        Observe = observe;
        Race = race;
        Result = result;
        TeamId = teamId;
        Toon = toon;
        WorkingSetSlotId = workingSetSlotId;
        if (name != null && name.Contains("<sp/>", StringComparison.Ordinal))
        {
            var ents = name.Split("<sp/>");
            Name = ents[1];
            ClanName = ents[0].Length > 8 ? ents[0][4..^4] : null;
        }
        else
        {
            Name = name ?? "";
        }
    }
    /// <summary>Record <c>PlayerColor</c> Parsed replay player color infos</summary>
    ///
    public PlayerColor Color { get; init; }
    /// <summary>Player Control</summary>
    ///
    public int Control { get; init; }
    /// <summary>Player Handicap</summary>
    ///
    public int Handicap { get; init; }
    /// <summary>Player Hero</summary>
    ///
    public string Hero { get; init; }
    /// <summary>Player Name (without clanTag)</summary>
    ///
    public string Name { get; init; }
    /// <summary>Player ClanName</summary>
    ///
    public string? ClanName { get; init; }
    /// <summary>Player Observe</summary>
    ///
    public int Observe { get; init; }
    /// <summary>Player Race</summary>
    /// <comment>language specific!</comment>
    ///
    public string Race { get; init; }
    /// <summary>Player Result</summary>
    ///
    public int Result { get; init; }
    /// <summary>Player TeamId</summary>
    ///
    public int TeamId { get; init; }
    /// <summary>Player Toon</summary>
    ///
    public Toon Toon { get; init; }
    /// <summary>Player WorkingSetSlotId</summary>
    ///
    public int WorkingSetSlotId { get; init; }
}

/// <summary>Record <c>Toon</c> Parsed replay player Toon infos</summary>
///
/// <remarks>Record <c>Toon</c> constructor</remarks>
///
public sealed class Toon(int id, string programId, int realm, int region)
{

    /// <summary>Toon Id</summary>
    ///
    public int Id { get; init; } = id;
    /// <summary>Toon ProgramId</summary>
    ///
    public string ProgramId { get; init; } = programId;
    /// <summary>Toon Realm</summary>
    ///
    public int Realm { get; init; } = realm;
    /// <summary>Toon Region</summary>
    ///
    public int Region { get; init; } = region;
}

/// <summary>Record <c>PlayerColor</c> Parsed replay player color infos</summary>
///
/// <remarks>Record <c>PlayerColor</c> constructor</remarks>
///
public sealed class PlayerColor(int a, int b, int g, int r)
{
    /// <summary>Color A</summary>
    ///
    public int A { get; init; } = a;
    /// <summary>Color B</summary>
    ///
    public int B { get; init; } = b;
    /// <summary>Color G</summary>
    ///
    public int G { get; init; } = g;
    /// <summary>Color R</summary>
    ///
    public int R { get; init; } = r;
}