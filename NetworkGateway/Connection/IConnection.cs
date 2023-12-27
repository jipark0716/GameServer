using NetworkGateway.Session;
using System.Xml;

namespace NetworkGateway.Connection;

public interface IConnection
{
    public ulong UserId { get; }
    public delegate void OnMessage(byte[] payload);
    public ulong ConnectionId { get; }
    public event Action? OnCLose;
    public Dictionary<ulong, ISession> Sessions { get; }
    public void Close();
    public event OnMessage OnMessageHandler;
    public void Send(byte[] payload);
}

public abstract class BaseConnection(ulong connectionId) : IConnection
{
    public ulong UserId { get; set; }
    public ulong ConnectionId { get; private set; } = connectionId;
    public Dictionary<ulong, ISession> Sessions { get; } = [];
    public event Action? OnCLose;
    public virtual void Close()
    {
        OnCLose?.Invoke();
    }
    public event IConnection.OnMessage? OnMessageHandler;
    public abstract void Send(byte[] payload);

    public void OnMessage(byte[] payload)
    {
        if (OnMessageHandler is not null)
            OnMessageHandler(payload);
    }
}
