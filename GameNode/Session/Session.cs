namespace GameNode.Session;

public class Session
{
    public required ulong UserId { get; set; }
    public required ulong SessionId { get; set; }
    public event Action? OnDisconnect;
    public event Action? OnReconnect;
    public event Action? OnRemove;
    public bool Disconnected { get; private set; } = true;
    public void Disconnect()
    {
        Disconnected = true;
        OnDisconnect?.Invoke();
    }
    public void Reconnect()
    {
        Disconnected = false;
        OnReconnect?.Invoke();
    }
    public bool Remove()
    {
        if (Disconnected)
        {
            OnRemove?.Invoke();
            return true;
        }
        return false;
    }
}
