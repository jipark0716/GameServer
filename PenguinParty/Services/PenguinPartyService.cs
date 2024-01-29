using PenguinParty.Dto;
using PenguinParty.Packets;
using PenguinParty.Repositories;
using Util.Extensions;

namespace PenguinParty.Services;

public class PenguinPartyService(
    GameState state, 
    CardRepository cardRepository)
{
    public event Action? OnRoundStart;
    public event Action? OnRoundEnd;

    public bool IsStart => state.IsStart;

    public bool IsCurrentPlayer(ulong userId) => userId == state.CurrentTurnPlayerId;

    public IEnumerable<Player> GetPlayers() => state.Players;

    public void Start(IEnumerable<ulong> userIds)
    {
        state.IsStart = true;
        state.Players = userIds.Select(o => new Player(o)).ToArray();
    }

    private void RoundStart()
    {
        ShuffleCard();
        OnRoundStart?.Invoke();
    }

    private void RoundEnd()
    {
        GetPlayers().Each(o => o.Score += o.Cards.Count);
        OnRoundEnd?.Invoke();
        state.Board.Clear();
        RoundStart();
    }

    private void TurnEnd(int skipCount = 0)
    {
        if (skipCount >= state.Players.Length)
        {
            RoundEnd();
            return;
        }
        
        state.Turn++;
        
        // 다음 사람이 할 수 있는게 없으면 스킵
        if (state.Board.IsSubmittable(state.CurrentTurnPlayer.Cards.Distinct().ToArray()))
        {
            // 자동스킵
            TurnEnd(skipCount + 1);
        }
    }

    public Card? SubmitCard(SubmitCardRequest request)
    {
        var card = state.CurrentTurnPlayer.Cards[request.CardIndex];
        
        if (state.Board.Submit(request.X, request.Y, card) is false)
        {
            return null;
        }

        state.CurrentTurnPlayer.Cards.RemoveAt(request.CardIndex);
        TurnEnd();
        return card;
    }
    
    private byte GetCardCount()
        => (byte)(36 - 36 % state.Players.Length);
    
    private void ShuffleCard()
    {
        var hands = cardRepository.Get(GetCardCount())
            .Shuffle()
            .Chunk(GetCardCount() / state.Players.Length)
            .ToArray();
        
        foreach (var (player, i) in state.Players.WithIndex())
        {
            Array.Sort(hands[i], (o, v) => o.Type > v.Type ? 1 : -1);
            player.Cards.AddRange(hands[i]);
        }
    }
}