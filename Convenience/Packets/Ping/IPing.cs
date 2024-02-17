using System.Text.Json.Serialization;
using Convenience.Enums;

namespace Convenience.Packets.Ping;

[JsonDerivedType(typeof(BasicPing), 0)]
[JsonDerivedType(typeof(PlayerPing), 1)]
public interface IPing
{
    public PingType Type { get; }
}