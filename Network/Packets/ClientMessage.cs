using System.Net.Sockets;
using Network.Sockets;

namespace Network.Packets;

public class ClientMessage(ulong connectionId, Socket socket, MessageType type, byte[]? payload = null)
{
    public readonly ulong ConnectionId = connectionId;
    public readonly Socket Socket = socket;
    public readonly byte[]? Payload = payload;
    public readonly MessageType Type = type;
}

public enum MessageType : byte
{
    Connect = 0,
    Disconnect = 1,
    Message = 2,
}