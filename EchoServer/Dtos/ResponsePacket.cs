namespace EchoServer.Dtos
{
    public class ResponsePacket
    {
        public required ITarget Target { get; set; }
        public required byte[] Payload { get; set; }
    }

    public interface ITarget {}

    public class BroadcastTarget : ITarget { }

    public class UserTarget : ITarget
    {
        public required ulong Uid { get; set; }
    }

    public class TopicTarget : ITarget
    {
        public required ulong TopicId { get; set; }
    }
}
