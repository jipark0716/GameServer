namespace NetworkGateway.ServerNode;

public interface IServerNode
{
    public byte ServerId { get; }
    public void Send(byte[] payload);
}
