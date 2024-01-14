using System.Net.Sockets;
using Chat.Packets.Messages;
using Network.Attributes;
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
    
    public void SendMessage([ID] ulong id, [Socket] Socket socket, [JsonBody] SendRequest request)
    {
        Message message = new()
        {
            Author = id,
            MessageId = _messageSequence++,
            Type = request.Type,
            Content = request.Content
        };
        _messages.Add(message);
        Send(new SendResponse(message).Encapsuleation(2000));
    }

    public override void AddUser(ulong id, Socket socket)
    {
        base.AddUser(id, socket);
        socket.SendAsync(new JoinResponse(CreateRoomPacket(), _messages).Encapsuleation(1001));
    }
}