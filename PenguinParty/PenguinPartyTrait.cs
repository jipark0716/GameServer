using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Dto;
using PenguinParty.Packets;

namespace PenguinParty;

public class PenguinPartyTrait(
    IRoom room,
    RoomState roomState,
    GameState gameState) : BaseTrait(room, roomState)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        if (gameState.CurrentTurnPlayer.UserId == author.UserId && gameState.IsStart)
        {
            base.RunAction(action, author, body);
        }
    }
    
    [Action(1002)]
    public void SubmitCard([Author] Author author, [JsonBody] SubmitCardRequest request)
    {
        var card = gameState.CurrentTurnPlayer.Cards[request.CardIndex];
        
        if (gameState.Board.Submit(request.X, request.Y, card) is false)
        {
            return;
        }

        gameState.CurrentTurnPlayer.Cards.RemoveAt(request.CardIndex);
        
        SkipTurn(author);
    }

    [Action(1003)]
    public void SkipTurn([Author] Author author)
    {
        gameState.Turn++;
    }
}