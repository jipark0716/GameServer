using System;
namespace Chat.Packets
{
	public interface ISocketPacket
	{
		public ulong Sequence { get; set; }
	}

	public abstract class SocketPacket : ISocketPacket
	{
        public required ulong Sequence { get; set; }
    }
}