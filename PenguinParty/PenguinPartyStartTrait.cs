using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Services;

namespace PenguinParty;

public class PenguinPartyStartTrait(
    IRoom room,
    RoomState roomState,
    IPenguinPartyService service) : BaseTrait(room, roomState)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        // 방장만 시작 가능
        if (author.UserId == RoomState.Owner.UserId)
        {
            base.RunAction(action, author, body);
        }
    }
    
    [Action(3000)]
    public void Start()
    {
        service.Start(RoomState.Users.Keys.ToArray());
        service.RoundStart();
    }
}