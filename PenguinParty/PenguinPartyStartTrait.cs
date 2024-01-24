using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Dto;
using PenguinParty.Packets;
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
    
    [Action(3000)]
    public void Start([Author] Author author)
    {
        gameState.IsStart = true;
        gameState.Players = RoomState.Users.Keys.Select(o => new Player(o)).ToArray();
        ShuffleCard();
        foreach (var player in gameState.Players)
        {
            roomState.Users
                .GetValueOrDefault(player.UserId)?
                .Socket
                .SendAsync(
                    new StartRoundResponse(player.Cards).Encapsulation(3000));
        }
    }
    
    private byte GetCardCount()
        => (byte)(36 - 36 % gameState.Players.Length);
    
    private void ShuffleCard()
    {
        var hands = cardRepository.Get(GetCardCount())
            .Shuffle()
            .Chunk(GetCardCount() / gameState.Players.Length)
            .ToArray();
        
        foreach (var (player, i) in gameState.Players.WithIndex())
        {
            player.Cards.AddRange(hands[i]);
        }
    }
}