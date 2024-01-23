using System.Collections;
using Network.Rooms;

namespace Network.Packets.Room;

public class RoomDto(RoomState state)
{
    public ulong Id { get; } = state.Id;
    public string Name { get; } = state.Name;
    public IEnumerable<ulong> Users { get; } = state.Users.Keys;
}