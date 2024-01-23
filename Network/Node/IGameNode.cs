using Network.Packets;

namespace Network.Node;

public interface IGameNode
{
    public void OnDisconnect(Author author);
    public void OnConnect(Author author);
    public void OnMessage(Author author, ushort actionType, byte[] body);
}