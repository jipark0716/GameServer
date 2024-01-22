using Network;
using PenguinParty.Repositories;

namespace PenguinParty;

public class PenguinPartyApplication : RoomApplication
{
    public PenguinPartyApplication(
        PenguinPartyConfig config,
        CardRepository cardRepository) : base(
        config.NetworkConfig,
        new PenguinPartyRoomRepository(cardRepository))
    {
        Listener.Instance = this;
    }
}