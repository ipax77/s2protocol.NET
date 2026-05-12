using System.Buffers.Binary;
using System.Numerics;
using System.Text;

namespace s2protocol.NET.S2Protocol;

internal interface IStructFieldReader
{
    bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId);
}

internal interface IArrayItemReader
{
    bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId);
}

internal interface IChoiceHandler
{
    bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId);
}

internal abstract class TypedProtocolDecoder(ReadOnlyMemory<byte> contents, List<S2TypeInfo> typeInfos)
{
    protected readonly BitPackedBuffer Buffer = new(contents);
    private readonly List<S2TypeInfo> _typeInfos = typeInfos;

    public bool Done() => Buffer.Done();
    public long UsedBits() => Buffer.UsedBits();
    public void ByteAlign() => Buffer.ByteAlign();

    public int ReadInt(int typeId)
    {
        long value = ReadLong(typeId);
        return value > int.MaxValue ? int.MaxValue : value < int.MinValue ? int.MinValue : (int)value;
    }

    public int? ReadNullableInt(int typeId)
    {
        if (GetKind(typeId) != S2DecodeKind.Optional)
        {
            return ReadInt(typeId);
        }

        return TryReadOptionalTypeId(typeId, out int innerTypeId)
            ? ReadInt(innerTypeId)
            : null;
    }

    public long ReadLong(int typeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        return typeInfo.DecodeKind switch
        {
            S2DecodeKind.Int => ReadIntValue((BoundsParameter)typeInfo.Parameters[0]),
            S2DecodeKind.Bool => ReadBoolValue() ? 1 : 0,
            S2DecodeKind.Choice => ReadChoiceLong(typeId),
            S2DecodeKind.Optional => ReadOptionalLong(typeId),
            S2DecodeKind.Struct => ReadFirstStructLong(typeId),
            S2DecodeKind.Null => 0,
            _ => throw new DecodeException($"Type {typeId} is not numeric."),
        };
    }

    public long? ReadNullableLong(int typeId)
    {
        if (GetKind(typeId) != S2DecodeKind.Optional)
        {
            return ReadLong(typeId);
        }

        return TryReadOptionalTypeId(typeId, out int innerTypeId)
            ? ReadLong(innerTypeId)
            : null;
    }

    public bool ReadBool(int typeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        return typeInfo.DecodeKind switch
        {
            S2DecodeKind.Bool => ReadBoolValue(),
            S2DecodeKind.Optional => ReadOptionalBool(typeId),
            _ => ReadLong(typeId) != 0,
        };
    }

    public string ReadString(int typeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        return typeInfo.DecodeKind switch
        {
            S2DecodeKind.Blob => Encoding.UTF8.GetString(ReadBlobValue((BoundsParameter)typeInfo.Parameters[0])),
            S2DecodeKind.FourCc => Encoding.UTF8.GetString(ReadFourCcValue()),
            S2DecodeKind.Optional => ReadOptionalString(typeId),
            S2DecodeKind.Choice => ReadChoiceString(typeId),
            S2DecodeKind.Struct => ReadFirstStructString(typeId),
            S2DecodeKind.Null => string.Empty,
            _ => ReadLong(typeId).ToString(System.Globalization.CultureInfo.InvariantCulture),
        };
    }

    public string? ReadNullableString(int typeId)
    {
        if (GetKind(typeId) != S2DecodeKind.Optional)
        {
            return ReadString(typeId);
        }

        return TryReadOptionalTypeId(typeId, out int innerTypeId)
            ? ReadString(innerTypeId)
            : null;
    }

    public KeyValuePair<int, BigInteger> ReadBitArray(int typeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.DecodeKind == S2DecodeKind.Optional)
        {
            return TryReadOptionalTypeId(typeId, out int innerTypeId)
                ? ReadBitArray(innerTypeId)
                : default;
        }

        if (typeInfo.DecodeKind != S2DecodeKind.BitArray)
        {
            SkipType(typeId);
            return default;
        }

        return ReadBitArrayValue((BoundsParameter)typeInfo.Parameters[0]);
    }

    public List<int> ReadIntList(int typeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.DecodeKind == S2DecodeKind.Optional)
        {
            return TryReadOptionalTypeId(typeId, out int innerTypeId)
                ? ReadIntList(innerTypeId)
                : [];
        }

        if (typeInfo.DecodeKind != S2DecodeKind.Array
            || typeInfo.Parameters is not [BoundsParameter bounds, TypeIdParameter itemType])
        {
            throw new DecodeException($"Type {typeId} is not an array.");
        }

        return ReadIntListItems(bounds, itemType.TypeId);
    }

    public List<string> ReadStringList(int typeId)
    {
        List<string> result = [];
        StringListItemReader reader = new(result);
        ReadArray(typeId, ref reader);
        return result;
    }

    public void ReadArray<THandler>(int typeId, ref THandler handler)
        where THandler : struct, IArrayItemReader
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.DecodeKind == S2DecodeKind.Optional)
        {
            if (TryReadOptionalTypeId(typeId, out int innerTypeId))
            {
                ReadArray(innerTypeId, ref handler);
            }

            return;
        }

        if (typeInfo.DecodeKind != S2DecodeKind.Array
            || typeInfo.Parameters is not [BoundsParameter bounds, TypeIdParameter itemType])
        {
            throw new DecodeException($"Type {typeId} is not an array.");
        }

        ReadArrayItems(bounds, itemType.TypeId, ref handler);
    }

    public void ReadStruct<TState>(int typeId, ref TState state)
        where TState : struct, IStructFieldReader
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.DecodeKind == S2DecodeKind.Optional)
        {
            if (TryReadOptionalTypeId(typeId, out int innerTypeId))
            {
                ReadStruct(innerTypeId, ref state);
            }

            return;
        }

        if (typeInfo.DecodeKind == S2DecodeKind.Choice)
        {
            if (typeInfo.Parameters is not [BoundsParameter bounds, ChoiceParameter choices])
            {
                throw new DecodeException($"Type {typeId} is not a choice.");
            }

            ReadChoiceStruct(bounds, choices, ref state);
            return;
        }

        if (typeInfo.DecodeKind != S2DecodeKind.Struct
            || typeInfo.Parameters is not [FieldListParameter fieldList])
        {
            SkipType(typeId);
            return;
        }

        ReadStructFields(fieldList, ref state);
    }

    public void ReadChoice<THandler>(int typeId, ref THandler handler)
        where THandler : struct, IChoiceHandler
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.DecodeKind != S2DecodeKind.Choice
            || typeInfo.Parameters is not [BoundsParameter bounds, ChoiceParameter choices])
        {
            throw new DecodeException($"Type {typeId} is not a choice.");
        }

        ReadChoiceValue(bounds, choices, ref handler);
    }

    public void SkipType(int typeId)
    {
        if (typeId < 0)
        {
            return;
        }

        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        switch (typeInfo.DecodeKind)
        {
            case S2DecodeKind.Array:
                if (typeInfo.Parameters is [BoundsParameter bounds, TypeIdParameter itemType])
                {
                    SkipArrayItemReader reader = default;
                    ReadArrayItems(bounds, itemType.TypeId, ref reader);
                }
                break;
            case S2DecodeKind.BitArray:
                ReadBitArrayValue((BoundsParameter)typeInfo.Parameters[0]);
                break;
            case S2DecodeKind.Blob:
                ReadBlobValue((BoundsParameter)typeInfo.Parameters[0]);
                break;
            case S2DecodeKind.Bool:
                ReadBoolValue();
                break;
            case S2DecodeKind.Choice:
                if (typeInfo.Parameters is [BoundsParameter choiceBounds, ChoiceParameter choices])
                {
                    SkipChoiceHandler handler = default;
                    ReadChoiceValue(choiceBounds, choices, ref handler);
                }
                break;
            case S2DecodeKind.FourCc:
                ReadFourCcValue();
                break;
            case S2DecodeKind.Int:
                ReadIntValue((BoundsParameter)typeInfo.Parameters[0]);
                break;
            case S2DecodeKind.Null:
                break;
            case S2DecodeKind.Optional:
                if (TryReadOptionalTypeId(typeId, out int innerTypeId))
                {
                    SkipType(innerTypeId);
                }
                break;
            case S2DecodeKind.Real32:
                ReadReal32Value();
                break;
            case S2DecodeKind.Real64:
                ReadReal64Value();
                break;
            case S2DecodeKind.Struct:
                if (typeInfo.Parameters is [FieldListParameter fieldList])
                {
                    SkipStructFieldReader reader = default;
                    ReadStructFields(fieldList, ref reader);
                }
                break;
            default:
                throw new DecodeException($"Unknown type {typeId}.");
        }
    }

    protected S2DecodeKind GetKind(int typeId) => GetTypeInfo(typeId).DecodeKind;

    protected S2TypeInfo GetTypeInfo(int typeId)
    {
        if (typeId < 0 || typeId >= _typeInfos.Count)
        {
            throw new DecodeException($"Invalid type id {typeId}.");
        }

        return _typeInfos[typeId];
    }

    protected abstract long ReadIntValue(BoundsParameter bounds);
    protected abstract bool ReadBoolValue();
    protected abstract ReadOnlySpan<byte> ReadBlobValue(BoundsParameter bounds);
    protected abstract ReadOnlySpan<byte> ReadFourCcValue();
    protected abstract KeyValuePair<int, BigInteger> ReadBitArrayValue(BoundsParameter bounds);
    protected abstract float ReadReal32Value();
    protected abstract double ReadReal64Value();
    protected abstract void ReadArrayItems<THandler>(BoundsParameter bounds, int itemTypeId, ref THandler handler)
        where THandler : struct, IArrayItemReader;
    protected abstract List<int> ReadIntListItems(BoundsParameter bounds, int itemTypeId);
    protected abstract void ReadStructFields<TState>(FieldListParameter fields, ref TState state)
        where TState : struct, IStructFieldReader;
    protected abstract void ReadChoiceValue<THandler>(BoundsParameter bounds, ChoiceParameter choices, ref THandler handler)
        where THandler : struct, IChoiceHandler;
    protected abstract void ReadChoiceStruct<TState>(BoundsParameter bounds, ChoiceParameter choices, ref TState state)
        where TState : struct, IStructFieldReader;
    protected abstract bool TryReadOptionalTypeId(int typeId, out int innerTypeId);

    private long ReadChoiceLong(int typeId)
    {
        LongChoiceHandler handler = default;
        ReadChoice(typeId, ref handler);
        return handler.Result;
    }

    private long ReadOptionalLong(int typeId)
        => TryReadOptionalTypeId(typeId, out int innerTypeId) ? ReadLong(innerTypeId) : 0;

    private long ReadFirstStructLong(int typeId)
    {
        FirstLongFieldReader reader = default;
        ReadStruct(typeId, ref reader);
        return reader.Result;
    }

    private bool ReadOptionalBool(int typeId)
        => TryReadOptionalTypeId(typeId, out int innerTypeId) && ReadBool(innerTypeId);

    private string ReadChoiceString(int typeId)
    {
        StringChoiceHandler handler = default;
        ReadChoice(typeId, ref handler);
        return handler.Result ?? string.Empty;
    }

    private string ReadOptionalString(int typeId)
        => TryReadOptionalTypeId(typeId, out int innerTypeId) ? ReadString(innerTypeId) : string.Empty;

    private string ReadFirstStructString(int typeId)
    {
        FirstStringFieldReader reader = default;
        ReadStruct(typeId, ref reader);
        return reader.Result ?? string.Empty;
    }

    private readonly struct StringListItemReader(List<string> result) : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            result.Add(decoder.ReadString(itemTypeId));
            return true;
        }
    }

    private struct LongChoiceHandler : IChoiceHandler
    {
        public long Result;

        public bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId)
        {
            Result = decoder.ReadLong(choiceTypeId);
            return true;
        }
    }

    private struct StringChoiceHandler : IChoiceHandler
    {
        public string? Result;

        public bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId)
        {
            Result = decoder.ReadString(choiceTypeId);
            return true;
        }
    }

    private struct FirstLongFieldReader : IStructFieldReader
    {
        private bool _found;
        public long Result;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (_found)
            {
                return false;
            }

            Result = decoder.ReadLong(fieldTypeId);
            _found = true;
            return true;
        }
    }

    private struct FirstStringFieldReader : IStructFieldReader
    {
        private bool _found;
        public string? Result;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            if (_found)
            {
                return false;
            }

            Result = decoder.ReadString(fieldTypeId);
            _found = true;
            return true;
        }
    }

    private struct SkipArrayItemReader : IArrayItemReader
    {
        public bool ReadItem(TypedProtocolDecoder decoder, int itemTypeId)
        {
            decoder.SkipType(itemTypeId);
            return true;
        }
    }

    private struct SkipChoiceHandler : IChoiceHandler
    {
        public bool HandleChoice(TypedProtocolDecoder decoder, string name, int choiceTypeId)
        {
            decoder.SkipType(choiceTypeId);
            return true;
        }
    }

    private struct SkipStructFieldReader : IStructFieldReader
    {
        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            decoder.SkipType(fieldTypeId);
            return true;
        }
    }

    protected static KeyValuePair<int, BigInteger> MakeBitArray(long length, ReadOnlySpan<byte> bytes)
    {
        Span<byte> littleEndian = stackalloc byte[bytes.Length + 1];
        bytes.CopyTo(littleEndian);
        littleEndian[..bytes.Length].Reverse();
        return new KeyValuePair<int, BigInteger>((int)length, new BigInteger(littleEndian));
    }
}

internal sealed class BitPackedTypedDecoder(ReadOnlyMemory<byte> contents, List<S2TypeInfo> typeInfos)
    : TypedProtocolDecoder(contents, typeInfos)
{
    protected override long ReadIntValue(BoundsParameter bounds)
        => bounds.Min + Buffer.ReadBits((int)bounds.Max);

    protected override bool ReadBoolValue() => Buffer.ReadBits(1) != 0;

    protected override ReadOnlySpan<byte> ReadBlobValue(BoundsParameter bounds)
    {
        long length = ReadIntValue(bounds);
        return Buffer.ReadAlignedSpan(length);
    }

    protected override ReadOnlySpan<byte> ReadFourCcValue() => Buffer.ReadUnalignedBytes(4);

    protected override KeyValuePair<int, BigInteger> ReadBitArrayValue(BoundsParameter bounds)
    {
        long length = ReadIntValue(bounds);
        long value = Buffer.ReadBits((int)length);
        return new KeyValuePair<int, BigInteger>((int)length, new BigInteger(value));
    }

    protected override float ReadReal32Value()
    {
        Span<byte> bytes = stackalloc byte[4];
        Buffer.ReadUnalignedBytes(4).CopyTo(bytes);
        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToSingle(bytes);
    }

    protected override double ReadReal64Value()
    {
        Span<byte> bytes = stackalloc byte[8];
        Buffer.ReadUnalignedBytes(8).CopyTo(bytes);
        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToDouble(bytes);
    }

    protected override void ReadArrayItems<THandler>(BoundsParameter bounds, int itemTypeId, ref THandler handler)
    {
        long length = ReadIntValue(bounds);
        for (long i = 0; i < length; i++)
        {
            if (!handler.ReadItem(this, itemTypeId))
            {
                SkipType(itemTypeId);
            }
        }
    }

    protected override List<int> ReadIntListItems(BoundsParameter bounds, int itemTypeId)
    {
        long length = ReadIntValue(bounds);
        List<int> result = new((int)Math.Min(length, int.MaxValue));
        for (long i = 0; i < length; i++)
        {
            result.Add(ReadInt(itemTypeId));
        }

        return result;
    }

    protected override void ReadStructFields<TState>(FieldListParameter fields, ref TState state)
    {
        foreach (var field in fields.Fields)
        {
            bool handled = field.Name == "__parent"
                ? ReadParentStruct(field.TypeId, ref state)
                : state.ReadField(this, field.Name, field.TypeId);

            if (!handled)
            {
                SkipType(field.TypeId);
            }
        }
    }

    protected override void ReadChoiceValue<THandler>(BoundsParameter bounds, ChoiceParameter choices, ref THandler handler)
    {
        long tag = ReadIntValue(bounds);
        if (!choices.Choices.TryGetValue(tag, out var choice))
        {
            throw new DecodeException(nameof(BitPackedTypedDecoder));
        }

        if (!handler.HandleChoice(this, choice.Name, choice.TypeId))
        {
            SkipType(choice.TypeId);
        }
    }

    protected override void ReadChoiceStruct<TState>(BoundsParameter bounds, ChoiceParameter choices, ref TState state)
    {
        long tag = ReadIntValue(bounds);
        if (!choices.Choices.TryGetValue(tag, out var choice))
        {
            throw new DecodeException(nameof(BitPackedTypedDecoder));
        }

        ReadStruct(choice.TypeId, ref state);
    }

    protected override bool TryReadOptionalTypeId(int typeId, out int innerTypeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.Parameters is not [TypeIdParameter innerType])
        {
            throw new DecodeException("Invalid optional type.");
        }

        innerTypeId = innerType.TypeId;
        return ReadBoolValue();
    }

    private bool ReadParentStruct<TState>(int typeId, ref TState state)
        where TState : struct, IStructFieldReader
    {
        ReadStruct(typeId, ref state);
        return true;
    }
}

internal sealed class VersionedTypedDecoder(ReadOnlyMemory<byte> contents, List<S2TypeInfo> typeInfos)
    : TypedProtocolDecoder(contents, typeInfos)
{
    protected override long ReadIntValue(BoundsParameter bounds)
    {
        ExpectSkip(9);
        return VInt();
    }

    protected override bool ReadBoolValue()
    {
        ExpectSkip(6);
        return Buffer.ReadBits(8) != 0;
    }

    protected override ReadOnlySpan<byte> ReadBlobValue(BoundsParameter bounds)
    {
        ExpectSkip(2);
        long length = VInt();
        return Buffer.ReadAlignedSpan(length);
    }

    protected override ReadOnlySpan<byte> ReadFourCcValue()
    {
        ExpectSkip(7);
        return Buffer.ReadAlignedSpan(4);
    }

    protected override KeyValuePair<int, BigInteger> ReadBitArrayValue(BoundsParameter bounds)
    {
        ExpectSkip(1);
        long length = VInt();
        long byteLength = (length + 7) / 8;
        return MakeBitArray(length, Buffer.ReadAlignedSpan(byteLength));
    }

    protected override float ReadReal32Value()
    {
        ExpectSkip(7);
        Span<byte> bytes = stackalloc byte[4];
        Buffer.ReadAlignedSpan(4).CopyTo(bytes);
        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToSingle(bytes);
    }

    protected override double ReadReal64Value()
    {
        ExpectSkip(8);
        Span<byte> bytes = stackalloc byte[8];
        Buffer.ReadAlignedSpan(8).CopyTo(bytes);
        if (BitConverter.IsLittleEndian)
        {
            bytes.Reverse();
        }

        return BitConverter.ToDouble(bytes);
    }

    protected override void ReadArrayItems<THandler>(BoundsParameter bounds, int itemTypeId, ref THandler handler)
    {
        ExpectSkip(0);
        long length = VInt();
        for (long i = 0; i < length; i++)
        {
            if (!handler.ReadItem(this, itemTypeId))
            {
                SkipType(itemTypeId);
            }
        }
    }

    protected override List<int> ReadIntListItems(BoundsParameter bounds, int itemTypeId)
    {
        ExpectSkip(0);
        long length = VInt();
        List<int> result = new((int)Math.Min(length, int.MaxValue));
        for (long i = 0; i < length; i++)
        {
            result.Add(ReadInt(itemTypeId));
        }

        return result;
    }

    protected override void ReadStructFields<TState>(FieldListParameter fields, ref TState state)
    {
        ExpectSkip(5);
        long length = VInt();

        for (long i = 0; i < length; i++)
        {
            long tag = VInt();
            if (fields.FieldsByTag.TryGetValue(tag, out var field))
            {
                bool handled = field.Name == "__parent"
                    ? ReadParentStruct(field.TypeId, ref state)
                    : state.ReadField(this, field.Name, field.TypeId);

                if (!handled)
                {
                    SkipType(field.TypeId);
                }
            }
            else
            {
                SkipInstance();
            }
        }
    }

    protected override void ReadChoiceValue<THandler>(BoundsParameter bounds, ChoiceParameter choices, ref THandler handler)
    {
        ExpectSkip(3);
        long tag = VInt();
        if (!choices.Choices.TryGetValue(tag, out var choice))
        {
            SkipInstance();
            return;
        }

        if (!handler.HandleChoice(this, choice.Name, choice.TypeId))
        {
            SkipType(choice.TypeId);
        }
    }

    protected override void ReadChoiceStruct<TState>(BoundsParameter bounds, ChoiceParameter choices, ref TState state)
    {
        ExpectSkip(3);
        long tag = VInt();
        if (!choices.Choices.TryGetValue(tag, out var choice))
        {
            SkipInstance();
            return;
        }

        ReadStruct(choice.TypeId, ref state);
    }

    protected override bool TryReadOptionalTypeId(int typeId, out int innerTypeId)
    {
        S2TypeInfo typeInfo = GetTypeInfo(typeId);
        if (typeInfo.Parameters is not [TypeIdParameter innerType])
        {
            throw new DecodeException("Invalid optional type.");
        }

        ExpectSkip(4);
        innerTypeId = innerType.TypeId;
        return Buffer.ReadBits(8) != 0;
    }

    private bool ReadParentStruct<TState>(int typeId, ref TState state)
        where TState : struct, IStructFieldReader
    {
        ReadStruct(typeId, ref state);
        return true;
    }

    private void ExpectSkip(int expected)
    {
        long actual = Buffer.ReadBits(8);
        if (actual != expected)
        {
            throw new DecodeException(nameof(VersionedTypedDecoder));
        }
    }

    private long VInt()
    {
        long b = Buffer.ReadBits(8);
        bool negative = (b & 1) != 0;
        long result = (b >> 1) & 0x3F;
        int bits = 6;

        while ((b & 0x80) != 0)
        {
            b = Buffer.ReadBits(8);
            result |= (b & 0x7F) << bits;
            bits += 7;
        }

        return negative ? -result : result;
    }

    private void SkipInstance()
    {
        long skip = Buffer.ReadBits(8);

        switch (skip)
        {
            case 0:
                {
                    long length = VInt();
                    for (long i = 0; i < length; i++)
                    {
                        SkipInstance();
                    }
                    break;
                }
            case 1:
                {
                    long length = VInt();
                    long byteLength = (length + 7) / 8;
                    Buffer.ReadAlignedSpan(byteLength);
                    break;
                }
            case 2:
                Buffer.ReadAlignedSpan(VInt());
                break;
            case 3:
                VInt();
                SkipInstance();
                break;
            case 4:
                if (Buffer.ReadBits(8) != 0)
                {
                    SkipInstance();
                }
                break;
            case 5:
                {
                    long length = VInt();
                    for (long i = 0; i < length; i++)
                    {
                        VInt();
                        SkipInstance();
                    }
                    break;
                }
            case 6:
                Buffer.ReadAlignedSpan(1);
                break;
            case 7:
                Buffer.ReadAlignedSpan(4);
                break;
            case 8:
                Buffer.ReadAlignedSpan(8);
                break;
            case 9:
                VInt();
                break;
            default:
                throw new DecodeException(nameof(VersionedTypedDecoder));
        }
    }
}
