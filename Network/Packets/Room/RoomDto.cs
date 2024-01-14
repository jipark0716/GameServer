namespace Network.Packets.Room;

public class RoomDto(ulong id, HashSet<ulong> users, string name)
{
    public ulong Id { get; } = id;
    public string Name { get; } = name;
    public HashSet<ulong> Users { get; } = users;
}