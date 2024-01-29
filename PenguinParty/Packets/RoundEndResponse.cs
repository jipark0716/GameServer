using PenguinParty.Dto;

namespace PenguinParty.Packets;

public class RoundEndResponse
{
    public required IEnumerable<Player> Players { get; init; }
}