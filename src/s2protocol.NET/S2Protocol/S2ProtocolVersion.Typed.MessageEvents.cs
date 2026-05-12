using s2protocol.NET.Models;

namespace s2protocol.NET.S2Protocol;

public sealed partial record S2ProtocolVersion
{
    private static ChatMessageEvent ReadSChatMessage(TypedProtocolDecoder decoder, int typeId, int userId, int gameloop)
    {
        SChatMessageReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new ChatMessageEvent(state.Recipient, userId, state.Message, gameloop);
    }

    private struct SChatMessageReadState : IStructFieldReader
    {
        public int Recipient;
        public string Message;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_recipient":
                    Recipient = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_string":
                    Message = decoder.ReadString(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }

    private static PingMessageEvent ReadSPingMessage(TypedProtocolDecoder decoder, int typeId, int userId, int gameloop)
    {
        SPingMessageReadState state = default;
        decoder.ReadStruct(typeId, ref state);
        return new PingMessageEvent(state.Recipient, userId, gameloop, state.X, state.Y);
    }

    private struct SPingMessageReadState : IStructFieldReader
    {
        public int Recipient;
        public long X;
        public long Y;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "m_recipient":
                    Recipient = decoder.ReadInt(fieldTypeId);
                    return true;
                case "m_point":
                    SPingPointReadState point = default;
                    decoder.ReadStruct(fieldTypeId, ref point);
                    X = point.X;
                    Y = point.Y;
                    return true;
                default:
                    return false;
            }
        }
    }

    private struct SPingPointReadState : IStructFieldReader
    {
        public long X;
        public long Y;

        public bool ReadField(TypedProtocolDecoder decoder, string name, int fieldTypeId)
        {
            switch (name)
            {
                case "x":
                    X = decoder.ReadLong(fieldTypeId);
                    return true;
                case "y":
                    Y = decoder.ReadLong(fieldTypeId);
                    return true;
                default:
                    return false;
            }
        }
    }
}
