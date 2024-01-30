namespace PenguinParty.Dto;

public class Player
{
    public required ulong UserId { get; init; }
    public List<Card> Cards { get; } = [];
    public int Score { get; set; }
}