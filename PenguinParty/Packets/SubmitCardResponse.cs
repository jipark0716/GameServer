using PenguinParty.Dto;

namespace PenguinParty.Packets;

public class SubmitCardResponse
{
    public required byte X { get; init; }
    public required byte Y { get; init; }
    public required Card Card { get; init; }
    public required ulong UserId { get; init; }
}