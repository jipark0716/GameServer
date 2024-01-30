using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Dto;
using PenguinParty.Packets;
using PenguinParty.Services;

namespace PenguinParty;

public class PenguinPartyTrait(
    IRoom room,
    RoomState roomState,
    GameState gameState,
    IPenguinPartyService service) : BaseTrait(room, roomState)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        if (author.UserId == gameState.CurrentTurnPlayer.UserId && gameState.IsStart)
        {
            base.RunAction(action, author, body);
        }
    }
    
    [Action(3001)]
    public void SubmitCard([JsonBody] SubmitCardRequest request)
    {
        service.SubmitCard(request);
    }
}