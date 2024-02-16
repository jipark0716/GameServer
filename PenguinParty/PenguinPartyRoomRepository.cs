using Chat;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using PenguinParty.Dto;
using PenguinParty.Services;
using Util.Extensions;

namespace PenguinParty;

public class PenguinPartyRoomRepository(IServiceProvider provider) : IRoomRepository
{
    private ulong _seq;

    public IRoom Create(Author author, CreateRequest request)
    {
        GameState gameState = new();
        RoomState roomState = new(_seq++, request.Name, author);

        var service = provider.ProxyBuild<IPenguinPartyService>(
            [gameState, roomState],
            [typeof(PenguinPartyIoService), typeof(PenguinPartyService)]);

        return provider.ProxyBuild<IRoom>(
            [gameState, roomState, service],
            [typeof(BasicRoom), typeof(ChatTrait), typeof(PenguinPartyStartTrait), typeof(PenguinPartyTrait)]);
    }
}