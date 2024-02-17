using Convenience.Enums;

namespace Convenience.Packets.Ping;

public class BasicPing : IPing
{
    public PingType Type => PingType.Basic;
    public required int X { get; init; }
    public required int Y { get; init; }
}