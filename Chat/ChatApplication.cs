using Network;
using Network.Packets.Room;
using Network.Rooms;

namespace Chat;

public class ChatApplication : RoomApplication
{
    public ChatApplication(int maxConnections, int port) : base(maxConnections, port)
        => Listener.Instance = this;
    
    protected override Room Create(ulong roomId, CreateRequest request)
        => new ChatRoom(roomId);
}