namespace Network.Packets;

public class HelloPacket(ulong id)
{
    public ulong Id { get; } = id;
}