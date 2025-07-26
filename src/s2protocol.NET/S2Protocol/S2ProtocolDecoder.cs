namespace s2protocol.NET.S2Protocol;

using System.Collections.Generic;

internal abstract class S2ProtocolDecoder
{
    /// <summary>
    /// Returns an instance of a type based on its type ID.
    /// </summary>
    public abstract object? Instance(int typeId);

    /// <summary>
    /// Returns whether the decoder has finished processing the buffer.
    /// </summary>
    public abstract bool Done();

    /// <summary>
    /// Returns the number of bits used so far in the stream.
    /// </summary>
    public abstract long UsedBits();

    /// <summary>
    /// Aligns the decoder to the next byte boundary.
    /// </summary>
    public abstract void ByteAlign();
    public Dictionary<string, object?> InstanceDict(int typeId)
    {
        if (typeId == -1)
        {
            return [];
        }
        var obj = Instance(typeId);
        if (obj is Dictionary<string, object?> dict)
            return dict;

        throw new DecodeException($"Type {typeId} did not decode to a dictionary.");
    }

    public static int Varuint32Value(object? value)
    {
        if (value is Dictionary<string, object?> dict)
        {
            foreach (var val in dict.Values)
            {
                if (val is int i) return i;
                if (val is long l) return (int)l;
            }
        }
        else if (value is int i)
        {
            return i;
        }
        else if (value is long l)
        {
            return (int)l;
        }

        return 0;
    }

    public abstract IDecodeParameter[] PrepareParameters(S2TypeInfo typeInfo);
}
