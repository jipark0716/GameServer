using Common.Queues;
using NetworkGateway.Connection;
using NetworkGateway.Session;
using Common.Extensions;
using NetworkGateway.Packets;
using NetworkGateway.ServerNode;
using System.Collections.Concurrent;

namespace NetworkGateway.Dispatcher;

public class Dispatcher(IQueue<ClientMessage> queue) : IDispatcher
{
    private readonly IQueue<ClientMessage> _queue = queue;
    private readonly ConcurrentDictionary<byte, IServerNode> _serverNodes = [];
    private readonly SessionFactory _sessionFactory = new();

    public void Start()
    {
        Task.Run(() => _queue.Each(Dispatch));
    }

    public bool AddServer(IServerNode serverNode) => _serverNodes.TryAdd(serverNode.ServerId, serverNode);

    public void AddSession(IConnection connection, ISession session)
    {
        _queue.EnQueue(new()
        {
            ServerId = session.ServerId,
            SessionId = session.SessionId,
            Payload = BitConverter.GetBytes((ushort)0),
        });
    }
     
    public void OnClientMessage(IConnection connection, byte[] payload)
    {
        if (connection.Sessions.TryGetValue(payload[0], out var session) is false)
        {
            session = _sessionFactory.Create(payload[0]);
            connection.Sessions.TryAdd(payload[0], session);
            AddSession(connection, session);
        }

        _queue.EnQueue(new()
        {
            ServerId = payload[0],
            SessionId = session.SessionId,
            Payload = payload[0..],
        });
    }

    protected void Dispatch(ClientMessage message)
    {
        if (_serverNodes.TryGetValue(message.ServerId, out var serverNode) is false)
        {
            return;
        }
        serverNode.Send(message.GetByte());
    }
}
