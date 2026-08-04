using System.Globalization;
using System.Text;

namespace s2protocol.NET.S2Protocol;

/// <summary>
/// S2ProtocolVersion
/// </summary>
public sealed partial record S2ProtocolVersion
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public int Version { get; set; }
    internal List<S2TypeInfo> TypeInfos { get; set; } = [];
    internal Dictionary<int, S2EventType> GameEvents = [];
    internal Dictionary<int, S2EventType> MessageEvents { get; set; } = [];
    internal Dictionary<int, S2EventType> TrackerEvents { get; set; } = [];
    public int? GameEventIdTypeId { get; set; }
    public int? MessageEventIdTypeId { get; set; }
    public int? TrackerEventIdTypeId { get; set; }
    public int? SVarUint32TypeId { get; set; }
    public int? ReplayUserIdTypeId { get; set; }
    public int? ReplayHeaderTypeId { get; set; }
    public int? GameDetailsTypeId { get; set; }
    public int? ReplayInitDataTypeId { get; set; }

    internal object? DecodeReplayHeaderRaw(byte[] content)
    {
        var decoder = new VersionedDecoder(content, TypeInfos);
        return decoder.Instance(ReplayHeaderTypeId ?? 18);
    }

    internal IEnumerable<Dictionary<string, object?>> DecodeReplayGameEventsRaw(byte[] content)
    {
        var decoder = new BitPackedDecoder(content, TypeInfos);
        foreach (var gameEvent in DecodeEventStream(decoder, GameEventIdTypeId ?? 0, GameEvents, true))
        {
            yield return gameEvent;
        }
    }

    internal IEnumerable<Dictionary<string, object?>> DecodeReplayMessageEventsRaw(byte[] content)
    {
        var decoder = new BitPackedDecoder(content, TypeInfos);
        foreach (var messageEvent in DecodeEventStream(decoder, MessageEventIdTypeId ?? 1, MessageEvents, true))
        {
            yield return messageEvent;
        }
    }

    internal IEnumerable<Dictionary<string, object?>> DecodeReplayTrackerEventsRaw(byte[] content)
    {
        var decoder = new VersionedDecoder(content, TypeInfos);
        foreach (var trackerEvent in DecodeEventStream(decoder, TrackerEventIdTypeId ?? 2, TrackerEvents, false))
        {
            yield return trackerEvent;
        }
    }

    internal object? DecodeReplayDetailsRaw(byte[] content)
    {
        var decoder = new VersionedDecoder(content, TypeInfos);
        return decoder.Instance(GameDetailsTypeId ?? 40);
    }

    internal object? DecodeReplayInitDataDictionaryRaw(byte[] content)
    {
        var decoder = new BitPackedDecoder(content, TypeInfos);
        return decoder.Instance(ReplayInitDataTypeId ?? 73);
    }

    private const int AttributeHeaderSize = 9;
    private const int AttributeRecordSize = 13;
    private const int AttributeValueSize = 4;

    public static Dictionary<string, object> DecodeReplayAttributeEventsRaw(byte[] content)
        => DecodeReplayAttributeEventsRaw((ReadOnlyMemory<byte>)content);

    public static Dictionary<string, object> DecodeReplayAttributeEventsRaw(ReadOnlyMemory<byte> content)
    {
        BitPackedBuffer buffer = new(content, "little");
        Dictionary<string, object> attributes = new(3);

        if (buffer.Done())
        {
            return attributes;
        }

        attributes["source"] = buffer.ReadBits(8);
        attributes["mapNamespace"] = buffer.ReadBits(32);
        long declaredCount = buffer.ReadBits(32);

        Dictionary<byte, Dictionary<uint, List<Dictionary<string, object>>>> scopes = [];
        attributes["scopes"] = scopes;

        while (!buffer.Done())
        {
            long ns = buffer.ReadBits(32);
            long attrid = buffer.ReadBits(32);
            byte scope = (byte)buffer.ReadBits(8);
            string valueText = DecodeAttributeValue(buffer.ReadAlignedSpan(AttributeValueSize));
            Dictionary<string, object> value = new(3)
            {
                ["namespace"] = ns,
                ["attrid"] = attrid,
                ["value"] = valueText,
            };

            if (!scopes.TryGetValue(scope, out var scopeDict))
            {
                scopeDict = [];
                scopes[scope] = scopeDict;
            }

            if (!scopeDict.TryGetValue((uint)attrid, out var attrList))
            {
                attrList = new List<Dictionary<string, object>>(
                    Math.Min(GetAttributeCapacity(content.Length, declaredCount), 4));
                scopeDict[(uint)attrid] = attrList;
            }

            attrList.Add(value);
        }

        return attributes;
    }

    private static int GetAttributeCapacity(int contentLength, long declaredCount)
    {
        int payloadCapacity = Math.Max(0, contentLength - AttributeHeaderSize) / AttributeRecordSize;
        return (int)Math.Min(Math.Max(0, declaredCount), payloadCapacity);
    }

    private static string DecodeAttributeValue(ReadOnlySpan<byte> rawValue)
    {
        Span<byte> decodedValue = stackalloc byte[AttributeValueSize];
        rawValue.CopyTo(decodedValue);
        decodedValue.Reverse();

        int start = 0;
        while (start < decodedValue.Length && decodedValue[start] == 0)
        {
            start++;
        }

        int end = decodedValue.Length;
        while (end > start && decodedValue[end - 1] == 0)
        {
            end--;
        }

        return Encoding.UTF8.GetString(decodedValue[start..end]);
    }

    private IEnumerable<Dictionary<string, object?>> DecodeEventStream(
     S2ProtocolDecoder decoder,
     int eventIdTypeId,
     Dictionary<int, S2EventType> eventTypes,
     bool decodeUserId)
    {
        int gameloop = 0;

        while (!decoder.Done())
        {
            var startBits = decoder.UsedBits();

            // Decode the gameloop delta
            var svaruint32 = decoder.Instance(SVarUint32TypeId ?? 7);
            if (svaruint32 is Dictionary<string, object> svaruint32dict)
            {
                int delta = Varuint32Value(svaruint32dict);
                gameloop += delta;
            }

            int userid = -1;
            if (decodeUserId)
            {
                var useridRaw = decoder.Instance(ReplayUserIdTypeId ?? 8);
                if (useridRaw is int useridInt)
                {
                    userid = useridInt;
                }
                else if (useridRaw is Dictionary<string, object> useridDict)
                {
                    userid = Varuint32Value(useridDict);
                }
                else
                {
                    throw new InvalidCastException("userid is not an int or dictionary");
                }
            }

            var eventidRaw = decoder.Instance(eventIdTypeId);
            if (eventidRaw is not long eventid)
            {
                throw new InvalidCastException("eventid is no long");
            }

            if (!eventTypes.TryGetValue((int)eventid, out var eventTypeInfo))
            {
                // throw new Exception($"CorruptedError: eventid({eventid}) at {decoder}");
                eventTypeInfo = new S2EventType(-1, "UnknownEvent");
            }

            int typeid = eventTypeInfo.TypeId;
            string typename = eventTypeInfo.Name;

            var eventInstance = decoder.InstanceDict(typeid);
            eventInstance["_event"] = typename;
            eventInstance["_eventid"] = eventid;
            eventInstance["_gameloop"] = gameloop;
            if (decodeUserId)
            {
                eventInstance["_userid"] = userid;
            }

            decoder.ByteAlign();
            eventInstance["_bits"] = decoder.UsedBits() - startBits;

            yield return eventInstance;
        }
    }

    private static int Varuint32Value(Dictionary<string, object> value)
    {
        foreach (var v in value.Values)
        {
            return Convert.ToInt32(v, CultureInfo.InvariantCulture);
        }
        return 0;
    }

    public static int UnitTag(int unitTagIndex, int unitTagRecycle)
    {
        return (unitTagIndex << 18) + unitTagRecycle;
    }

    public static int UnitTagIndex(int unitTag)
    {
        return (unitTag >> 18) & 0x00003FFF;
    }

    public static int UnitTagRecycle(int unitTag)
    {
        return unitTag & 0x0003FFFF;
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
