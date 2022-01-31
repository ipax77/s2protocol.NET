using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

