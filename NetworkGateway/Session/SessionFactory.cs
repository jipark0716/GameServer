namespace NetworkGateway.Session;

public class SessionFactory
{
    private ulong _sessionIdSequence = 0;
    private ulong SessionIdSequence { get => _sessionIdSequence++; }

    public ISession Create(byte serverId)
    {
        return new Session()
        {
            ServerId = serverId,
            SessionId = SessionIdSequence
        };
    }
}
