using System.Net.Sockets;
using Network.Attributes;
using Util.Extensions;
using Network.Packets.Room;
using Network.Rooms;

namespace Network;

public abstract class RoomApplication : Application
{
    private readonly Dictionary<ulong, Room> _rooms = new();
    private readonly Dictionary<ulong, ulong> _users = new();
    private ulong _roomSequence;
    
    protected RoomApplication(int maxConnections, int port): base(maxConnections, port)
    {
        Listener.Instance = this;
        Listener.AddAction(1000, nameof(CreateRoom));
        Listener.AddAction(1001, nameof(RoomJoin));
        Listener.DefaultAction = OnRoomMessage;
    }

    protected override void OnDisconnect(ulong id)
        => LeaveRoom(id);

    private void OnRoomMessage(ushort actionType, ulong id, Socket socket, byte[] body)
    {
        if (_users.TryGetValue(id, out var roomId) is false) return;
        if (_rooms.TryGetValue(roomId, out var room) is false) return;
        room.OnMessage(actionType, id, socket, body);
    }

    protected abstract Room Create(ulong roomId, CreateRequest request);

    public void CreateRoom([ID] ulong id, [Socket] Socket socket, [JsonBody] CreateRequest request)
    {
        var room = Create(_roomSequence++, request);
        if (_rooms.TryAdd(room.Id, room) is false) return;

        RoomJoin(id, socket, room);
        
        SendCreateRoomResponse(socket, room);
    }

    protected virtual void SendCreateRoomResponse(Socket socket, Room room)
    {
        socket.SendAsync(
            new CreateResponse(room.Id).Encapsuleation(1000));
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

    private void RoomJoin(ulong id, Socket socket, Room room)
    {
        if (_users.TryGetValue(id, out var roomId))
        {
            // 다른방에 포함되어 있으면 이전방 나가기
            _users[id] = room.Id;
            LeaveRoom(id, roomId);
        }
        _users.Add(id, room.Id);
        room.AddUser(id, socket);
    }

    public void RoomJoin([ID] ulong id, [Socket] Socket socket, [JsonBody] JoinRequest request)
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

        RoomJoin(id, socket, room);
    }

    protected virtual bool RoomAuthenticate(Room room, JoinRequest request) => true;
}