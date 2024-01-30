using Chat;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using PenguinParty.Dto;
using PenguinParty.Repositories;
using PenguinParty.Services;

namespace PenguinParty;

public class PenguinPartyRoomRepository(CardRepository cardRepository) : ChatRoomRepository
{
    public override IRoom Create(Author author, CreateRequest request)
    {
        GameState gameState = new();
        var result = base.Create(author, request);
        var service = GetService(gameState, result.RoomState);
        result = new PenguinPartyStartTrait(result, result.RoomState, service);
        return new PenguinPartyTrait(result, result.RoomState, gameState, service);
    }

    private IPenguinPartyService GetService(GameState gameState, RoomState roomState)
    {
        IPenguinPartyService result = new PenguinPartyIoService(gameState, roomState);
        return new PenguinPartyService(result, gameState, cardRepository);
    }
}