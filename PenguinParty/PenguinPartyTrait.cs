using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Packets;
using PenguinParty.Services;
using Util.Extensions;

namespace PenguinParty;

public class PenguinPartyTrait(
    IRoom room,
    RoomState roomState,
    PenguinPartyService service) : BaseTrait(room, roomState)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        if (service.IsCurrentPlayer((ulong) author.UserId!) && service.IsStart)
        {
            base.RunAction(action, author, body);
        }
    }
    
    [Action(3001)]
    public void SubmitCard([JsonBody] SubmitCardRequest request)
    {
        var card = service.SubmitCard(request);
        if (card is null) return;
        
        roomState.Broadcast(new SubmitCardResponse(request.X, request.Y, card).Encapsulation(3001));
    }
}