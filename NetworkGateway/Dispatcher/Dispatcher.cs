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
    private readonly BlockingCollection<IServerNode> _serverNodes = [];
    private readonly SessionFactory _sessionFactory = new();

    public void Start()
    {
        Task.Run(() => _queue.Each(Dispatch));
    }

    public bool AddServer(IServerNode serverNode) => _serverNodes.TryAdd(serverNode);

    public void AddSession(IConnection connection, ISession session)
    {
        var payload = new byte[14];
        BitConverter.GetBytes((ushort)0).CopyTo(payload, 0);
        BitConverter.GetBytes(connection.UserId).CopyTo(payload, 2);
        _queue.EnQueue(new()
        {
            ServerId = session.ServerId,
            SessionId = session.SessionId,
            Payload = payload,
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
            Payload = payload[1..],
        });
    }

    public void Disconnect(IConnection connection)
    {
        connection.Sessions.Select(o => new ClientMessage()
        {
            ServerId = o.Value.ServerId,
            SessionId = o.Value.SessionId,
            Payload = BitConverter.GetBytes((ushort)1),
        }).Each(_queue.EnQueue);
    }

    protected void Dispatch(ClientMessage message)
    {
        _serverNodes
            .Where(o => o.ServerId == message.ServerId)
            .Each(o => o.Send(message.GetByte()));
    }
}
