using System.Net.Sockets;
using Network.Attributes;
using Network.EventListeners;
using Network.Packets;
using Util.Extensions;
using Network.Packets.Room;
using Network.Rooms;

namespace Network;

public abstract class RoomApplication : Application
{
    private readonly Dictionary<ulong, Room> _rooms = new();
    private readonly Dictionary<ulong, ulong> _users = new();
    private ulong _roomSequence;
    private readonly IRoomRepository _roomRepository;
    
    protected RoomApplication(NetworkConfig config, IRoomRepository roomRepository): base(config)
    {
        _roomRepository = roomRepository;
        AuthorizeMiddleware middleware = new();
        
        Listener.Instance = this;
        Listener.AddAction(1000, nameof(CreateRoom), middleware);
        Listener.AddAction(1001, nameof(RoomJoin), middleware);
        Listener.DefaultAction = OnRoomMessage;
    }

    protected override void OnDisconnect(ulong id)
        => LeaveRoom(id);

    private void OnRoomMessage(ushort actionType, Author author, byte[] body)
    {
        if (_users.TryGetValue(author.ConnectionId, out var roomId) is false) return;
        if (_rooms.TryGetValue(roomId, out var room) is false) return;
        room.OnMessage(actionType, author, body);
    }


    public void CreateRoom([Author] Author author, [JsonBody] CreateRequest request)
    {
        var room = _roomRepository.Create(_roomSequence++, author, request);
        if (_rooms.TryAdd(room.Id, room) is false) return;

        RoomJoin(author, room);
        
        SendCreateRoomResponse(author.Socket, room);
    }

    protected virtual void SendCreateRoomResponse(Socket socket, Room room)
    {
        socket.SendAsync(
            new CreateResponse(room.Id).Encapsulation(1000));
    }

    private void LeaveRoom(ulong userId)
    {
        if (_users.TryGetValue(userId, out var roomId))
            LeaveRoom(userId, roomId);
    }

    private void LeaveRoom(ulong userId, ulong roomId)
    {
        if (_rooms.TryGetValue(roomId, out var room))
            LeaveRoom(userId, room);
    }

    private void LeaveRoom(ulong userId, Room room)
    {
        _users.Remove(userId);
        if (room.LeaveUser(userId))
        {
            _rooms.Remove(room.Id);
        }
    }

    private void RoomJoin(Author author, Room room)
    {
        if (_users.TryGetValue(author.ConnectionId, out var roomId))
        {
            // 다른방에 포함되어 있으면 이전방 나가기
            _users[author.ConnectionId] = room.Id;
            LeaveRoom(author.ConnectionId, roomId);
        }
        else
        {
            _users.Add(author.ConnectionId, room.Id);
        }
        room.AddUser(author);
    }

    public void RoomJoin([Author] Author author, [JsonBody] JoinRequest request)
    {
        if (_rooms.TryGetValue(request.Id, out var room) is false)
        {
            // 방 없을때
            return;
        }
        
        if (RoomAuthenticate(room, request) is false)
        {
            // 인증 실패
            return;
        }

        RoomJoin(author, room);
    }

    protected virtual bool RoomAuthenticate(Room room, JoinRequest request) => true;
}