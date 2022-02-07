using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;
/// <summary>Record <c>AttributeEvents</c> Parsed replay AttributeEvents</summary>
///
public sealed record AttributeEvents
{
    /// <summary>Record <c>AttributeEvents</c> constructor</summary>
    ///
    public AttributeEvents(int source,
                           int mapNameSpace,
                           ICollection<AttributeEventScope> scopes)
    {
        Source = source;
        MapNameSpace = mapNameSpace;
        Scopes = scopes;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AttributeEvents()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>AttributeEvent DataBuildNum</summary>
    ///
    public int Source { get; init; }
    /// <summary>AttributeEvent DataBuildNum</summary>
    ///
    public int MapNameSpace { get; init; }
    /// <summary>AttributeEvent Scopes</summary>
    ///
    public ICollection<AttributeEventScope> Scopes { get; init; }
}
/// <summary>Record <c>AttributeEventScope</c> Parsed replay AttributeEvents scopes</summary>
///
public sealed record AttributeEventScope
{
    /// <summary>Record <c>AttributeEventScope</c> constructor</summary>
    ///
    public AttributeEventScope(int scope,
                               int sopeId,
                               int @namespace,
                               int attrid,
                               string value)
    {
        Scope = scope;
        SopeId = sopeId;
        Namespace = @namespace;
        Attrid = attrid;
        Value = value;
    }

    [JsonConstructor]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public AttributeEventScope()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {

    }

    /// <summary>AttributeEvent Scopes Scope</summary>
    ///
    public int Scope { get; init; }
    /// <summary>AttributeEvent Scopes SopeId</summary>
    ///
    public int SopeId { get; init; }
    /// <summary>AttributeEvent Scopes Namespace</summary>
    ///
    public int Namespace { get; init; }
    /// <summary>AttributeEvent Scopes Attrid</summary>
    ///
    public int Attrid { get; init; }
    /// <summary>AttributeEvent Scopes Value</summary>
    ///
    public string Value { get; init; }
}
