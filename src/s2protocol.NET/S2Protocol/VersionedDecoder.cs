using System.Reflection;

namespace s2protocol.NET.S2Protocol;

internal sealed class VersionedDecoder(byte[] contents, List<S2TypeInfo> typeinfos) : S2ProtocolDecoder
{
    private readonly BitPackedBuffer _buffer = new(contents);
    private readonly List<S2TypeInfo> _typeInfos = typeinfos;

    public override string ToString()
    {
        return _buffer.ToString();
    }

    public override object? Instance(int typeid)
    {
        if (typeid >= _typeInfos.Count)
            throw new DecodeException(nameof(VersionedDecoder));

        var typeInfo = _typeInfos[typeid];
        if (typeInfo.DecodeKind == S2DecodeKind.Unknown)
        {
            throw new DecodeException($"Unknown method: {typeInfo.TypeName}");
        }

        try
        {
            return typeInfo.DecodeKind switch
            {
                S2DecodeKind.Array => Vdarray(typeInfo.Parameters),
                S2DecodeKind.BitArray => Vdbitarray(),
                S2DecodeKind.Blob => Vdblob(),
                S2DecodeKind.Bool => Vdbool(),
                S2DecodeKind.Choice => Vdchoice(typeInfo.Parameters),
                S2DecodeKind.FourCc => Vdfourcc(),
                S2DecodeKind.Int => Vdint(),
                S2DecodeKind.Null => Vdnull(),
                S2DecodeKind.Optional => Vdoptional(typeInfo.Parameters),
                S2DecodeKind.Real32 => Vdreal32(),
                S2DecodeKind.Real64 => Vdreal64(),
                S2DecodeKind.Struct => Vdstruct(typeInfo.Parameters),
                S2DecodeKind.Unknown => throw new NotImplementedException(),
                _ => throw new DecodeException(nameof(VersionedDecoder)),
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

    private void Vdexpect_skip(int expected)
    {
        var bits = _buffer.ReadBits(8);
        if (bits != expected)
            throw new DecodeException(nameof(VersionedDecoder));
    }

    private long Vdvint()
    {
        var b = _buffer.ReadBits(8);
        bool negative = (b & 1) != 0;
        long result = (b >> 1) & 0x3F;
        int bits = 6;

        while ((b & 0x80) != 0)
        {
            b = _buffer.ReadBits(8);
            result |= (b & 0x7F) << bits;
            bits += 7;
        }

        return negative ? -result : result;
    }

    private List<object?> Vdarray(IDecodeParameter[] decodeParameters)
    {
        var list = new List<object?>();
        if (decodeParameters.Length == 2
            && decodeParameters[0] is BoundsParameter
            && decodeParameters[1] is TypeIdParameter typeParam
        )
        {
            Vdexpect_skip(0);
            long length = Vdvint();
            for (long i = 0; i < length; i++)
            {
                list.Add(Instance(typeParam.TypeId));
            }
        }
        return list;
    }

    private (long, byte[]) Vdbitarray()
    {
        Vdexpect_skip(1);
        long length = Vdvint();
        long byteLength = (length + 7) / 8;
        return (length, _buffer.ReadAlignedBytes(byteLength));
    }

    private byte[] Vdblob()
    {
        Vdexpect_skip(2);
        long length = Vdvint();
        return _buffer.ReadAlignedBytes(length);
    }

    private bool Vdbool()
    {
        Vdexpect_skip(6);
        return _buffer.ReadBits(8) != 0;
    }

    private Dictionary<string, object?> Vdchoice(IDecodeParameter[] decodeParameters)
    {
        //BoundsParameter bounds, Dictionary<long, DecodeChoice> fields
        if (decodeParameters.Length == 2
            && decodeParameters[0] is BoundsParameter
            && decodeParameters[1] is ChoiceParameter choiceParam)
        {

            Vdexpect_skip(3);
            long tag = Vdvint();
            if (!choiceParam.Choices.TryGetValue(tag, out DecodeChoice? field))
            {
                Vdskip_instance();
                return [];
            }

            return new Dictionary<string, object?> { { field.Name, Instance(field.TypeId) } };
        }
        return [];
    }

    private byte[] Vdfourcc()
    {
        Vdexpect_skip(7);
        return _buffer.ReadAlignedBytes(4);
    }

    private long Vdint()
    {
        Vdexpect_skip(9);
        return Vdvint();
    }

    private static object? Vdnull()
    {
        return null;
    }

    private object? Vdoptional(IDecodeParameter[] decodeParameters)
    {
        Vdexpect_skip(4);
        bool exists = _buffer.ReadBits(8) != 0;
        return decodeParameters.Length == 1
            && decodeParameters[0] is TypeIdParameter typeParam
            ? exists ? Instance(typeParam.TypeId) : null
            : throw new NotSupportedException("Optional without TypeIdParameter is not supported.");
    }

    private float Vdreal32()
    {
        Vdexpect_skip(7);
        var bytes = _buffer.ReadAlignedBytes(4);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToSingle(bytes, 0);
    }

    private double Vdreal64()
    {
        Vdexpect_skip(8);
        var bytes = _buffer.ReadAlignedBytes(8);
        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
        return BitConverter.ToDouble(bytes, 0);
    }

    private object? Vdstruct(IDecodeParameter[] decodeParameters)
    {
        if (decodeParameters.Length == 1 && decodeParameters[0] is FieldListParameter fieldListParam)
        {
            Vdexpect_skip(5);
            var result = new Dictionary<string, object?>();
            long length = Vdvint();

            for (long i = 0; i < length; i++)
            {
                long tag = Vdvint();
                if (fieldListParam.FieldsByTag.TryGetValue(tag, out var field))
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
                    Vdskip_instance();
                }
            }

            return result;
        }
        return null;
    }

    private void Vdskip_instance()
    {
        var skip = _buffer.ReadBits(8);

        switch (skip)
        {
            case 0: // array
                {
                    long length = Vdvint();
                    for (long i = 0; i < length; i++)
                        Vdskip_instance();
                    break;
                }
            case 1: // bitblob
                {
                    long length = Vdvint();
                    long byteLength = (length + 7) / 8;
                    _ = _buffer.ReadAlignedBytes(byteLength);
                    break;
                }
            case 2: // blob
                {
                    long length = Vdvint();
                    _ = _buffer.ReadAlignedBytes(length);
                    break;
                }
            case 3: // choice
                {
                    _ = Vdvint(); // tag
                    Vdskip_instance();
                    break;
                }
            case 4: // optional
                {
                    bool exists = _buffer.ReadBits(8) != 0;
                    if (exists) Vdskip_instance();
                    break;
                }
            case 5: // struct
                {
                    long length = Vdvint();
                    for (long i = 0; i < length; i++)
                    {
                        _ = Vdvint(); // tag
                        Vdskip_instance();
                    }
                    break;
                }
            case 6: // u8
                _ = _buffer.ReadAlignedBytes(1);
                break;
            case 7: // u32
                _ = _buffer.ReadAlignedBytes(4);
                break;
            case 8: // u64
                _ = _buffer.ReadAlignedBytes(8);
                break;
            case 9: // vint
                _ = Vdvint();
                break;
            default:
                throw new DecodeException(nameof(VersionedDecoder));
        }
    }
}
