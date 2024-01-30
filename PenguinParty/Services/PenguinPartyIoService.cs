using Network.Rooms;
using PenguinParty.Dto;
using PenguinParty.Packets;
using Util.Extensions;

namespace PenguinParty.Services;

public class PenguinPartyIoService(
    GameState gameState,
    RoomState roomState) : IPenguinPartyService
{
    public void RoundStart()
    {
        foreach (var player in gameState.Players)
        {
            roomState.Users
                .GetValueOrDefault(player.UserId)?
                .Socket
                .SendAsync(
                    new StartRoundResponse
                    {
                        Cards = player.Cards
                    }.Encapsulation(3000));
        }
    }

    public void RoundEnd()
        => roomState.Broadcast(new RoundEndResponse
        {
            Players = gameState.Players
        }.Encapsulation(3002));

    public void Start(ulong[] userIds)
    {
        // todo 게임 시작 패킷 전송
    }

    public void AutoSkipTurn()
    {
        // todo 자동스킵 패킷 전송
    }

    public void SubmitCard(SubmitCardRequest request)
        => roomState.Broadcast(new SubmitCardResponse
        {
            X = request.X,
            Y = request.Y,
            Card = gameState.CurrentTurnPlayer.Cards[request.CardIndex],
            UserId = gameState.CurrentTurnPlayer.UserId
        }.Encapsulation(3001));
}