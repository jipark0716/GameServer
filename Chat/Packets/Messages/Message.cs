using System.Net;

namespace Chat.Packets.Messages;

public class Message
{
    public required ulong Author { get; set; }
    public required ulong MessageId { get; set; }
    public required MessageType Type { get; set; }
    public required string Content { get; set; }
}