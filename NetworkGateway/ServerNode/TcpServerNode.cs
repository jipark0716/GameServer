using System.Net.Sockets;

namespace NetworkGateway.ServerNode;

public class TcpServerNode(Socket socket, byte serverId) : IServerNode
{
    private readonly Socket _socket = socket;
    private readonly byte _serverId = serverId;
    public byte ServerId { get => _serverId; }
    public void Send(byte[] payload) => _socket.Send(payload);
}
