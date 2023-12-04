using Boostrap.DI;
using EchoServer.Dtos;
using EchoServer.Queues;
using EchoServer.Services;

namespace EchoServer.Application
{
    [LifeCycle(LifeCycle.Scoped, typeof(JsonSocketApplication))]
    public interface ISocketApplication
    {
        public void Init(DoubleBufferingQueue<ResponsePacket> responseQueue, TopicService _topicService);
        public void OnMessage(Packet packet);
    }
}
