namespace Chat.Packets.Messages;

public class SendResponse(Message message)
{
    public Message Message { get; } = message;
}