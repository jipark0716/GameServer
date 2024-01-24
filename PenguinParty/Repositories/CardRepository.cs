using PenguinParty.Dto;

namespace PenguinParty.Repositories;

public class CardRepository(byte numOfTypes = 5)
{
    private readonly Card[] _cards = CreateCared(numOfTypes);

    private static Card[] CreateCared(byte numOfTypes)
        => Enumerable.Range(0, numOfTypes)
            .Select(i => new Card((byte)i))
            .ToArray();

    public IEnumerable<Card> Get(byte count)
        => Enumerable.Range(0, count)
            .Select(i => _cards[i % _cards.Length]);
}