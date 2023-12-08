using System;
using Chat.Packets.Dtos;

namespace Chat.Packets.Response
{
	public class MessageResponse : SocketPacket
	{
		public required ulong TopicId { get; set; }
		public required IMessage Message { get; set; }
    }
}

