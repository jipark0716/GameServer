using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using Util.Extensions;

namespace Chat;

public class ChatRoomRepository(IServiceProvider provider) : IRoomRepository
{
    private ulong _seq;

    public IRoom Create(Author author, CreateRequest request)
    {
        return provider.ProxyBuild<IRoom>(
            [new RoomState(_seq++, request.Name, author)],
            [typeof(BasicRoom), typeof(ChatTrait)]);
    }
}