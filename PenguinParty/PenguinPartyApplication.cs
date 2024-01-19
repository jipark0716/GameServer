using Network;
using Network.Packets;
using Network.Packets.Room;
using Network.Rooms;
using PenguinParty.Repositories;

namespace PenguinParty;

public class PenguinPartyApplication : RoomApplication
{
    private readonly CardRepository _cardRepository;

    public PenguinPartyApplication(
        int maxConnections,
        int port,
        CardRepository cardRepository) : base(maxConnections, port)
    {
        Listener.Instance = this;
        _cardRepository = cardRepository;
    }
    
    protected override Room Create(ulong roomId, Author author, CreateRequest request)
        => new PenguinPartyRoom(roomId, author, request.Name, _cardRepository);
}