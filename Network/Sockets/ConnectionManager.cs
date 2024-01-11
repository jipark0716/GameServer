using System.Net;
using System.Net.Sockets;
using Network.Packets;

namespace Network.Sockets;

public class ConnectionManager
{
    private readonly IPEndPoint _endpoint;
    private readonly Socket _server;
    private readonly SocketEventPool _socketEventPool;
    private readonly Dictionary<ulong, Socket> _connections;
    private readonly Queue<ClientMessage> _messageQueue;
    private ulong _connectionSequence;

    private ulong ConnectionId => _connectionSequence++;

    public ConnectionManager(Queue<ClientMessage> messageQueue, int maxConnections, int port)
    {
        _messageQueue = messageQueue;
        _connections = new();
        _endpoint = new(IPAddress.Any, port);
        _server = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socketEventPool = new(maxConnections);

        _socketEventPool.Init(maxConnections, () =>
        {
            SocketAsyncEventArgs result = new();
            result.Completed += OnIoComplete;
            result.SetBuffer(new byte[1024], 0, 1024);
            return result;
        });
    }

    public Task Start() => Task.Run(() => {
        _server.Bind(_endpoint);
        _server.Listen(100);

        SocketAsyncEventArgs args = new();
        args.Completed += AcceptEventArgCompleted;
        StartAccept(args);
    });
    
    private void AcceptEventArgCompleted(object? sender, SocketAsyncEventArgs e)
    {
        OnConnect(e);
        StartAccept(e);
    }

    private void StartAccept(SocketAsyncEventArgs acceptEventArg)
    {
        while (true)
        {
            acceptEventArg.AcceptSocket = null;
            if (_server.AcceptAsync(acceptEventArg)) {
                return;
            }
            OnConnect(acceptEventArg);
        }
    }

    private void OnConnect(SocketAsyncEventArgs e)
    {
        var readEventArgs = _socketEventPool.Pop();
        var (id, socket) = (ConnectionId, e.AcceptSocket ?? throw new InvalidOperationException());
        readEventArgs.UserToken = (id, socket);
        _messageQueue.Enqueue(new ClientMessage(id, socket, MessageType.Connect));
        _connections.Add(id, socket);

        if (e.AcceptSocket.ReceiveAsync(readEventArgs) is false)
        {
            OnReceiveMessage(readEventArgs);
        }
    }

    private void OnIoComplete(object? sender, SocketAsyncEventArgs e)
    {
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Receive:
                OnReceiveMessage(e);
                break;
            case SocketAsyncOperation.Send:
                ProcessSend(e);
                break;
            default:
                throw new ArgumentException("The last operation completed on the socket was not a receive or send");
        }
    }

    private void OnReceiveMessage(SocketAsyncEventArgs e)
    {
        if (e.BytesTransferred > 0 & e.SocketError == SocketError.Success)
        {
            var payload = e.Buffer?[..e.BytesTransferred];
            var (id, socket) = ((ulong, Socket))(e.UserToken ?? throw new InvalidOperationException());
            _messageQueue.Enqueue(new(id, socket, MessageType.Message, payload));
            ProcessSend(e);
            return;
        }
        OnCloseClientSocket(e);
    }

    private void ProcessSend(SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            var (_, socket) = ((ulong, Socket))(e.UserToken ?? throw new InvalidOperationException());
            if (socket.ReceiveAsync(e) is false)
                OnReceiveMessage(e);
            return;
        }
        OnCloseClientSocket(e);
    }

    private void OnCloseClientSocket(SocketAsyncEventArgs e)
    {
        var (id, socket) = ((ulong, Socket))(e.UserToken ?? throw new InvalidOperationException());

        try
        {
            socket.Shutdown(SocketShutdown.Send);
        }
        catch (Exception)
        {
            // ignored
        }

        socket.Close();

        _socketEventPool.Push(e);
        _connections.Remove(id);
        _messageQueue.Enqueue(new(id, socket, MessageType.Disconnect));
    }
}