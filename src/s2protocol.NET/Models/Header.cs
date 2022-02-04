using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Record <c>Header</c> Parsed replay header infos</summary>
///
public sealed record Header
{
    /// <summary>Record <c>Header</c> constructor</summary>
    ///
    public Header(
        int protocol,
        int elapsedGameLoops,
        bool useScaledTime,
        Version version,
        string signature,
        string rootKey,
        string compatibilityHash,
        int type,
        int flags,
        int build,
        int baseBuild)
    {
        DataBuildNum = protocol;
        ElapsedGameLoops = elapsedGameLoops;
        UseScaledTime = useScaledTime;
        Version = version;
        Signature = signature;
        NgpdRootKey = rootKey;
        CompatibilityHash = compatibilityHash;
        Type = type;
        Flags = flags;
        Build = build;
        BaseBuild = baseBuild;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Header()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>Replay DataBuildNum</summary>
    ///
    public int DataBuildNum { get; init; }
    /// <summary>Replay NgpdRootKey</summary>
    ///
    public string NgpdRootKey { get; init; }
    /// <summary>Replay ElapsedGameLoops</summary>
    ///
    public int ElapsedGameLoops { get; init; }
    /// <summary>Replay UseScaledTime</summary>
    ///
    public bool UseScaledTime { get; init; }
    /// <summary>Replay version</summary>
    ///
    public Version Version { get; init; }
    /// <summary>Replay signature</summary>
    ///
    public string Signature { get; init; }
    /// <summary>Replay CompatibilityHash</summary>
    ///
    public string CompatibilityHash { get; init; }
    /// <summary>Replay Type</summary>
    ///
    public int Type { get; init; }
    /// <summary>Replay header version flags</summary>
    ///
    public int Flags { get; init; }
    /// <summary>Replay header version Build</summary>
    ///
    public int Build { get; init; }
    /// <summary>Replay header version BaseBuild</summary>
    ///
    public int BaseBuild { get; init; }
}

