using Chat.Packets.Dtos;

namespace Chat.Packets.Request
{
	public class SendMessageRequest : SocketPacket
	{
        public required ulong TopicId { get; set; }
        public required IMessageContent MessageContent { get; set; }
    }
}