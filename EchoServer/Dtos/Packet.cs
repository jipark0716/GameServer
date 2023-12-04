namespace EchoServer.Dtos
{
    public class Packet
    {
        public required ISession Session { get; set; }
        public required PacketType Type { get; set; }
        public byte[]? Body { get; set; }
        public ushort Header { get; set; }
    }

    public enum PacketType
    {
        Basic,
        Close,
        ContentToLarge,
    }
}
