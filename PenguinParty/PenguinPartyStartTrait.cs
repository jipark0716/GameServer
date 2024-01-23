using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Dto;
using PenguinParty.Repositories;
using Util.Extensions;

namespace PenguinParty;

public class PenguinPartyStartTrait(
    IRoom room,
    RoomState roomState,
    GameState gameState,
    CardRepository cardRepository) : BaseTrait(room, roomState)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        // 방장만 시작 가능
        if (author == roomState.Owner)
        {
            base.RunAction(action, author, body);
        }
    }
    
    [Action(1001)]
    public void Start([Author] Author author)
    {
        gameState.IsStart = true;
        gameState.Players = RoomState.Users.Keys.Select(o => new Player(o)).ToArray();
        ShuffleCard();
    }
    
    private byte GetCardCount()
        => (byte)(36 - 36 % gameState.Players.Length);
    
    private void ShuffleCard()
    {
        var hands = cardRepository .Get(GetCardCount())
            .Shuffle()
            .Chunk(gameState.Players.Length)
            .ToArray();
        
        foreach (var (player, i) in gameState.Players.WithIndex())
        {
            player.Cards.AddRange(hands[i]);
        }
    }
}