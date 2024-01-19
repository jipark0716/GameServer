using PenguinParty.Dto;

namespace PenguinParty.Repositories;

public class CardRepository(byte numOfTypes)
{
    private readonly Card[] _cards = CreateCared(numOfTypes);

    private static Card[] CreateCared(byte numOfTypes)
        => Enumerable.Range(0, numOfTypes - 1)
            .Select(i => new Card((byte)i))
            .ToArray();

    public IEnumerable<Card> Get(byte count)
        => Enumerable.Range(0, count - 1)
            .Select(i => _cards[i % _cards.Length]);
}