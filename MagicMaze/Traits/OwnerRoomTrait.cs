using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;

namespace MagicMaze.Traits;

public class OwnerRoomTrait(IRoom room, RoomState state) : BaseTrait(room, state)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        // 방장만 사용 가능
        if (author.UserId == RoomState.Owner.UserId)
        {
            base.RunAction(action, author, body);
        }
    }

    [Action(3000)]
    public void Start()
    {

    }

}