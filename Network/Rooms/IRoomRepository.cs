using Network.Packets;
using Network.Packets.Room;

namespace Network.Rooms;

public interface IRoomRepository
{
    public IRoom Create(Author author, CreateRequest request);
}