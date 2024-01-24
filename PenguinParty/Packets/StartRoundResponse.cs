using PenguinParty.Dto;

namespace PenguinParty.Packets;

public class StartRoundResponse(IEnumerable<Card> cards)
{
    public IEnumerable<Card> Cards { get; init; }  = cards;
}