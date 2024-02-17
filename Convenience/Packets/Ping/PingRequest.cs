using System.Text.Json.Nodes;

namespace Convenience.Packets.Ping;

public class PingRequest
{
    public required IPing Ping { get; init; }
}