using Chat.Packets.Messages;
using Network.Attributes;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using Network.Rooms.Traits;
using Util.Extensions;
using JoinResponse = Chat.Packets.Messages.JoinResponse;

namespace Chat;

public class ChatTrait(IRoom room, RoomState state) : BaseTrait(room, state)
{
    private ulong _messageSequence;
    private readonly List<Message> _messages = [];
    private readonly RoomState _state = state;
    private readonly IRoom _room = room;

    [Action(2000)]
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
        _state.Broadcast(new SendResponse(message).Encapsulation(2000));
    }

    public override void Join(Author author)
    {
        _room.Join(author);
        author.Socket.SendAsync(new JoinResponse(new RoomDto(RoomState), _messages).Encapsulation(1001));
        RoomState.Broadcast(new OnJoin((ulong)author.UserId!).Encapsulation(1002));
    }
}