using Common.Queues;
using NetworkGateway.Connection;
using NetworkGateway.Packets;
using NetworkGateway.ServerNode;

namespace NetworkGateway;

public class Server
{
    private readonly Gateway.Gateway _gateway;
    private readonly Dispatcher.Dispatcher _dispatcher;
    private readonly TcpServerNodeService _tcpServerNodeService;
    private readonly AConnectionService _connectionService;

    public Server(AConnectionService connectionService)
    {
        DoubleBufferingQueue<ServerMessage> serverMessageQueue = new();
        DoubleBufferingQueue<ClientMessage> clientMessageQueue = new();

        _dispatcher = new(clientMessageQueue);
        _tcpServerNodeService = new(_dispatcher, serverMessageQueue);
        _gateway = new(_dispatcher, serverMessageQueue);
        _connectionService = connectionService;
        _connectionService.AddConnectionHandler += _gateway.AddConnection;
    }

    public void Start()
    {
        _dispatcher.Start();
        _gateway.Start();
        _tcpServerNodeService.Start();
        _connectionService.Start();
    }
}
