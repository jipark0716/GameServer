namespace NetworkGateway.Connection;

public interface IConnectionService
{
    public delegate void AddConnection(IConnection connection);
    public event AddConnection AddConnectionHandler;

    public void Start();
}

public abstract class AConnectionService : IConnectionService
{
    public event IConnectionService.AddConnection? AddConnectionHandler;

    public virtual void Start()
    {

    }

    protected virtual ulong? Authrize(byte[] payload)
    {
        if (payload.Length < 8)
        {
            return null;
        }
        return BitConverter.ToUInt64(payload.AsSpan()[..8]);
    }

    public virtual void AddConnection(IConnection connection)
    {
        if (AddConnectionHandler is not null)
        {
            AddConnectionHandler(connection);
        }
    }
}