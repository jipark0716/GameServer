using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;

namespace Chat;

public class ChatRoomRepository : IRoomRepository
{
    private ulong _seq;
    
    public IRoom Create(Author author, CreateRequest request)
    {
        RoomState state = new(_seq++, request.Name);
        IRoom result = new BasicRoom(state);
        return new ChatTrait(result, state);
    }
}