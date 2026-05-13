using System.Reflection;

namespace s2protocol.NET.S2Protocol;

internal sealed class BitPackedDecoder(byte[] contents, List<S2TypeInfo> typeinfos) : S2ProtocolDecoder
{
    private readonly BitPackedBuffer _buffer = new(contents);
    private readonly List<S2TypeInfo> _typeInfos = typeinfos;

    public override string ToString()
    {
        return _buffer.ToString();
    }

    public override bool Done()
    {
        return _buffer.Done();
    }

    public override long UsedBits()
    {
        return _buffer.UsedBits();
    }

    public override void ByteAlign()
    {
        _buffer.ByteAlign();
    }

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
                S2DecodeKind.Array => BpArray(typeInfo.Parameters),
                S2DecodeKind.BitArray => BpBitArray(typeInfo.Parameters),
                S2DecodeKind.Blob => BpBlob(typeInfo.Parameters),
                S2DecodeKind.Bool => BpBool(typeInfo.Parameters),
                S2DecodeKind.Choice => BpChoice(typeInfo.Parameters),
                S2DecodeKind.FourCc => BpFourcc(typeInfo.Parameters),
                S2DecodeKind.Int => BpInt(typeInfo.Parameters),
                S2DecodeKind.Null => BpNull(typeInfo.Parameters),
                S2DecodeKind.Optional => BpOptional(typeInfo.Parameters),
                S2DecodeKind.Real32 => BpReal32(typeInfo.Parameters),
                S2DecodeKind.Real64 => BpReal64(typeInfo.Parameters),
                S2DecodeKind.Struct => BpStruct(typeInfo.Parameters),
                S2DecodeKind.Unknown => throw new NotImplementedException(),
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
    {
        return bounds.Min + _buffer.ReadBits((int)bounds.Max);
    }

    private List<object?> BpArray(IDecodeParameter[] parameters)
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

    private object? BpBitArray(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
        {
            var length = ReadInt(bounds);
            return (length, _buffer.ReadBits((int)length));
        }
        throw new ArgumentException("Invalid parameters for _bitarray");
    }

    private byte[] BpBlob(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds])
        {
            var length = ReadInt(bounds);
            return _buffer.ReadAlignedBytes((int)length);
        }
        throw new ArgumentException("Invalid parameters for _blob");
    }

    private bool BpBool(IDecodeParameter[] _)
    {
        return _buffer.ReadBits(1) != 0;
    }

    private Dictionary<string, object?> BpChoice(IDecodeParameter[] parameters)
    {
        if (parameters is [BoundsParameter bounds, ChoiceParameter choices])
        {
            var tag = ReadInt(bounds);
            return !choices.Choices.TryGetValue((int)tag, out var choice)
                ? throw new DecodeException(nameof(BitPackedDecoder))
                : new Dictionary<string, object?>
                {
                    [choice.Name] = Instance(choice.TypeId)
                };
        }
        throw new ArgumentException("Invalid parameters for _choice");
    }

    private byte[] BpFourcc(IDecodeParameter[] _)
    {
        return _buffer.ReadUnalignedBytes(4);
    }

    private long BpInt(IDecodeParameter[] parameters)
    {
        return parameters is [BoundsParameter bounds] ? ReadInt(bounds) : throw new ArgumentException("Invalid parameters for _int");
    }

#pragma warning disable CA1822 // Mark members as static
    private object? BpNull(IDecodeParameter[] _)
    {
        return null;
    }
#pragma warning restore CA1822 // Mark members as static

    private object? BpOptional(IDecodeParameter[] parameters)
    {
        return parameters is [TypeIdParameter type] ? BpBool([]) ? Instance(type.TypeId) : null : null;
    }

    private float BpReal32(IDecodeParameter[] _)
    {
        var bytes = _buffer.ReadUnalignedBytes(4);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    private double BpReal64(IDecodeParameter[] _)
    {
        var bytes = _buffer.ReadUnalignedBytes(8);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    private object? BpStruct(IDecodeParameter[] parameters)
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
