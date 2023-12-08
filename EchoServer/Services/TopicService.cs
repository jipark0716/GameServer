using Boostrap.DI;
using ConcurrentCollections;
using System.Collections.Concurrent;

namespace EchoServer.Services
{
    [LifeCycle(LifeCycle.Singleton)]
    public class TopicService
    {
        public delegate void OnCloseHandler(ulong topicId);
        public event OnCloseHandler? OnClose = null;

        private readonly ConcurrentDictionary<ulong, ConcurrentHashSet<ulong>> _topics;
        private ulong _topicSequence = 0;
        public ulong TopicSequence { get => ++_topicSequence; }

        public TopicService()
        {
            _topics = new();
        }

        public IEnumerable<ulong> GetOrDefault(ulong key, IEnumerable<ulong>? defaultValue = null)
        {
            if (_topics.TryGetValue(key, out var topic))
            {
                return topic;
            }
            return defaultValue ?? Array.Empty<ulong>();
        }

        public ulong Subscript(Dtos.ISession session)
        {
            var topicId = TopicSequence;
            Subscript(session, topicId);
            return topicId;
        }

        public void Subscript(Dtos.ISession session, ulong topicId)
        {
            var topic = _topics.GetOrAdd(topicId, id => new());
            topic.Add(session.Uid);
        }

        public void UnSubscript(Dtos.ISession session)
        {
            foreach (var topic in _topics)
            {
                topic.Value.TryRemove(session.Uid);

                if (topic.Value.Count == 0)
                {
                    CloseTopic(topic.Key);
                }
            }
        }

        public void UnSubscript(Dtos.ISession session, ulong topicId)
        {
            if (_topics.TryGetValue(topicId, out var topic) is false)
            {
                return;
            }

            topic.TryRemove(session.Uid);

            if (topic.Count == 0)
            {
                CloseTopic(topicId);
            }
        }

        public void CloseTopic(ulong topicId)
        {
            _topics.TryRemove(topicId, out _);
            if (OnClose is not null)
            {
                OnClose(topicId);
            }
        }
    }
}
