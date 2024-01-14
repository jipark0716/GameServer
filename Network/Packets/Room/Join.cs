namespace Network.Packets.Room;

public class JoinRequest
{
    public required ulong Id { get; set; }
}

public class JoinResponse(RoomDto room)
{
    public RoomDto Room { get; } = room;
}

public class OnJoin(ulong id)
{
    public ulong Id { get; } = id;
}