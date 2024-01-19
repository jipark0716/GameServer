namespace PenguinParty.Packets;

public class SubmitCardRequest
{
    public required byte X { get; set; }
    public required byte Y { get; set; }
    public required byte CardIndex { get; set; }
}