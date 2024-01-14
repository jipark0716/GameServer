using System.Net.Sockets;
using Network.EventListeners;
using Network.Packets.Room;
using Util.Extensions;

namespace Network.Rooms;

public abstract class Room
{
    public readonly ulong Id;
    public readonly string Name;
    protected readonly Dictionary<ulong, Socket> Users = new();
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
    public bool LeaveUser(ulong id)
    {
        Users.Remove(id);
        
        return Users.Count == 0;
    }

    public virtual void AddUser(ulong id, Socket socket)
    {
        Users.Add(id, socket);
        Send(new OnJoin(id).Encapsuleation(1002), id);
    }

    public void OnMessage(ushort actionType, ulong id, Socket socket, byte[] body)
        => Listener.OnMessage(actionType, id, socket, body);

    protected Task Send(byte[] payload) => Task.WhenAll(
        Users.Select(
            o 
                => o.Value.SendAsync(payload))
    );
    
    protected Task Send(byte[] payload, ulong id) => Task.WhenAll(
        Users.Where(o => o.Key != id).Select(
            o 
                => o.Value.SendAsync(payload))
    );

    protected virtual RoomDto CreateRoomPacket()
        => new(Id, Users.Select(o => o.Key).ToHashSet(), Name);
}