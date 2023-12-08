using System;
namespace Chat.Packets.Request
{
    public class JoinChannelRequest : SocketPacket
    {
        public required ulong TopicId { get; set; }
    }

    public class CreateChannelRequest : SocketPacket
    {
    }
}