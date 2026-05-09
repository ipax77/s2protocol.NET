using System.Text.Json.Serialization;
using s2protocol.NET.Models;

namespace s2protocol.NET;

[JsonSerializable(typeof(ReplayMetadata))]
[JsonSerializable(typeof(MetadataPlayer))]
internal sealed partial class S2ProtocolJsonSerializerContext : JsonSerializerContext;
