namespace s2protocol.NET.Models;

/// <summary>Record <c>Header</c> Parsed replay header infos</summary>
///
/// <remarks>Record <c>Header</c> constructor</remarks>
///
public sealed class Header(
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
    /// <summary>Replay DataBuildNum</summary>
    ///
    public int DataBuildNum { get; init; } = protocol;
    /// <summary>Replay NgpdRootKey</summary>
    ///
    public string NgpdRootKey { get; init; } = rootKey;
    /// <summary>Replay ElapsedGameLoops</summary>
    ///
    public int ElapsedGameLoops { get; init; } = elapsedGameLoops;
    /// <summary>Replay UseScaledTime</summary>
    ///
    public bool UseScaledTime { get; init; } = useScaledTime;
    /// <summary>Replay version</summary>
    ///
    public Version Version { get; init; } = version;
    /// <summary>Replay signature</summary>
    ///
    public string Signature { get; init; } = signature;
    /// <summary>Replay CompatibilityHash</summary>
    ///
    public string CompatibilityHash { get; init; } = compatibilityHash;
    /// <summary>Replay Type</summary>
    ///
    public int Type { get; init; } = type;
    /// <summary>Replay header version flags</summary>
    ///
    public int Flags { get; init; } = flags;
    /// <summary>Replay header version Build</summary>
    ///
    public int Build { get; init; } = build;
    /// <summary>Replay header version BaseBuild</summary>
    ///
    public int BaseBuild { get; init; } = baseBuild;
}

