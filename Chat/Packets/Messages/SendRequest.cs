namespace Chat.Packets.Messages;

public class SendRequest
{
    public required MessageType Type { get; set; }
    public required string Content { get; set; }
}