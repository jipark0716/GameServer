using System;
namespace Chat.Packets.Dtos
{
	public class ChatChannel
	{
		private ulong _messageId = 0;
        public ulong MessageId { get => ++_messageId; }

		public required ulong TopicId { get; set; }
		public List<IChannelMember> Members { get; set; } = new();
        public List<IMessage> Messages { get; set; } = new();
	}
}