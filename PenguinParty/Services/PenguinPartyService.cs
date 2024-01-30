using PenguinParty.Dto;
using PenguinParty.Packets;
using PenguinParty.Repositories;
using Util.Extensions;

namespace PenguinParty.Services;

public class PenguinPartyService(
    IPenguinPartyService source,
    GameState state, 
    CardRepository cardRepository) : IPenguinPartyService
{
    public void Start(ulong[] userIds)
    {
        state.IsStart = true;
        state.Players = userIds.Select(o => new Player{UserId = o}).ToArray();
        source.Start(userIds);
    }

    public void RoundStart()
    {
        ShuffleCard();
        source.RoundStart();
    }

    public void RoundEnd()
    {
        state.Players.Each(o => o.Score += o.Cards.Count);
        state.Board.Clear();
        source.RoundEnd();
        state.Players.Each(o => o.Cards.RemoveAll(_ => true));
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
        if (state.Board.IsSubmittable(state.CurrentTurnPlayer.Cards.Distinct().ToArray())) return;
        
        AutoSkipTurn();
        TurnEnd(skipCount + 1);
    }

    public void AutoSkipTurn() => source.AutoSkipTurn();

    public void SubmitCard(SubmitCardRequest request)
    {
        var card = state.CurrentTurnPlayer.Cards[request.CardIndex];
        
        ObjectDisposedException.ThrowIf(
            state.Board.Submit(request.X, request.Y, card) is false,
            new InvalidOperationException("놓을 수 없는 위치"));

        source.SubmitCard(request);
        state.CurrentTurnPlayer.Cards.RemoveAt(request.CardIndex);
        TurnEnd();
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
            Array.Sort(hands[i], (a, b) =>
            {
                if (a == b) return 0;
                return a.Type > b.Type ? 1 : -1;
            });
            player.Cards.AddRange(hands[i]);
        }
    }
}