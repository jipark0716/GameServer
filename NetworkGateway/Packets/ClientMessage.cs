namespace NetworkGateway.Packets;

public class ClientMessage
{
    public required byte ServerId { get; set; }
    public required ulong SessionId { get; set; }
    public byte[] Payload { get; set; } = [];

    public byte[] GetByte()
    {
        byte[] payload = new byte[8 + Payload.Length];
        payload.CopyTo(BitConverter.GetBytes(SessionId), 0);
        if (Payload.Length > 0)
        {
            payload.CopyTo(Payload, 8);
        }
        return payload;
    }
}
