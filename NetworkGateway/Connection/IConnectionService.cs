namespace NetworkGateway.Connection;

public interface IConnectionService
{
    public delegate void AddConnection(IConnection connection);
    public event AddConnection AddConnectionHandler;

    public void Start();
}

public abstract class AConnectionService : IConnectionService
{
    public event IConnectionService.AddConnection AddConnectionHandler;

    public void Start()
    {

    }

    public virtual void AddConnection(IConnection connection)
    {
        AddConnectionHandler(connection);
    }
}