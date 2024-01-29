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
        PenguinPartyService service = new(gameState, cardRepository);
        
        var result = base.Create(author, request);
        result = new PenguinPartyStartTrait(result, result.RoomState, service);
        return new PenguinPartyTrait(result, result.RoomState, service);
    }
}