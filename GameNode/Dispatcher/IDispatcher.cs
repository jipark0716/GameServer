using GameNode.Packets;

namespace GameNode.Dispatcher;

public interface IDispatcher
{
    public void Start();
    public void Produce(ClientMessage clientMessage);
}
