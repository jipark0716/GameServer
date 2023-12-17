namespace NetworkGateway.Session;

public interface ISession
{
    public byte ServerId { get; }
    public ulong SessionId { get; }
}
 