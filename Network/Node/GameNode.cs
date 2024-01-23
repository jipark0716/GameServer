using Network.Packets;

namespace Network.Node;

public abstract class GameNode : IGameNode
{
    public abstract void OnDisconnect(Author author);
    public abstract void OnConnect(Author author);
    public abstract void OnMessage(Author author, ushort actionType, byte[] body);
}