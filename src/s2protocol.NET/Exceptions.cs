﻿namespace s2protocol.NET;

/// <summary>Engine startup Exception</summary>
///
[Serializable]
public class EngineException : Exception
{
    /// <summary>Engine startup Exception</summary>
    ///
    public EngineException()
    {
    }
    /// <summary>Engine startup Exception</summary>
    ///
    public EngineException(string message) : base(message)
    {
    }
    /// <summary>Engine startup Exception</summary>
    ///
    public EngineException(string message, Exception innerExeption) : base(message, innerExeption)
    {
    }
}

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