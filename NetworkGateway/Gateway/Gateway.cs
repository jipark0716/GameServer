using NetworkGateway.Connection;
using NetworkGateway.Session;
using System.Collections.Concurrent;
using NetworkGateway.Dispatcher;
using Common.Queues;
using NetworkGateway.Packets;
using Common.Extensions;

namespace NetworkGateway.Gateway;

public class Gateway(IDispatcher dispatcher, IQueue<ServerMessage> serverMessageQueue)
{
    public delegate void OnAddSession(IConnection connection, ISession session);
    public delegate void OnClientMessage(IConnection connection, byte[] payload);

    private readonly IQueue<ServerMessage> _serverMessageQueue = serverMessageQueue;
    private readonly ConcurrentDictionary<ulong, IConnection> _connections = new();
    private readonly IDispatcher _dispatcher = dispatcher;

    public void Start() => Task.Run(() => _serverMessageQueue.Each(OnServerMessage));

    public void OnServerMessage(ServerMessage serverMessage)
    {
        var header = serverMessage.Payload[0];
        var body = serverMessage.Payload[1..];
        switch (header)
        {
            case 0:
                SendMessage(body);
                break;
            default: break;
        }
    }
    
    public void SendMessage(byte[] body)
    {
        var targetConnectionId = BitConverter.ToUInt64(body[..8]);
        if (_connections.TryGetValue(targetConnectionId, out var connection) is false)
        {
            return;
        }
        connection.Send(body[8..]);
    }

    public void AddConnection(IConnection connection)
    {
        connection.OnMessageHandler += (byte[] payload) => _dispatcher.OnClientMessage(connection, payload);
        connection.OnCLose += () =>
        {
            _dispatcher.Disconnect(connection);
            _connections.Remove(connection.ConnectionId, out _);
        };
        _connections.TryAdd(connection.ConnectionId, connection);
    }
}
