namespace s2protocol.NET.Models;
/// <summary>Record <c>AttributeEvents</c> Parsed replay AttributeEvents</summary>
///
/// <remarks>Record <c>AttributeEvents</c> constructor</remarks>
///
public sealed class AttributeEvents(int source,
                       int mapNameSpace,
                       ICollection<AttributeEventScope> scopes)
{

    /// <summary>AttributeEvent DataBuildNum</summary>
    ///
    public int Source { get; init; } = source;
    /// <summary>AttributeEvent DataBuildNum</summary>
    ///
    public int MapNameSpace { get; init; } = mapNameSpace;
    /// <summary>AttributeEvent Scopes</summary>
    ///
    public ICollection<AttributeEventScope> Scopes { get; init; } = scopes;
}
/// <summary>Record <c>AttributeEventScope</c> Parsed replay AttributeEvents scopes</summary>
///
/// <remarks>Record <c>AttributeEventScope</c> constructor</remarks>
///
public sealed class AttributeEventScope(int scope,
                           int sopeId,
                           int @namespace,
                           int attrid,
                           string value)
{
    /// <summary>AttributeEvent Scopes Scope</summary>
    ///
    public int Scope { get; init; } = scope;
    /// <summary>AttributeEvent Scopes SopeId</summary>
    ///
    public int SopeId { get; init; } = sopeId;
    /// <summary>AttributeEvent Scopes Namespace</summary>
    ///
    public int Namespace { get; init; } = @namespace;
    /// <summary>AttributeEvent Scopes Attrid</summary>
    ///
    public int Attrid { get; init; } = attrid;
    /// <summary>AttributeEvent Scopes Value</summary>
    ///
    public string Value { get; init; } = value;
}
