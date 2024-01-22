using Network.Packets;
using Network.Packets.Room;

namespace Network.Rooms;

public interface IRoomRepository
{
    public Room Create(ulong roomId, Author author, CreateRequest request);
}