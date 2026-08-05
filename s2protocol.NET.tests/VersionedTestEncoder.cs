using s2protocol.NET.S2Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s2protocol.NET.tests;

internal static class VersionedTestEncoder
{
    public static byte[] EncodeTrackerEvent(
        S2ProtocolVersion protocol,
        int eventId,
        IReadOnlyDictionary<string, object?>? values = null)
    {
        if (!protocol.TrackerEvents.TryGetValue(eventId, out S2EventType? eventType))
        {
            throw new ArgumentOutOfRangeException(nameof(eventId));
        }

        VersionedWriter writer = new();
        WriteType(writer, protocol, protocol.SVarUint32TypeId ?? 7, 0, string.Empty, null);
        WriteType(writer, protocol, protocol.TrackerEventIdTypeId ?? 2, eventId, string.Empty, null);
        WriteType(writer, protocol, eventType.TypeId, null, string.Empty, values);
        return writer.ToArray();
    }

    private static void WriteType(
        VersionedWriter writer,
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
                writer.WriteByte(1);
                writer.WriteVInt(0);
                break;
            case S2DecodeKind.Blob:
                WriteBlob(writer, value as string ?? string.Empty);
                break;
            case S2DecodeKind.Bool:
                writer.WriteByte(6);
                writer.WriteByte(value is true ? (byte)1 : (byte)0);
                break;
            case S2DecodeKind.Choice:
                WriteChoice(writer, protocol, typeInfo, value, path, values);
                break;
            case S2DecodeKind.FourCc:
                WriteFourCc(writer, value as string ?? string.Empty);
                break;
            case S2DecodeKind.Int:
                writer.WriteByte(9);
                writer.WriteVInt(Convert.ToInt64(value ?? 0, System.Globalization.CultureInfo.InvariantCulture));
                break;
            case S2DecodeKind.Null:
                break;
            case S2DecodeKind.Optional:
                WriteOptional(writer, protocol, typeInfo, value, path, values);
                break;
            case S2DecodeKind.Real32:
                writer.WriteByte(7);
                writer.WriteBytes(BitConverter.GetBytes(0F).Reverse());
                break;
            case S2DecodeKind.Real64:
                writer.WriteByte(8);
                writer.WriteBytes(BitConverter.GetBytes(0D).Reverse());
                break;
            case S2DecodeKind.Struct:
                WriteStruct(writer, protocol, typeInfo, path, values);
                break;
            default:
                throw new InvalidOperationException($"Unsupported test type {typeInfo.TypeName}.");
        }
    }

    private static void WriteArray(
        VersionedWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        object? value,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        int itemTypeId = ((TypeIdParameter)typeInfo.Parameters[1]).TypeId;
        object?[] items = value is IEnumerable<object?> sequence ? sequence.ToArray() : [];
        writer.WriteByte(0);
        writer.WriteVInt(items.Length);
        for (int index = 0; index < items.Length; index++)
        {
            WriteType(writer, protocol, itemTypeId, items[index], $"{path}[{index}]", values);
        }
    }

    private static void WriteBlob(VersionedWriter writer, string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        writer.WriteByte(2);
        writer.WriteVInt(bytes.Length);
        writer.WriteBytes(bytes);
    }

    private static void WriteChoice(
        VersionedWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        object? value,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        ChoiceParameter choices = (ChoiceParameter)typeInfo.Parameters[1];
        TestChoiceValue? requested = value as TestChoiceValue;
        KeyValuePair<long, DecodeChoice> selected = requested is null
            ? choices.Choices.MinBy(choice => choice.Key)
            : choices.Choices.First(choice => string.Equals(choice.Value.Name, requested.Name, StringComparison.Ordinal));

        writer.WriteByte(3);
        writer.WriteVInt(selected.Key);
        WriteType(writer, protocol, selected.Value.TypeId, requested?.Value, path, values);
    }

    private static void WriteFourCc(VersionedWriter writer, string value)
    {
        writer.WriteByte(7);
        writer.WriteBytes(Encoding.UTF8.GetBytes(value.PadRight(4, '\0')[..4]));
    }

    private static void WriteOptional(
        VersionedWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        object? value,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        bool present = value is not null || HasValue(values, path);
        writer.WriteByte(4);
        writer.WriteByte(present ? (byte)1 : (byte)0);
        if (present)
        {
            WriteType(writer, protocol, ((TypeIdParameter)typeInfo.Parameters[0]).TypeId, value, path, values);
        }
    }

    private static void WriteStruct(
        VersionedWriter writer,
        S2ProtocolVersion protocol,
        S2TypeInfo typeInfo,
        string path,
        IReadOnlyDictionary<string, object?>? values)
    {
        FieldListParameter fields = (FieldListParameter)typeInfo.Parameters[0];
        writer.WriteByte(5);
        writer.WriteVInt(fields.Fields.Count);
        foreach (DecodeField field in fields.Fields)
        {
            writer.WriteVInt(field.Tag);
            string fieldPath = field.Name == "__parent" ? path : Append(path, field.Name);
            WriteType(writer, protocol, field.TypeId, null, fieldPath, values);
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

    private sealed class VersionedWriter
    {
        private readonly List<byte> bytes = [];

        public void WriteByte(byte value) => bytes.Add(value);

        public void WriteBytes(IEnumerable<byte> values) => bytes.AddRange(values);

        public void WriteVInt(long value)
        {
            bool negative = value < 0;
            ulong remaining = negative ? (ulong)(-(value + 1)) + 1 : (ulong)value;
            byte first = (byte)((remaining & 0x3F) << 1);
            if (negative)
            {
                first |= 1;
            }

            remaining >>= 6;
            if (remaining != 0)
            {
                first |= 0x80;
            }

            bytes.Add(first);
            while (remaining != 0)
            {
                byte next = (byte)(remaining & 0x7F);
                remaining >>= 7;
                if (remaining != 0)
                {
                    next |= 0x80;
                }

                bytes.Add(next);
            }
        }

        public byte[] ToArray() => [.. bytes];
    }
}
