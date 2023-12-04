using Boostrap.DI;
using ConcurrentCollections;
using System.Collections.Concurrent;

namespace EchoServer.Services
{
    [LifeCycle(LifeCycle.Scoped)]
    public class TopicService
    {
        private readonly ConcurrentDictionary<ulong, ConcurrentHashSet<ulong>> _topics;

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
                    _topics.TryRemove(topic.Key, out _);
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
                _topics.TryRemove(topicId, out _);
            }
        }
    }
}
