using System.Globalization;

namespace s2protocol.NET.S2Protocol;

/// <summary>
/// S2ProtocolVersion
/// </summary>
public sealed record S2ProtocolVersion
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

    public object? DecodeReplayHeader(byte[] content)
    {
        var decoder = new VersionedDecoder(content, TypeInfos);
        return decoder.Instance(ReplayHeaderTypeId ?? 18);
    }

    public IEnumerable<Dictionary<string, object?>> DecodeReplayGameEvents(byte[] content)
    {
        var decoder = new BitPackedDecoder(content, TypeInfos);
        foreach (var gameEvent in DecodeEventStream(decoder, GameEventIdTypeId ?? 0, GameEvents, true))
        {
            yield return gameEvent;
        }
    }

    public IEnumerable<Dictionary<string, object?>> DecodeReplayMessageEvents(byte[] content)
    {
        var decoder = new BitPackedDecoder(content, TypeInfos);
        foreach (var messageEvent in DecodeEventStream(decoder, MessageEventIdTypeId ?? 1, MessageEvents, true))
        {
            yield return messageEvent;
        }
    }

    public IEnumerable<Dictionary<string, object?>> DecodeReplayTrackerEvents(byte[] content)
    {
        var decoder = new VersionedDecoder(content, TypeInfos);
        foreach (var trackerEvent in DecodeEventStream(decoder, TrackerEventIdTypeId ?? 2, TrackerEvents, false))
        {
            yield return trackerEvent;
        }
    }

    public object? DecodeReplayDetails(byte[] content)
    {
        var decoder = new VersionedDecoder(content, TypeInfos);
        return decoder.Instance(GameDetailsTypeId ?? 40);
    }

    public object? DecodeReplayInitDataRaw(byte[] content)
    {
        var decoder = new BitPackedDecoder(content, TypeInfos);
        return decoder.Instance(ReplayInitDataTypeId ?? 73);
    }

    public static Dictionary<string, object> DecodeReplayAttributeEventsRaw(byte[] content)
    {
        var buffer = new BitPackedBuffer(content, "little");
        var attributes = new Dictionary<string, object>();

        if (buffer.Done())
        {
            return attributes;
        }

        // Read initial attributes
        attributes["source"] = buffer.ReadBits(8);
        attributes["mapNamespace"] = buffer.ReadBits(32);
        var count = buffer.ReadBits(32);

        var scopes = new Dictionary<byte, Dictionary<uint, List<Dictionary<string, object>>>>();
        attributes["scopes"] = scopes;

        while (!buffer.Done())
        {
            var value = new Dictionary<string, object>();

            var ns = buffer.ReadBits(32);
            var attrid = buffer.ReadBits(32);
            byte scope = (byte)buffer.ReadBits(8);
            byte[] rawValue = buffer.ReadAlignedBytes(4);

            // Reverse and strip null bytes
            Array.Reverse(rawValue);
            string cleanedValue = System.Text.Encoding.UTF8.GetString(rawValue).TrimEnd('\0');

            value["namespace"] = ns;
            value["attrid"] = attrid;
            value["value"] = cleanedValue;

            // Ensure the scope exists
            if (!scopes.TryGetValue(scope, out var scopeDict))
            {
                scopeDict = new Dictionary<uint, List<Dictionary<string, object>>>();
                scopes[scope] = scopeDict;
            }
            // Ensure the attribute ID list exists
            if (!scopeDict.TryGetValue((uint)attrid, out var attrList))
            {
                attrList = new List<Dictionary<string, object>>();
                scopeDict[(uint)attrid] = attrList;
            }

            attrList.Add(value);
            {
                scopes[scope][(uint)attrid] = new List<Dictionary<string, object>>();
            }

            scopes[scope][(uint)attrid].Add(value);
        }

        return attributes;
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
