using Common.Queues;
using GameNode.GameRoom;
using GameNode.Packets;
using GameNode.Packets.Room;

namespace GameNode.Listener;

public abstract class HasRoomListener<T>(IQueue<ClientMessage> queue, BaseRoomFactory<T> roomFactory) : BaseListener(queue)
    where T : BaseRoom
{
    protected readonly Dictionary<ulong, int> _users = [];
    protected readonly Dictionary<int, T> _rooms = [];

    protected T CreateRoom(Session.Session session, RoomCreateRequest request)
    {
        var room = roomFactory.Create(request);
        _users.Add(session.UserId, room.Id);
        _rooms.Add(room.Id, room);
        return room;
    }

    protected virtual void CloseRoom(int roomId)
        => CloseRoom(_rooms.GetValueOrDefault(roomId) ?? throw new("room not found"));

    protected virtual void CloseRoom(T room)
    {
        
    }
}

public abstract class BaseRoomFactory<T>
    where T : BaseRoom
{
    private int _roomSequence = 0;
    protected int _roomId { get => ++_roomSequence; }

    public abstract T Create(RoomCreateRequest request);
}