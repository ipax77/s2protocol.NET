using System.Reflection;

namespace s2protocol.NET.S2Protocol;

internal sealed class BitPackedDecoder : S2ProtocolDecoder
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
        if (typeInfo.DecodeKind == S2DecodeKind.Unknown)
        {
            throw new DecodeException($"Unknown method: {typeInfo.TypeName}");
        }

        try
        {
            return typeInfo.DecodeKind switch
            {
                S2DecodeKind.Array => _array(typeInfo.Parameters),
                S2DecodeKind.BitArray => _bitarray(typeInfo.Parameters),
                S2DecodeKind.Blob => _blob(typeInfo.Parameters),
                S2DecodeKind.Bool => _bool(typeInfo.Parameters),
                S2DecodeKind.Choice => _choice(typeInfo.Parameters),
                S2DecodeKind.FourCc => _fourcc(typeInfo.Parameters),
                S2DecodeKind.Int => _int(typeInfo.Parameters),
                S2DecodeKind.Null => _null(typeInfo.Parameters),
                S2DecodeKind.Optional => _optional(typeInfo.Parameters),
                S2DecodeKind.Real32 => _real32(typeInfo.Parameters),
                S2DecodeKind.Real64 => _real64(typeInfo.Parameters),
                S2DecodeKind.Struct => _struct(typeInfo.Parameters),
                _ => throw new DecodeException(nameof(BitPackedDecoder)),
            };
        }
        catch (TargetInvocationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new TargetInvocationException(ex);
        }
    }

    private long ReadInt(BoundsParameter bounds)
        => bounds.Min + _buffer.ReadBits((int)bounds.Max);

    private List<object?> _array(IDecodeParameter[] parameters)
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

    private byte[] _blob(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
        {
            var length = ReadInt(bounds);
            return _buffer.ReadAlignedBytes((int)length);
        }
        throw new ArgumentException("Invalid parameters for _blob");
    }

    private bool _bool(IDecodeParameter[] _) => _buffer.ReadBits(1) != 0;

    private Dictionary<string, object?> _choice(IDecodeParameter[] parameters)
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

    private byte[] _fourcc(IDecodeParameter[] _) => _buffer.ReadUnalignedBytes(4);

    private long _int(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
            return ReadInt(bounds);
        throw new ArgumentException("Invalid parameters for _int");
    }

#pragma warning disable CA1822 // Mark members as static
    private object? _null(IDecodeParameter[] _) => null;
#pragma warning restore CA1822 // Mark members as static

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
}
