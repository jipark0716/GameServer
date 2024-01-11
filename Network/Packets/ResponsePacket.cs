namespace Network.Packets;

public abstract class ResponsePacket(ushort type)
{
    public ushort Type { get; set; } = type;
}