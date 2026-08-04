using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Attributes decoded from <c>replay.attributes.events</c>.</summary>
/// <remarks>Creates a decoded attribute collection.</remarks>
#pragma warning disable CA1708 // The legacy MapNameSpace API is retained for source compatibility.
public sealed class AttributeEvents
{
    private int _mapNamespace;
    private ICollection<AttributeEventScope> _attributes;

    /// <summary>Creates a decoded attribute collection.</summary>
    public AttributeEvents(int source, int mapNameSpace, ICollection<AttributeEventScope> scopes)
    {
        Source = source;
        _mapNamespace = mapNameSpace;
        _attributes = scopes;
    }

    /// <summary>Creates an empty attribute collection for JSON deserialization.</summary>
    [JsonConstructor]
    public AttributeEvents()
    {
        _attributes = [];
    }

    /// <summary>Gets the attribute source identifier.</summary>
    public int Source { get; init; }

    /// <summary>Gets the map namespace.</summary>
    [JsonIgnore]
    public int MapNamespace
    {
        get => _mapNamespace;
        init => _mapNamespace = value;
    }

    /// <summary>Gets the decoded attributes in stream order.</summary>
    [JsonIgnore]
    public ICollection<AttributeEventScope> Attributes
    {
        get => _attributes;
        init => _attributes = value;
    }

    /// <summary>Gets the map namespace.</summary>
    [Obsolete($"Use {nameof(MapNamespace)} instead.")]
    public int MapNameSpace
    {
        get => _mapNamespace;
        init => _mapNamespace = value;
    }

    /// <summary>Gets the decoded attributes in stream order.</summary>
    [Obsolete($"Use {nameof(Attributes)} instead.")]
    public ICollection<AttributeEventScope> Scopes
    {
        get => _attributes;
        init => _attributes = value;
    }
}
#pragma warning restore CA1708

/// <summary>A single scoped attribute decoded from <c>replay.attributes.events</c>.</summary>
/// <remarks>Creates a decoded scoped attribute.</remarks>
public sealed class AttributeEventScope
{
    private int _attributeId;

    /// <summary>Creates a decoded scoped attribute.</summary>
    public AttributeEventScope(int scope, int sopeId, int @namespace, int attrid, string value)
    {
        Scope = scope;
        _attributeId = attrid;
        Namespace = @namespace;
        Value = value;
        _ = sopeId;
    }

    /// <summary>Gets the scope identifier.</summary>
    public int Scope { get; init; }

    /// <summary>Gets the attribute identifier.</summary>
    [JsonIgnore]
    public int AttributeId
    {
        get => _attributeId;
        init => _attributeId = value;
    }

    /// <summary>Gets the legacy grouped attribute identifier.</summary>
    [Obsolete($"Use {nameof(AttributeId)} instead.")]
    public int SopeId
    {
        get => _attributeId;
        init => _attributeId = value;
    }

    /// <summary>Gets the attribute namespace.</summary>
    public int Namespace { get; init; }

    /// <summary>Gets the attribute identifier.</summary>
    [Obsolete($"Use {nameof(AttributeId)} instead.")]
    public int Attrid
    {
        get => _attributeId;
        init => _attributeId = value;
    }

    /// <summary>Gets the decoded four-byte attribute value.</summary>
    public string Value { get; init; }
}
