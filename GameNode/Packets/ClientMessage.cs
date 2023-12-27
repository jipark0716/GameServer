namespace GameNode.Packets;

public class ClientMessage
{
    public required ulong SessionId { get; set; }
    public required ushort ProtocolType { get; set; }
    public required byte[] Payload { get; set; }
}
