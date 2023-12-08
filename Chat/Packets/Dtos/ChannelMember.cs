using System;
namespace Chat.Packets.Dtos
{
    public interface IChannelMember
    {
        public ulong SessionId { get; set; }
    }

    public class ChannelMember : IChannelMember
    {
        public required ulong SessionId { get; set; }
    }
}

