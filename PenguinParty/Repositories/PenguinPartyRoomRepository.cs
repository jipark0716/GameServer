using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;

namespace PenguinParty.Repositories;

public class PenguinPartyRoomRepository(CardRepository cardRepository) : IRoomRepository
{
    public Room Create(ulong roomId, Author author, CreateRequest request)
        => new PenguinPartyRoom(roomId, author, request.Name, cardRepository);
}