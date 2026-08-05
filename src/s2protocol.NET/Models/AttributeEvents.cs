using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

/// <summary>Attributes decoded from <c>replay.attributes.events</c>.</summary>
public sealed class AttributeEvents
{
    /// <summary>Creates a decoded attribute collection.</summary>
    [JsonConstructor]
    public AttributeEvents(int source, int mapNamespace, ICollection<AttributeEventScope> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        Source = source;
        MapNamespace = mapNamespace;
        Attributes = attributes;
    }

    /// <summary>Gets the attribute source identifier.</summary>
    public int Source { get; init; }

    /// <summary>Gets the map namespace.</summary>
    public int MapNamespace { get; init; }

    /// <summary>Gets the decoded attributes in stream order.</summary>
    public ICollection<AttributeEventScope> Attributes { get; init; }
}

/// <summary>A single scoped attribute decoded from <c>replay.attributes.events</c>.</summary>
public sealed class AttributeEventScope
{
    /// <summary>Creates a decoded scoped attribute.</summary>
    [JsonConstructor]
    public AttributeEventScope(int scope, int attributeId, int @namespace, string value)
    {
        Scope = scope;
        AttributeId = attributeId;
        Namespace = @namespace;
        Value = value;
    }

    /// <summary>Gets the scope identifier.</summary>
    public int Scope { get; init; }

    /// <summary>Gets the attribute identifier.</summary>
    public int AttributeId { get; init; }

    /// <summary>Gets the attribute namespace.</summary>
    public int Namespace { get; init; }

    /// <summary>Gets the decoded four-byte attribute value.</summary>
    public string Value { get; init; }
}
