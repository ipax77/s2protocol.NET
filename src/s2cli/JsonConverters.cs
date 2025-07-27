using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace s2cli;

internal sealed class Utf8ByteArrayConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Not needed for your case
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        // Interpret as UTF-8 string and escape non-printable characters
        var str = Encoding.UTF8.GetString(value);

        // Escape using JSON-compatible method
        var escaped = JsonEncodedText.Encode(str);
        writer.WriteStringValue(escaped);
    }
}