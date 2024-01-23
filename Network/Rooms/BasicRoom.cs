using Network.Exceptions;
using Network.Packets;

namespace Network.Rooms;

public class BasicRoom(RoomState roomState) : IRoom
{
    public RoomState RoomState { get; } = roomState;

    public bool Leave(Author author)
    {
        RoomState.Users.Remove((ulong)author.UserId!);
        return RoomState.Users.Count == 0;
    }

    public void Join(Author author)
    {
        var userId = (ulong)author.UserId!;
        RoomState.Users.Add(userId, author);
    }

    public void OnMessage(Author author, ushort actionType, byte[] body)
        => throw new NotSupportActionException();
}