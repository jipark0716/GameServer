using System.Net;
using System.Net.Sockets;
using Network.Packets;
using Util.Extensions;

namespace Network.Sockets;

public class ConnectionManager : IDisposable
{
    private readonly IPEndPoint _endpoint;
    private readonly Socket _server;
    private readonly SocketEventPool _socketEventPool;
    private readonly Dictionary<ulong, Author> _connections;
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
        Author author = new(ConnectionId, e.AcceptSocket ?? throw new InvalidOperationException());
        readEventArgs.UserToken = author;
        _messageQueue.Enqueue(new ClientMessage(author, MessageType.Connect));
        _connections.Add(author.ConnectionId, author);

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

            var author = (Author)e.UserToken;

            _messageQueue.Enqueue(new(author, MessageType.Message, payload));
            ProcessSend(e);
            return;
        }
        OnCloseClientSocket(e);
    }

    private void ProcessSend(SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success)
        {
            var author = (Author)e.UserToken;
            if (author.Socket.ReceiveAsync(e) is false)
                OnReceiveMessage(e);
            return;
        }
        OnCloseClientSocket(e);
    }

    private void OnCloseClientSocket(SocketAsyncEventArgs e)
    {
        var author = (Author)e.UserToken;

        try
        {
            author.Socket.Shutdown(SocketShutdown.Send);
        }
        catch (Exception)
        {
            // ignored
        }

        author.Socket.Close();

        _socketEventPool.Push(e);
        _connections.Remove(author.ConnectionId);
        _messageQueue.Enqueue(new(author, MessageType.Disconnect));
    }

    public void Dispose()
    {
        _connections.Each(o => o.Value.Socket.Close());
        _server.Dispose();
    }
}