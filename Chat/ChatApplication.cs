using System.Net.Sockets;
using Network;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;

namespace Chat;

public class ChatApplication : RoomApplication
{
    public ChatApplication(ChatConfig config) : base(config.NetworkConfig, new ChatRoomRepository())
    {
        Listener.Instance = this;
    }
    
    protected override void SendCreateRoomResponse(Socket socket, Room room) {}
}

public sealed class ChatRoomRepository : IRoomRepository
{
    public Room Create(ulong roomId, Author author, CreateRequest request)
        => new ChatRoom(roomId, request.Name);    
}