using NetworkGateway.Connection;
using System.Net.Sockets;

namespace NetworkGateway.Tcp;

public class TcpConnectionFactory : AConnectionFactory
{
    public TcpConnection Create(Socket socket) => new(ConnectionIdSequence, socket);
}