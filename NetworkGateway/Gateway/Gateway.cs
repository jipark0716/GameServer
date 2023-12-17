using NetworkGateway.Connection;
using NetworkGateway.Session;
using System.Collections.Concurrent;
using NetworkGateway.Dispatcher;
using Common.Queues;
using NetworkGateway.Packets;
using Common.Extensions;

namespace NetworkGateway.Gateway;

public class Gateway
{
    public delegate void OnAddSession(IConnection connection, ISession session);
    public event OnAddSession OnAddSessionHandler;
    public delegate void OnClientMessage(IConnection connection, byte[] payload);
    public event OnClientMessage OnClientMessageHandler;

    private readonly IQueue<ServerMessage> _serverMessageQueue;
    private readonly ConcurrentDictionary<ulong, IConnection> _connections;
    
    public Gateway(IDispatcher dispatcher, IQueue<ServerMessage> serverMessageQueue)
    {
        _connections = new();
        _serverMessageQueue = serverMessageQueue;
        OnAddSessionHandler += dispatcher.AddSession;
        OnClientMessageHandler += dispatcher.OnClientMessage;
    }

    public void Start()
    {
        Task.Run(() => _serverMessageQueue.Each(OnServerMessage));
    }

    public void OnServerMessage(ServerMessage serverMessage)
    {
        var header = serverMessage.Payload[0];
        var body = serverMessage.Payload[0..];
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
        connection.OnMessageHandler += (byte[] payload) => OnClientMessageHandler(connection, payload);
        _connections.TryAdd(connection.ConnectionId, connection);
    }
}
