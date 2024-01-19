using Network.EventListeners;
using Network.Packets;
using Network.Packets.Room;
using Util.Extensions;

namespace Network.Rooms;

public abstract class Room
{
    public readonly ulong Id;
    public readonly string Name;
    protected readonly Dictionary<ulong, Author> Users = new();
    protected readonly OnClientMessageListener Listener;

    protected Room(ulong id, string name)
    {
        Listener = new(this);
        Id = id;
        Name = name;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true 이면 방 삭제</returns>
    public virtual bool LeaveUser(ulong id)
    {
        Users.Remove(id);
        
        return Users.Count == 0;
    }

    public virtual void AddUser(Author author)
    {
        var userId = (ulong)author.UserId!;
        Users.Add(userId, author);
        Send(new OnJoin(userId).Encapsuleation(1002), userId);
    }

    public void OnMessage(ushort actionType, Author author, byte[] body)
        => Listener.OnMessage(actionType, author, body);

    protected Task Send(byte[] payload) => Task.WhenAll(
        Users.Select(
            o 
                => o.Value.Socket.SendAsync(payload))
    );
    
    protected Task Send(byte[] payload, ulong id) => Task.WhenAll(
        Users.Where(o => o.Key != id).Select(
            o 
                => o.Value.Socket.SendAsync(payload))
    );

    protected virtual RoomDto CreateRoomPacket()
        => new(Id, Users.Select(o => o.Key).ToHashSet(), Name);
}