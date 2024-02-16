using Convenience.Enums;

namespace Convenience.Packets.Ping;

public class PlayerPing: IPing
{
    public PingType Type => PingType.Player;
    public required ulong UserId { get; init; }
}