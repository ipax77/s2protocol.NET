namespace s2protocol.NET.Models;
/// <summary>Record <c>MetadataPlayer</c> Parsed replay player infos</summary>
///
/// <remarks>Record <c>MetadataPlayer</c> constructor</remarks>
///
public sealed class MetadataPlayer(double apm, string assignedRace, int playerID, string result, string selectedRace)
{

    /// <summary>Player APM</summary>
    ///
    public double APM { get; init; } = apm;
    /// <summary>Player AssignedRace</summary>
    ///
    public string AssignedRace { get; init; } = assignedRace;
    /// <summary>Player PlayerID</summary>
    ///
    public int PlayerID { get; init; } = playerID;
    /// <summary>Player Rsult</summary>
    ///
    public string Result { get; init; } = result;
    /// <summary>Player SelectedRace</summary>
    ///
    public string SelectedRace { get; init; } = selectedRace;
}


