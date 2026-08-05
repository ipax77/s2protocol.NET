using s2protocol.NET;
using s2protocol.NET.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace s2cli;

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
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(JsonElement))]
internal sealed partial class S2CliJsonSerializerContext : JsonSerializerContext;
