using Network.Attributes;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;

namespace Network.Node.Traits;

public class RoomTrait(IGameNode node, JwtDecoder jwtDecoder, IRoomRepository roomRepository) : BaseTrait(node, jwtDecoder)
{
    private readonly Dictionary<ulong, IRoom> _rooms = new();
    
    public override void OnDisconnect(Author author)
    {
        base.OnDisconnect(author);
        LeaveRoom(author);
    }

    protected override void DefaultAction(Author author, ushort type, byte[] body)
        => author.Room?.OnMessage(author, type, body);
    
    [Action(1000)]
    public void Create([Author] Author author, [JsonBody] CreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var room = roomRepository.Create(author, request);
        if (_rooms.TryAdd(room.RoomState.Id, room) is false) return;

        author.Room = room;
        room.Join(author);
    }

    [Action(1001)]
    public void Join([Author] Author author, [JsonBody] JoinRequest request)
    {
        if (_rooms.TryGetValue(request.Id, out author.Room) is false)
        {
            return;
        }
        
        author.Room.Join(author);
    }

    private void LeaveRoom(Author author)
    {
        var room = author.Room;
        if (room is null)
        {
            return;
        }
        
        author.Room = null;
        if (room.Leave(author))
        {
            _rooms.Remove(room.RoomState.Id);
        }
    }
}