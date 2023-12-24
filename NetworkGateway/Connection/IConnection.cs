using NetworkGateway.Session;

namespace NetworkGateway.Connection;

public interface IConnection
{
    public delegate void OnMessage(byte[] payload);
    public ulong ConnectionId { get; }
    public Dictionary<ulong, ISession> Sessions { get; }
    public void Close();
    public event OnMessage OnMessageHandler;
    public void Send(byte[] payload);
}

public abstract class BaseConnection(ulong connectionId) : IConnection
{
    public ulong ConnectionId { get; private set; } = connectionId;
    public Dictionary<ulong, ISession> Sessions { get; } = [];
    public abstract void Close();
    public event IConnection.OnMessage OnMessageHandler;
    public abstract void Send(byte[] payload);

    public void OnMessage(byte[] payload)
    {
        if (OnMessageHandler is not null)
            OnMessageHandler(payload);
    }
}
