using Common.Queues;
using NetworkGateway.Connection;
using NetworkGateway.Packets;
using NetworkGateway.ServerNode;
using NetworkGateway.Session;

namespace NetworkGateway.Dispatcher;

public interface IDispatcher
{
    public void AddSession(IConnection connection, ISession session);
    public void OnClientMessage(IConnection connection, byte[] payload);
    public bool AddServer(IServerNode serverNode);

}
