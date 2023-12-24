namespace NetworkGateway.Packets;

public class ClientMessage
{
    public required byte ServerId { get; set; }
    public required ulong SessionId { get; set; }
    public byte[] Payload { get; set; } = [];

    public byte[] GetByte()
    {
        byte[] payload = new byte[8 + Payload.Length];
        BitConverter.GetBytes(SessionId).CopyTo(payload, 0);
        if (Payload.Length > 0)
        {
            Payload.CopyTo(payload, 8);
        }
        return payload;
    }
}
