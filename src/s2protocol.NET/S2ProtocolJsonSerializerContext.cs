using s2protocol.NET.Models;
using System.Text.Json.Serialization;

namespace s2protocol.NET;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(ReplayMetadata))]
internal sealed partial class ReplayMetadataJsonSerializerContext : JsonSerializerContext;
