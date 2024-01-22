using Chat.Packets.Messages;
using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Util.Extensions;
using JoinResponse = Chat.Packets.Messages.JoinResponse;

namespace Chat;

public class ChatRoom : Room
{
    private ulong _messageSequence;
    private readonly List<Message> _messages = new();
    
    public ChatRoom(ulong id, string name) : base(id, name)
    {
        Listener.Instance = this;
        Listener.AddAction(2000, nameof(SendMessage));
    }
    
    public void SendMessage([Author] Author author, [JsonBody] SendRequest request)
    {
        var userId = (ulong)author.UserId!;
        Message message = new()
        {
            Author = userId,
            MessageId = _messageSequence++,
            Type = request.Type,
            Content = request.Content
        };
        _messages.Add(message);
        Send(new SendResponse(message).Encapsulation(2000));
    }

    public override void AddUser(Author author)
    {
        base.AddUser(author);
        author.Socket.SendAsync(new JoinResponse(CreateRoomPacket(), _messages).Encapsulation(1001));
    }
}