using System.Net.Sockets;

namespace Network.Packets;

public class ClientMessage(Author author, MessageType type, byte[]? payload = null)
{
    public readonly Author Author = author;
    public readonly byte[]? Payload = payload;
    public readonly MessageType Type = type;
}

public class Author(ulong connectionId, Socket socket)
{
    public readonly ulong ConnectionId = connectionId;
    public ulong? UserId;
    public readonly Socket Socket = socket;
}

public enum MessageType : byte
{
    Connect = 0,
    Disconnect = 1,
    Message = 2,
}