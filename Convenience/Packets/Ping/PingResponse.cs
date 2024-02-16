namespace Convenience.Packets.Ping;

public class PingResponse
{
    public required IPing Ping { get; init; }
    public required ulong UserId { get; init; }
}