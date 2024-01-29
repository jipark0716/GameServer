using PenguinParty.Dto;

namespace PenguinParty.Packets;

public class StartRoundResponse
{
    public required IEnumerable<Card> Cards { get; init; }
}