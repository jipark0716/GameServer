namespace Network.Packets.Room;

public class CreateRequest
{
    public required string Name { get; set; }
}

public class CreateResponse(ulong roomId)
{
    public ulong RoomId { get; set; } = roomId;
}