using System;
namespace Chat.Packets.Dtos
{
    public interface IMessage
    {
        public byte MessageType { get; }
        public ulong MessageId { get; set; }
        public IChannelMember Author { get; set; }
    }

    public abstract class Message : IMessage
    {
        public abstract byte MessageType { get; }
        public required ulong MessageId { get; set; }
        public required IChannelMember Author { get; set; }
    }

    public abstract class SystemMessage : Message { }

    public class JoinMessage : SystemMessage
    {
        public override byte MessageType { get => 0; }
    }

    public class MemberMessage : Message
    {
        public override byte MessageType { get => 1; }
        public required IMessageContent Content { get; set; }
    }

    public interface IMessageContent
    {
    }

    public class TextMessageContent : IMessageContent
    {
        public required string Value { get; set; }
    }
}
