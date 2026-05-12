using System.Text.Json.Serialization;

namespace s2protocol.NET.S2Protocol;

internal sealed record CompactProtocolVersion
{
    [JsonPropertyName("v")]
    public int Version { get; init; }
    [JsonPropertyName("t")]
    public List<CompactTypeInfo> TypeInfos { get; init; } = [];
    [JsonPropertyName("ge")]
    public Dictionary<int, CompactEventType> GameEvents { get; init; } = [];
    [JsonPropertyName("me")]
    public Dictionary<int, CompactEventType> MessageEvents { get; init; } = [];
    [JsonPropertyName("te")]
    public Dictionary<int, CompactEventType> TrackerEvents { get; init; } = [];
    [JsonPropertyName("gei")]
    public int? GameEventIdTypeId { get; init; }
    [JsonPropertyName("mei")]
    public int? MessageEventIdTypeId { get; init; }
    [JsonPropertyName("tei")]
    public int? TrackerEventIdTypeId { get; init; }
    [JsonPropertyName("sv")]
    public int? SVarUint32TypeId { get; init; }
    [JsonPropertyName("ru")]
    public int? ReplayUserIdTypeId { get; init; }
    [JsonPropertyName("rh")]
    public int? ReplayHeaderTypeId { get; init; }
    [JsonPropertyName("gd")]
    public int? GameDetailsTypeId { get; init; }
    [JsonPropertyName("ri")]
    public int? ReplayInitDataTypeId { get; init; }

    public S2ProtocolVersion ToProtocolVersion()
    {
        S2ProtocolVersion protocolVersion = new()
        {
            Version = Version,
            GameEventIdTypeId = GameEventIdTypeId,
            MessageEventIdTypeId = MessageEventIdTypeId,
            TrackerEventIdTypeId = TrackerEventIdTypeId,
            SVarUint32TypeId = SVarUint32TypeId,
            ReplayUserIdTypeId = ReplayUserIdTypeId,
            ReplayHeaderTypeId = ReplayHeaderTypeId,
            GameDetailsTypeId = GameDetailsTypeId,
            ReplayInitDataTypeId = ReplayInitDataTypeId,
        };

        for (int i = 0; i < TypeInfos.Count; i++)
        {
            protocolVersion.TypeInfos.Add(TypeInfos[i].ToTypeInfo(i));
        }

        CopyEventTypes(GameEvents, protocolVersion.GameEvents);
        CopyEventTypes(MessageEvents, protocolVersion.MessageEvents);
        CopyEventTypes(TrackerEvents, protocolVersion.TrackerEvents);

        return protocolVersion;
    }

    private static void CopyEventTypes(
        Dictionary<int, CompactEventType> source,
        Dictionary<int, S2EventType> target)
    {
        foreach (var (eventId, eventType) in source)
        {
            target[eventId] = new S2EventType(eventType.TypeId, eventType.Name);
        }
    }
}

internal sealed record CompactTypeInfo
{
    [JsonPropertyName("n")]
    public string TypeName { get; init; } = string.Empty;
    [JsonPropertyName("b")]
    public long[]? Bounds { get; init; }
    [JsonPropertyName("i")]
    public int? TypeId { get; init; }
    [JsonPropertyName("f")]
    public List<CompactField>? Fields { get; init; }
    [JsonPropertyName("c")]
    public Dictionary<int, CompactChoice>? Choices { get; init; }

    public S2TypeInfo ToTypeInfo(int number)
    {
        S2TypeInfo typeInfo = new(TypeName, number);

        if (Bounds is not null)
        {
            if (Bounds.Length != 2)
            {
                throw new DecodeException($"Invalid bounds in typeinfo {number}.");
            }

            typeInfo.Elements.Add(new S2TypeInfoTypeElement
            {
                Bounds = new BoundsParameter(Bounds[0], Bounds[1])
            });
        }

        if (TypeId is not null)
        {
            typeInfo.Elements.Add(new S2TypeInfoTypeElement
            {
                Bounds = new BoundsParameter(TypeId.Value, -1)
            });
        }

        if (Fields is not null)
        {
            var fieldElement = new S2TypeInfoMElement();
            foreach (var field in Fields)
            {
                fieldElement.Elements.Add(new S2MElement
                {
                    TypeName = field.Name,
                    Bounds = new BoundsParameter(field.TypeId, field.Tag)
                });
            }

            typeInfo.Elements.Add(fieldElement);
        }

        if (Choices is not null)
        {
            var choiceElement = new DsTypeInfoChoiceElemet();
            foreach (var (tag, choice) in Choices)
            {
                choiceElement.Elements[tag] = new S2ChoiceElement
                {
                    TypeName = choice.Name,
                    Number = choice.TypeId
                };
            }

            typeInfo.Elements.Add(choiceElement);
        }

        return typeInfo;
    }
}

internal sealed record CompactField
{
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;
    [JsonPropertyName("i")]
    public int TypeId { get; init; }
    [JsonPropertyName("g")]
    public long Tag { get; init; }
}

internal sealed record CompactChoice
{
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;
    [JsonPropertyName("i")]
    public int TypeId { get; init; }
}

internal sealed record CompactEventType
{
    [JsonPropertyName("i")]
    public int TypeId { get; init; }
    [JsonPropertyName("n")]
    public string Name { get; init; } = string.Empty;
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(CompactProtocolVersion))]
internal sealed partial class S2ProtocolTypeInfoJsonSerializerContext : JsonSerializerContext;
