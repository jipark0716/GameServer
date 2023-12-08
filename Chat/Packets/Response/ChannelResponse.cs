using System;
namespace Chat.Packets.Response
{
    public class CreateChannelResponse : SocketPacket
    {
        public required ulong TopicId { get; set; }
    }
}

