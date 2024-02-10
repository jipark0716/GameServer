using System.Collections;
using Network.Rooms;

namespace Network.Packets.Room;

public class RoomDto(RoomState state)
{
    public ulong Id => state.Id;
    public string Name => state.Name;
    public IEnumerable<RoomUser> Users => state.Users.Select(o
        => new RoomUser(o.Key, o.Key == state.Owner.UserId));
}

public class RoomUser(ulong id, bool isOwner)
{
    public ulong Id => id;
    public bool IsOwner => isOwner;
}