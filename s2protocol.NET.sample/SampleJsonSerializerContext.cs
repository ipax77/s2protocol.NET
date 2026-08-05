using s2protocol.NET;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Sc2Replay))]
internal sealed partial class SampleJsonSerializerContext : JsonSerializerContext;
