using Chat;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using Util.Extensions;

namespace MagicMaze;

public class MagicMazeRoomRepository(IServiceProvider provider) : IRoomRepository
{
    private ulong _seq;

    public IRoom Create(Author author, CreateRequest request)
    {
        RoomState roomState = new(_seq++, request.Name, author);

        return provider.ProxyBuild<IRoom>(
            [roomState],
            [typeof(BasicRoom), typeof(ChatTrait)]);
    }
}