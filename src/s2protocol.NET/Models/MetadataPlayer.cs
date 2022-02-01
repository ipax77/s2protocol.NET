using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>MetadataPlayer</c> Parsed replay player infos</summary>
///
public sealed record MetadataPlayer
{
    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public MetadataPlayer()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }


    /// <summary>Record <c>MetadataPlayer</c> constructor</summary>
    ///
    public MetadataPlayer(double apm, string assignedRace, int playerID, string result, string selectedRace)
    {
        APM = apm;
        AssignedRace = assignedRace;
        PlayerID = playerID;
        Result = result;
        SelectedRace = selectedRace;
    }

    /// <summary>Player APM</summary>
    ///
    public double APM { get; init; }
    /// <summary>Player AssignedRace</summary>
    ///
    public string AssignedRace { get; init; }
    /// <summary>Player PlayerID</summary>
    ///
    public int PlayerID { get; init; }
    /// <summary>Player Rsult</summary>
    ///
    public string Result { get; init; }
    /// <summary>Player SelectedRace</summary>
    ///
    public string SelectedRace { get; init; }
}


