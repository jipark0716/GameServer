namespace NetworkGateway.Connection;

public interface IConnectionFactory
{
}

public abstract class AConnectionFactory : IConnectionFactory
{
    private ulong _connectionIdSequence = 0;
    protected ulong ConnectionIdSequence { get => _connectionIdSequence++; }
}
