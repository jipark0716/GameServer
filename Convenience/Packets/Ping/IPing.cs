using Convenience.Enums;

namespace Convenience.Packets.Ping;

public interface IPing
{
    public PingType Type { get; }
}