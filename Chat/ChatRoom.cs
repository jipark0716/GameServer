using System.Net.Sockets;
using Chat.Packets.Messages;
using Network.Attributes;
using Network.Rooms;
using Util.Extensions;

namespace Chat;

public class ChatRoom : Room
{
    private ulong _messageSequence;
    private readonly List<Message> _messages = new();
    
    public ChatRoom(ulong id) : base(id)
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
        Send(new SendResponse{ Message = message }.ToJsonByte());
    }


    public override void AddUser(ulong id, Socket socket, bool isFirst = false)
    {
        base.AddUser(id, socket, isFirst);
        socket.SendAsync(
            new MessageCollectionResponse{ Messages = _messages }.ToJsonByte(),
            SocketFlags.Peek);
    }
}