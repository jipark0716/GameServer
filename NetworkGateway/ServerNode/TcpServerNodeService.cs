using System.Net.Sockets;
using System.Net;
using Common.Queues;
using NetworkGateway.Packets;
using NetworkGateway.Dispatcher;

namespace NetworkGateway.ServerNode;

public class TcpServerNodeService : IServerNodeService
{
    private readonly Socket _listener;
    private readonly IQueue<ServerMessage> _serverMessageQueue;
    private readonly IDispatcher _dispatcher;

    public TcpServerNodeService(IDispatcher dispatcher, IQueue<ServerMessage> serverMessageQueue)
    {
        _dispatcher = dispatcher;
        _serverMessageQueue = serverMessageQueue;
        _listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _listener.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), 7000));
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
                    AddServerNode(handler);
                    handler.Disconnect(false);
                    handler.Close();
                });
            }
        });
    }

    private void AddServerNode(Socket socket)
    {
        byte[] buffer = new byte[4096];
        socket.Receive(buffer);
        TcpServerNode serverNode = new(socket, buffer[0]);
        if (_dispatcher.AddServer(serverNode) is false)
        {
            return;
        }

        while (true)
        {
            socket.Receive(buffer);
            _serverMessageQueue.EnQueue(new()
            {
                ServerId = serverNode.ServerId,
                Payload = buffer,
            });
        }
    }
}
