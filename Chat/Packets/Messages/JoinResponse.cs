using Network.Packets.Room;

namespace Chat.Packets.Messages;

public class JoinResponse(RoomDto roomDto, List<Message> messages)
    : Network.Packets.Room.JoinResponse(roomDto)
{
    public List<Message> Messages { get; } = messages;
}