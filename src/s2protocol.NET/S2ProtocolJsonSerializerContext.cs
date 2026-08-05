using s2protocol.NET.Models;
using System.Text.Json.Serialization;

namespace s2protocol.NET;

/// <summary>Source-generated JSON metadata for the complete public replay graph.</summary>
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Sc2Replay))]
[JsonSerializable(typeof(Header))]
[JsonSerializable(typeof(Initdata))]
[JsonSerializable(typeof(Details))]
[JsonSerializable(typeof(ReplayMetadata))]
[JsonSerializable(typeof(MetadataPlayer))]
[JsonSerializable(typeof(GameEvents))]
[JsonSerializable(typeof(MessageEvents))]
[JsonSerializable(typeof(TrackerEvents))]
[JsonSerializable(typeof(AttributeEvents))]
public sealed partial class S2ProtocolJsonSerializerContext : JsonSerializerContext;
