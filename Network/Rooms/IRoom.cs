using System.Runtime.Serialization;
using Network.Packets;
using Util.Extensions;

namespace Network.Rooms;

public interface IRoom
{
    public RoomState RoomState { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="author"></param>
    /// <returns>true 이면 방 삭제</returns>
    public bool Leave(Author author);
    public void Join(Author author);
    public void OnMessage(Author author, ushort actionType, byte[] body);
}

public class RoomState(ulong id, string name)
{
    public ulong Id { get; init; } = id;
    public readonly string Name = name;
    public readonly Dictionary<ulong, Author> Users = [];

    public void Broadcast(byte[] payload)
        => Users.Values.Each(o => o.Socket.SendAsync(payload));
}