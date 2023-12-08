using Boostrap.DI;
using EchoServer.Dtos;
using EchoServer.Queues;

namespace EchoServer.Application
{
    [LifeCycle(LifeCycle.Scoped, typeof(JsonSocketApplication))]
    public interface ISocketApplication
    {
        public void Init(DoubleBufferingQueue<ResponsePacket> responseQueue);
        public void OnMessage(Packet packet);
    }
}
