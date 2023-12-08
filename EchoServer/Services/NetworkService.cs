using EchoServer.Dtos;
using EchoServer.Factories;
using EchoServer.Queues;
using System.Collections.Concurrent;
using EchoServer.Application;

namespace EchoServer.Services
{

    public abstract class NetworkService<T>
        where T : class, Dtos.ISession
    {
        private ulong _sessionIndex = 0;
        public ulong SessionIndex { get => ++_sessionIndex; }
        protected readonly ConcurrentDictionary<ulong, T> _sessions;
        protected readonly IPacketFactory _packetFactory;
        protected readonly DoubleBufferingQueue<Packet> _requestQueue;
        private readonly DoubleBufferingQueue<ResponsePacket> _responseQueue;
        private readonly TopicService _topicService;
        private readonly ISocketApplication _application;

        public NetworkService(IPacketFactory packetFactory, ISocketApplication application, TopicService topicService)
        {
            _packetFactory = packetFactory;
            _sessions = new();
            _requestQueue = new();
            _responseQueue = new();
            _application = application;
            _topicService = topicService;
            _application.Init(_responseQueue);

            new Thread(ExecuteRequest).Start();
            new Thread(ExecuteResponse).Start();
        }

        private void ExecuteRequest()
        {
            foreach (var packet in _requestQueue)
            {
                switch (packet.Type)
                {
                    case PacketType.Close:
                        RemoveSession(packet.Session);
                        break;
                    case PacketType.Basic:
                        _application.OnMessage(packet);
                        break;
                }
            }
        }

        private void ExecuteResponse()
        {
            foreach (var packet in _responseQueue)
            {
                switch (packet.Target)
                {
                    case BroadcastTarget:
                        SendResponse(_sessions.Keys, packet.Payload);
                        break;
                    case UserTarget target:
                        SendResponse(target.Uid, packet.Payload);
                        break;
                    case TopicTarget target:
                        SendResponse(_topicService.GetOrDefault(target.TopicId), packet.Payload);
                        break;
                }
            }
        }

        protected void SendResponse(IEnumerable<ulong> receivers, byte[] payload)
        {
            foreach (ulong receiver in receivers)
            {
                SendResponse(receiver, payload);
            }
        }

        protected abstract void SendResponse(ulong receiver, byte[] payload);

        private void RemoveSession(Dtos.ISession session)
        {
            _sessions.Remove(session.Uid, out _);
            _topicService.UnSubscript(session);
        }

        public virtual Task AddSession(T session)
        {
            _sessions.TryAdd(session.Uid, session);
            return Task.CompletedTask;
        }
    }
}
