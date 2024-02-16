using Network.Exceptions;
using Network.Packets;
using Util.Extensions;

namespace Network.Node;

public class BasicGameNode : IGameNode
{
    public void OnDisconnect(Author author) {}

    public void OnConnect(Author author)
    {
        author.Socket.SendAsync(new HelloPacket(author.ConnectionId).Encapsulation(100));
    }

    public void OnMessage(Author author, ushort actionType, byte[] body)
    {
        throw new NotSupportActionException();
    }
}