using PenguinParty.Dto;

namespace PenguinParty.Packets;

public class SubmitCardResponse(byte x, byte y, Card card)
{
    public byte X { get; set; } = x;
    public byte Y { get; set; } = y;
    public Card Card { get; set; } = card;
}