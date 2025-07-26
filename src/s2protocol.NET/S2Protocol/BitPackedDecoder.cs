using System.Reflection;

namespace s2protocol.NET.S2Protocol;

internal class BitPackedDecoder : S2ProtocolDecoder
{
    private readonly BitPackedBuffer _buffer;
    private readonly List<S2TypeInfo> _typeInfos;

    public BitPackedDecoder(byte[] contents, List<S2TypeInfo> typeinfos)
    {
        _buffer = new BitPackedBuffer(contents);
        _typeInfos = typeinfos;
    }

    public override string ToString() => _buffer.ToString();
    public override bool Done() => _buffer.Done();
    public override long UsedBits() => _buffer.UsedBits();
    public override void ByteAlign() => _buffer.ByteAlign();

    public override object? Instance(int typeid)
    {
        if (typeid >= _typeInfos.Count)
            throw new DecodeException(nameof(BitPackedDecoder));

        var typeInfo = _typeInfos[typeid];
        var method = GetType().GetMethod(typeInfo.TypeName, BindingFlags.NonPublic | BindingFlags.Instance)
                     ?? throw new DecodeException($"Unknown method: {typeInfo.TypeName}");

        var parameters = PrepareParameters(typeInfo);
        return method.Invoke(this, new object[] { parameters });
    }

    private long ReadInt(BoundsParameter bounds)
        => bounds.Min + _buffer.ReadBits((int)bounds.Max);

    private object? _array(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds, TypeIdParameter type])
        {
            var length = ReadInt(bounds);
            var result = new List<object?>();
            for (long i = 0; i < length; i++)
                result.Add(Instance(type.TypeId));
            return result;
        }
        throw new ArgumentException("Invalid parameters for _array");
    }

    private object? _bitarray(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
        {
            var length = ReadInt(bounds);
            return (length, _buffer.ReadBits((int)length));
        }
        throw new ArgumentException("Invalid parameters for _bitarray");
    }

    private object? _blob(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
        {
            var length = ReadInt(bounds);
            return _buffer.ReadAlignedBytes((int)length);
        }
        throw new ArgumentException("Invalid parameters for _blob");
    }

    private bool _bool(IDecodeParameter[] _) => ReadInt(new BoundsParameter(0, 1)) != 0;

    private object? _choice(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds, ChoiceParameter choices])
        {
            var tag = ReadInt(bounds);
            if (!choices.Choices.TryGetValue((int)tag, out var choice))
                throw new DecodeException(nameof(BitPackedDecoder));

            return new Dictionary<string, object?>
            {
                [choice.Name] = Instance(choice.TypeId)
            };
        }
        throw new ArgumentException("Invalid parameters for _choice");
    }

    private object? _fourcc(IDecodeParameter[] _) => _buffer.ReadUnalignedBytes(4);

    private long _int(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
            return ReadInt(bounds);
        throw new ArgumentException("Invalid parameters for _int");
    }

    private object? _null(IDecodeParameter[] _) => null;

    private object? _optional(IDecodeParameter[] parameters)
    {
        if (parameters is [TypeIdParameter type])
            return _bool([]) ? Instance(type.TypeId) : null;
        return null;
    }

    private float _real32(IDecodeParameter[] _)
    {
        var bytes = _buffer.ReadUnalignedBytes(4);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    private double _real64(IDecodeParameter[] _)
    {
        var bytes = _buffer.ReadUnalignedBytes(8);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    private object? _struct(IDecodeParameter[] parameters)
    {
        if (parameters is [FieldListParameter fieldList])
        {
            var result = new Dictionary<string, object?>();
            foreach (var field in fieldList.Fields)
            {
                if (field.Name == "__parent")
                {
                    var parent = Instance(field.TypeId);
                    if (parent is Dictionary<string, object?> parentDict)
                        foreach (var kv in parentDict)
                            result[kv.Key] = kv.Value;
                    else if (fieldList.Fields.Count == 1)
                        return parent;
                    else
                        result[field.Name] = parent;
                }
                else
                {
                    result[field.Name] = Instance(field.TypeId);
                }
            }
            return result;
        }
        throw new ArgumentException("Invalid parameters for _struct");
    }

    public override IDecodeParameter[] PrepareParameters(S2TypeInfo typeInfo)
    {
        var parameters = new List<IDecodeParameter>();

        foreach (var element in typeInfo.Elements)
        {
            switch (element)
            {
                case S2TypeInfoTypeElement bounds:
                    if (bounds.Bounds.Max == -1)
                    {
                        parameters.Add(new TypeIdParameter((int)bounds.Bounds.Min));
                    }
                    else
                    {
                        parameters.Add(new BoundsParameter(bounds.Bounds.Min, bounds.Bounds.Max));
                    }
                    break;

                case S2TypeInfoMElement mElement:
                    parameters.Add(new FieldListParameter(
                        mElement.Elements.Select(e =>
                            new DecodeField(e.TypeName, (int)e.Bounds.Min, e.Bounds.Max)
                        ).ToList()
                    ));
                    break;

                case DsTypeInfoChoiceElemet choiceElem:
                    var dict = new Dictionary<long, DecodeChoice>();
                    foreach (var kv in choiceElem.Elements)
                        dict[kv.Key] = new DecodeChoice(kv.Value.TypeName, kv.Value.Number);
                    parameters.Add(new ChoiceParameter(dict));
                    break;

                default:
                    throw new NotSupportedException($"Unknown element type: {element.GetType().Name}");
            }
        }

        return parameters.ToArray();
    }
}

