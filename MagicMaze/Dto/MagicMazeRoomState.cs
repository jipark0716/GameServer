using Network.Packets;
using Network.Rooms;

namespace MagicMaze.Dto;

public class MagicMazeRoomState(ulong id, string name, Author owner) : RoomState(id, name, owner)
{
    public int ScenarioNo { get; set; } = 1;
}