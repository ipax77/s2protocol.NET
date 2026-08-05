namespace s2protocol.NET;

/// <summary>Engine startup Exception</summary>
///
[Serializable]
public class DecodeException : Exception
{
    /// <summary>decode exception</summary>
    ///
    public DecodeException()
    {
    }
    /// <summary>decode exception</summary>
    ///
    public DecodeException(string message) : base(message)
    {
    }
    /// <summary>decode exception</summary>
    ///
    public DecodeException(string message, Exception innerExeption) : base(message, innerExeption)
    {
    }
}
