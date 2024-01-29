using Network.Attributes;
using Network.Packets;
using Network.Rooms;
using Network.Rooms.Traits;
using PenguinParty.Dto;
using PenguinParty.Packets;
using PenguinParty.Services;
using Util.Extensions;

namespace PenguinParty;

public class PenguinPartyStartTrait(
    IRoom room,
    RoomState roomState,
    PenguinPartyService service) : BaseTrait(room, roomState)
{
    protected override void RunAction(Listener action, Author author, byte[] body)
    {
        service.OnRoundStart += SendRoundStartPacket;
        service.OnRoundEnd += SendRoundEndPacket;
        // 방장만 시작 가능
        if (author == RoomState.Owner)
        {
            base.RunAction(action, author, body);
        }
    }
    
    [Action(3000)]
    public void Start()
    {
        service.Start(RoomState.Users.Keys);
    }

    private void SendRoundStartPacket()
    {
        foreach (var player in service.GetPlayers())
        {
            RoomState.Users
                .GetValueOrDefault(player.UserId)?
                .Socket
                .SendAsync(
                    new StartRoundResponse
                    {
                        Cards = player.Cards
                    }.Encapsulation(3000));
        }
    }

    private void SendRoundEndPacket()
    {
        roomState.Broadcast(new RoundEndResponse
        {
            Players = service.GetPlayers()
        }.Encapsulation(3002));
    }
}