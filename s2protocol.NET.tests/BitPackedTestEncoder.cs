using s2protocol.NET.S2Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s2protocol.NET.tests;

internal sealed record TestChoiceValue(string Name, object? Value = null);

internal static class BitPackedTestEncoder
{
    public static byte[] EncodeGameEvent(
        S2ProtocolVersion protocol,
        int eventId,
        IReadOnlyDictionary<string, object?>? values = null)
    {
        if (!protocol.GameEvents.TryGetValue(eventId, out S2EventType? eventType))
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        BitWriter writer = new();
        WriteType(writer, protocol, protocol.SVarUint32TypeId ?? 7, 0, string.Empty, null);
        WriteType(writer, protocol, protocol.ReplayUserIdTypeId ?? 8, 0, string.Empty, null);
        WriteType(writer, protocol, protocol.GameEventIdTypeId ?? 0, eventId, string.Empty, null);
        WriteType(writer, protocol, eventType.TypeId, null, string.Empty, values);
        writer.ByteAlign();
        return writer.ToArray();
    }

    private static void WriteType(
        BitWriter writer,
        S2ProtocolVersion protocol,
        int typeId,
        object? explicitValue,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        S2TypeInfo typeInfo = protocol.TypeInfos[typeId];
        object? value = explicitValue ?? FindValue(values, path);

        switch (typeInfo.DecodeKind)
        {
            case S2DecodeKind.Array:
                WriteArray(writer, protocol, typeInfo, value, path, values);
                break;
            case S2DecodeKind.BitArray:
                WriteLength(writer, (BoundsParameter)typeInfo.Parameters[0], 0);
                break;
            case S2DecodeKind.Blob:
                WriteBlob(writer, (BoundsParameter)typeInfo.Parameters[0], value as string ?? string.Empty);
                break;
            case S2DecodeKind.Bool:
                writer.WriteBits(value is true ? 1 : 0, 1);
                break;
            case S2DecodeKind.Choice:
                WriteChoice(writer, protocol, typeInfo, value, path, values);
                break;
            case S2DecodeKind.FourCc:
                WriteFourCc(writer, value as string ?? string.Empty);
                break;
            case S2DecodeKind.Int:
                WriteInteger(writer, (BoundsParameter)typeInfo.Parameters[0], Convert.ToInt64(
                    value ?? ((BoundsParameter)typeInfo.Parameters[0]).Min,
                    System.Globalization.CultureInfo.InvariantCulture));
                break;
            case S2DecodeKind.Null:
                break;
            case S2DecodeKind.Optional:
                WriteOptional(writer, protocol, typeInfo, value, path, values);
                break;
            case S2DecodeKind.Real32:
                WriteBytesUnaligned(writer, BitConverter.GetBytes(0F).Reverse());
                break;
            case S2DecodeKind.Real64:
                WriteBytesUnaligned(writer, BitConverter.GetBytes(0D).Reverse());
                break;
            case S2DecodeKind.Struct:
                WriteStruct(writer, protocol, typeInfo, path, values);
                break;
            default:
                throw new InvalidOperationException($"Unsupported test type {typeInfo.TypeName}.");
        }
    }

    private static void WriteArray(
        BitWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        object? value,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        BoundsParameter bounds = (BoundsParameter)typeInfo.Parameters[0];
        int itemTypeId = ((TypeIdParameter)typeInfo.Parameters[1]).TypeId;
        object?[] items = value is IEnumerable<object?> sequence ? sequence.ToArray() : [];
        WriteLength(writer, bounds, items.Length);
        for (int index = 0; index < items.Length; index++)
        {
            WriteType(writer, protocol, itemTypeId, items[index], $"{path}[{index}]", values);
        }
    }

    private static void WriteBlob(BitWriter writer, BoundsParameter bounds, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        WriteLength(writer, bounds, bytes.Length);
        writer.ByteAlign();
        foreach (byte item in bytes)
        {
            writer.WriteAlignedByte(item);
        }
    }

    private static void WriteChoice(
        BitWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        object? value,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        BoundsParameter bounds = (BoundsParameter)typeInfo.Parameters[0];
        ChoiceParameter choices = (ChoiceParameter)typeInfo.Parameters[1];
        TestChoiceValue? requested = value as TestChoiceValue;
        KeyValuePair<long, DecodeChoice> selected = requested is null
            ? choices.Choices.MinBy(choice => choice.Key)
            : choices.Choices.First(choice => string.Equals(choice.Value.Name, requested.Name, StringComparison.Ordinal));
        WriteInteger(writer, bounds, selected.Key);
        WriteType(writer, protocol, selected.Value.TypeId, requested?.Value, path, values);
    }

    private static void WriteFourCc(BitWriter writer, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value.PadRight(4, '\0')[..4]);
        WriteBytesUnaligned(writer, bytes);
    }

    private static void WriteOptional(
        BitWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        object? value,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        bool present = value is not null || HasValue(values, path);
        writer.WriteBits(present ? 1 : 0, 1);
        if (present)
        {
            WriteType(writer, protocol, ((TypeIdParameter)typeInfo.Parameters[0]).TypeId, value, path, values);
        }
    }

    private static void WriteStruct(
        BitWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        FieldListParameter fields = (FieldListParameter)typeInfo.Parameters[0];
        foreach (DecodeField field in fields.Fields)
        {
            string fieldPath = field.Name == "__parent" ? path : Append(path, field.Name);
            WriteType(writer, protocol, field.TypeId, null, fieldPath, values);
        }
    }

    private static void WriteLength(BitWriter writer, BoundsParameter bounds, long length)
        => WriteInteger(writer, bounds, length);

    private static void WriteInteger(BitWriter writer, BoundsParameter bounds, long value)
    {
        long encoded = value - bounds.Min;
        if (encoded < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), $"{value} is below {bounds.Min}.");
        }

        writer.WriteBits(encoded, checked((int)bounds.Max));
    }

    private static void WriteBytesUnaligned(BitWriter writer, IEnumerable<byte> bytes)
    {
        foreach (byte item in bytes)
        {
            writer.WriteBits(item, 8);
        }
    }

    private static object? FindValue(IReadOnlyDictionary<string, object?>? values, string path)
        => values is not null && values.TryGetValue(path, out object? value) ? value : null;

    private static bool HasValue(IReadOnlyDictionary<string, object?>? values, string path)
        => values is not null && (values.ContainsKey(path)
            || values.Keys.Any(key => key.StartsWith(path + ".", StringComparison.Ordinal)
                || key.StartsWith(path + "[", StringComparison.Ordinal)));

    private static string Append(string path, string field)
        => path.Length == 0 ? field : $"{path}.{field}";

    private sealed class BitWriter
    {
        private readonly List<byte> _bytes = [];
        private int _bitOffset;

        public void WriteBits(long value, int count)
        {
            int remaining = count;
            while (remaining > 0)
            {
                if (_bitOffset == 0)
                {
                    _bytes.Add(0);
                }

                int copyCount = Math.Min(remaining, 8 - _bitOffset);
                int shift = remaining - copyCount;
                long mask = (1L << copyCount) - 1;
                byte chunk = (byte)((value >> shift) & mask);
                int last = _bytes.Count - 1;
                _bytes[last] |= (byte)(chunk << _bitOffset);
                _bitOffset = (_bitOffset + copyCount) & 7;
                remaining -= copyCount;
            }
        }

        public void ByteAlign() => _bitOffset = 0;

        public void WriteAlignedByte(byte value)
        {
            if (_bitOffset != 0)
            {
                throw new InvalidOperationException("Writer is not byte aligned.");
            }

            _bytes.Add(value);
        }

        public byte[] ToArray() => [.. _bytes];
    }
}
