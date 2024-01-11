using Network.Packets;

namespace Chat.Packets.Messages;

public class MessageCollectionResponse() : ResponsePacket(1001)
{
    public required List<Message> Messages { get; set; }
}