namespace NetworkGateway.Packets;

public class ServerMessage
{
    public required byte ServerId { get; set; }
    public required byte[] Payload { get; set; }
}
