namespace PenguinParty.Dto;

public class Player(ulong userId)
{
    public readonly ulong UserId = userId;
    public List<Card> Cards { get; } = [];
    public byte Score;
}