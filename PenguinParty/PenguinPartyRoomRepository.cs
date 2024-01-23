using Chat;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using PenguinParty.Dto;
using PenguinParty.Repositories;

namespace PenguinParty;

public class PenguinPartyRoomRepository(CardRepository cardRepository) : ChatRoomRepository
{
    public override IRoom Create(Author author, CreateRequest request)
    {
        GameState gameState = new();
        
        var result = base.Create(author, request);
        result = new PenguinPartyStartTrait(result, result.RoomState, gameState, cardRepository);
        return new PenguinPartyTrait(result, result.RoomState, gameState);
    }
}