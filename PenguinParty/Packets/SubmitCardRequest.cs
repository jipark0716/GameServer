namespace PenguinParty.Packets;

public class SubmitCardRequest
{
    public required byte X { get; init; }
    public required byte Y { get; init; }
    public required byte CardIndex { get; init; }
}