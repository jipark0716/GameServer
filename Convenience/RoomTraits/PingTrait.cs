using Convenience.Packets.Ping;
using Network.Attributes;
using Network.Packets;
using Network.Rooms.Traits;
using Network.Rooms;
using Util.Extensions;

namespace Convenience.RoomTraits;

public class PingTrait(IRoom room, RoomState state): BaseTrait(room, state)
{
    [Action(4000)]
    public void AddPing([Author] Author author, [JsonBody] PingRequest request)
        => RoomState.Broadcast(new PingResponse
        {
            Ping = request.Ping,
            UserId = (ulong)author.UserId!
        }.Encapsulation(4000));
}