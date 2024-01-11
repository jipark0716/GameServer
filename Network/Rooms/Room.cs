using System.Net.Sockets;
using Network.EventListeners;

namespace Network.Rooms;

public abstract class Room
{
    public readonly ulong Id;
    private readonly Dictionary<ulong, Socket> _users = new();
    protected readonly OnClientMessageListener Listener;

    protected Room(ulong id)
    {
        Listener = new(this);
        Id = id;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="id"></param>
    /// <returns>true 이면 방 삭제</returns>
    public bool LeaveUser(ulong id)
    {
        _users.Remove(id);
        
        return _users.Count != 0;
    }

    public virtual void AddUser(ulong id, Socket socket, bool isFirst = false)
        => _users.Add(id, socket);

    public void OnMessage(ushort actionType, ulong id, Socket socket, byte[] body)
        => Listener.OnMessage(actionType, id, socket, body);

    protected Task Send(byte[] payload) => Task.WhenAll(
        _users.Select(
            o 
                => o.Value.SendAsync(payload, SocketFlags.Peek))
    );
}