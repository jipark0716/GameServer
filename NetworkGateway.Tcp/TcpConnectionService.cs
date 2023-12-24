using NetworkGateway.Connection;
using System.Net;
using System.Net.Sockets;

namespace NetworkGateway.Tcp;

public class TcpConnectionService : AConnectionService
{
    private readonly TcpConnectionFactory _tcpConnectionFactory;
    private readonly Socket _listener;

    public TcpConnectionService(TcpConnectionFactory tcpConnectionFactory)
    {
        _tcpConnectionFactory = tcpConnectionFactory;
        _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 1654));
    }


    public void Start()
    {
        _listener.Listen();
        Task.Run(() =>
        {
            while (true)
            {
                Socket handler = _listener.Accept();
                Task.Run(() =>
                {
                    var connection = _tcpConnectionFactory.Create(handler);
                    AddConnection(connection);
                    ReceiveMessage(connection);
                    handler.Disconnect(false);
                });
            }
        });
    }

    private void ReceiveMessage(TcpConnection connection)
    {
        var buffer = new byte[1024 * 4];
        int payloadSize;

        while (true)
        {
            try
            {
                payloadSize = connection.Socket.Receive(buffer);
            }
            catch (Exception)
            {
                break; ;
            }
            connection.OnMessage(buffer[..payloadSize]);
        };

        connection.Close();
    }
}
