using s2protocol.NET.Models;
using System.Text.Json.Serialization;

namespace s2protocol.NET;

[JsonSerializable(typeof(ReplayMetadata))]
[JsonSerializable(typeof(MetadataPlayer))]
internal sealed partial class S2ProtocolJsonSerializerContext : JsonSerializerContext;
