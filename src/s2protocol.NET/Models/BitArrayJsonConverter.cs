using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace s2protocol.NET.Models;

internal sealed class BitArrayJsonConverter : JsonConverter<KeyValuePair<int, BigInteger>>
{
    public override KeyValuePair<int, BigInteger> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        JsonElement root = document.RootElement;
        int length = root.GetProperty("Length").GetInt32();
        BigInteger value = BigInteger.Parse(
            root.GetProperty("Value").GetRawText(),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture);
        return new KeyValuePair<int, BigInteger>(length, value);
    }

    public override void Write(
        Utf8JsonWriter writer,
        KeyValuePair<int, BigInteger> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteNumber("Length", value.Key);
        writer.WritePropertyName("Value");
        writer.WriteRawValue(value.Value.ToString(CultureInfo.InvariantCulture));
        writer.WriteEndObject();
    }
}
