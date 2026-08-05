using s2protocol.NET;
using System.Text.Json.Serialization;

namespace s2protocol.NET.tests;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(Sc2Replay))]
internal sealed partial class TestJsonSerializerContext : JsonSerializerContext;
