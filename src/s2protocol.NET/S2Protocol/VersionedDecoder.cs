using System.Reflection;

namespace s2protocol.NET.S2Protocol;

internal sealed class VersionedDecoder : S2ProtocolDecoder
{
    private BitPackedBuffer _buffer;
    private List<S2TypeInfo> _typeInfos;

    public VersionedDecoder(byte[] contents, List<S2TypeInfo> typeinfos)
    {
        _buffer = new BitPackedBuffer(contents);
        _typeInfos = typeinfos;
    }

    public override string ToString() => _buffer.ToString();

    public override object? Instance(int typeid)
    {
        if (typeid >= _typeInfos.Count)
            throw new DecodeException(nameof(VersionedDecoder));

        var typeInfo = _typeInfos[typeid];
        string methodName = typeInfo.TypeName;

        var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (method == null)
            throw new DecodeException($"Unknown method: {methodName}");

        IDecodeParameter[] parameters = PrepareParameters(typeInfo);
        return method.Invoke(this, new object[] { parameters });
    }

    public override bool Done() => _buffer.Done();
    public override long UsedBits() => _buffer.UsedBits();
    public override void ByteAlign() => _buffer.ByteAlign();

    private void _expect_skip(int expected)
    {
        var bits = _buffer.ReadBits(8);
        if (bits != expected)
            throw new DecodeException(nameof(VersionedDecoder));
    }

    private long _vint()
    {
        var b = _buffer.ReadBits(8);
        bool negative = (b & 1) != 0;
        long result = (b >> 1) & 0x3F;
        int bits = 6;

        while ((b & 0x80) != 0)
        {
            b = _buffer.ReadBits(8);
            result |= ((long)(b & 0x7F)) << bits;
            bits += 7;
        }

        return negative ? -result : result;
    }

    private List<object?> _array(IDecodeParameter[] decodeParameters)
    {
        var list = new List<object?>();
        if (decodeParameters.Length == 2
            && decodeParameters[0] is BoundsParameter bounds
            && decodeParameters[1] is TypeIdParameter typeParam
        )
        {
            _expect_skip(0);
            long length = _vint();
            for (long i = 0; i < length; i++)
            {
                list.Add(Instance(typeParam.TypeId));
            }
        }
        return list;
    }

    private (long, byte[]) _bitarray(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(1);
        long length = _vint();
        long byteLength = (length + 7) / 8;
        return (length, _buffer.ReadAlignedBytes(byteLength));
    }

    private byte[] _blob(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(2);
        long length = _vint();
        return _buffer.ReadAlignedBytes(length);
    }

    private bool _bool(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(6);
        return _buffer.ReadBits(8) != 0;
    }

    private Dictionary<string, object?> _choice(IDecodeParameter[] decodeParameters)
    {
        //BoundsParameter bounds, Dictionary<long, DecodeChoice> fields
        if (decodeParameters.Length == 2
            && decodeParameters[0] is BoundsParameter bounds
            && decodeParameters[1] is ChoiceParameter choiceParam)
        {

            _expect_skip(3);
            long tag = _vint();
            if (!choiceParam.Choices.TryGetValue(tag, out DecodeChoice? field))
            {
                _skip_instance();
                return [];
            }

            return new Dictionary<string, object?> { { field.Name, Instance(field.TypeId) } };
        }
        return [];
    }

    private byte[] _fourcc(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(7);
        return _buffer.ReadAlignedBytes(4);
    }

    private long _int(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(9);
        return _vint();
    }

    private static object? _null(IDecodeParameter[] decodeParameters) => null;

    private object? _optional(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(4);
        bool exists = _buffer.ReadBits(8) != 0;
        if (decodeParameters.Length == 1
            && decodeParameters[0] is TypeIdParameter typeParam)
        {
            return exists ? Instance(typeParam.TypeId) : null;
        }
        else
        {
            throw new NotSupportedException("Optional without TypeIdParameter is not supported.");
        }
    }

    private float _real32(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(7);
        var bytes = _buffer.ReadAlignedBytes(4);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    private double _real64(IDecodeParameter[] decodeParameters)
    {
        _expect_skip(8);
        var bytes = _buffer.ReadAlignedBytes(8);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    private object? _struct(IDecodeParameter[] decodeParameters)
    {
        if (decodeParameters.Length == 1 && decodeParameters[0] is FieldListParameter fieldListParam)
        {
            _expect_skip(5);
            var result = new Dictionary<string, object?>();
            long length = _vint();

            for (long i = 0; i < length; i++)
            {
                long tag = _vint();
                var field = fieldListParam.Fields.FirstOrDefault(f => f.Tag == tag);
                if (field != default)
                {
                    if (field.Name == "__parent")
                    {
                        var parent = Instance(field.TypeId);
                        if (parent is Dictionary<string, object?> dict)
                            foreach (var kv in dict) result[kv.Key] = kv.Value;
                        else if (fieldListParam.Fields.Count == 1)
                            return parent;
                        else
                            result[field.Name] = parent;
                    }
                    else
                    {
                        result[field.Name] = Instance(field.TypeId);
                    }
                }
                else
                {
                    _skip_instance();
                }
            }

            return result;
        }
        return null;
    }

    private void _skip_instance()
    {
        var skip = _buffer.ReadBits(8);

        switch (skip)
        {
            case 0: // array
                {
                    long length = _vint();
                    for (long i = 0; i < length; i++)
                        _skip_instance();
                    break;
                }
            case 1: // bitblob
                {
                    long length = _vint();
                    long byteLength = (length + 7) / 8;
                    _buffer.ReadAlignedBytes(byteLength);
                    break;
                }
            case 2: // blob
                {
                    long length = _vint();
                    _buffer.ReadAlignedBytes(length);
                    break;
                }
            case 3: // choice
                {
                    _vint(); // tag
                    _skip_instance();
                    break;
                }
            case 4: // optional
                {
                    bool exists = _buffer.ReadBits(8) != 0;
                    if (exists) _skip_instance();
                    break;
                }
            case 5: // struct
                {
                    long length = _vint();
                    for (long i = 0; i < length; i++)
                    {
                        _vint(); // tag
                        _skip_instance();
                    }
                    break;
                }
            case 6: // u8
                _buffer.ReadAlignedBytes(1);
                break;
            case 7: // u32
                _buffer.ReadAlignedBytes(4);
                break;
            case 8: // u64
                _buffer.ReadAlignedBytes(8);
                break;
            case 9: // vint
                _vint();
                break;
            default:
                throw new DecodeException(nameof(VersionedDecoder));
        }
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
                    var fieldList = new List<DecodeField>();
                    foreach (var m in mElement.Elements)
                    {
                        fieldList.Add(new(m.TypeName, (int)m.Bounds.Min, m.Bounds.Max));
                    }
                    parameters.Add(new FieldListParameter(fieldList));
                    break;

                case DsTypeInfoChoiceElemet choiceElement:
                    var dict = new Dictionary<long, DecodeChoice>();
                    foreach (var kv in choiceElement.Elements)
                    {
                        dict[kv.Key] = new(kv.Value.TypeName, kv.Value.Number);
                    }
                    parameters.Add(new ChoiceParameter(dict));
                    break;

                default:
                    throw new NotSupportedException($"Unsupported element type: {element.GetType().Name}");
            }
        }

        return parameters.ToArray();
    }

}