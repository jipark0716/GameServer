using NetworkGateway.Connection;
using System.Net.Sockets;

namespace NetworkGateway.Tcp;

public class TcpConnection(ulong connectionId, Socket socket) : BaseConnection(connectionId)
{
    public readonly Socket Socket = socket;

    public override void Close()
    {
        base.Close();
        Socket.Dispose();
    }

    public override void Send(byte[] payload)
    {
        Socket.Send(payload);
    }

}
