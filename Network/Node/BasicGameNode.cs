using Network.Exceptions;
using Network.Packets;
using Util.Extensions;

namespace Network.Node;

public class BasicGameNode : GameNode
{
    public override void OnDisconnect(Author author) {}

    public override void OnConnect(Author author)
    {
        author.Socket.SendAsync(new HelloPacket(author.ConnectionId).Encapsulation(100));
    }

    public override void OnMessage(Author author, ushort actionType, byte[] body)
    {
        throw new NotSupportActionException();
    }
}