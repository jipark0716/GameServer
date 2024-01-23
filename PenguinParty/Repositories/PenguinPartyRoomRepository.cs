using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using PenguinParty.Dto;
using Util.Entity.Models;
using Util.Saves;

namespace PenguinParty.Repositories;

public class PenguinPartyRoomRepository(CardRepository cardRepository, SaveRepository saveRepository) : IRoomRepository
{
    public BasicRoom Create(ulong roomId, Author author, CreateRequest request)
        => new PenguinPartyBasicRoom(roomId, author, request.Name, cardRepository, saveRepository);
}