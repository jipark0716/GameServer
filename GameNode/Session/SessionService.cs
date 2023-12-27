using Common.Extensions;
using Common.Queues;
using System.Collections.Concurrent;

namespace GameNode.Session;

public class SessionService(TimeSpan delay)
{
    public SessionService() : this(TimeSpan.FromMinutes(5)) { }

    private readonly ConcurrentDictionary<ulong, Session> _sessions = new();
    private readonly DelayedQueue<Session> _removeQueue = new(delay);

    public void Start() => _removeQueue.Each(RemoveSession);

    public void AddSession(ulong sessionId, byte[] payload)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
        {
            // 재연결
            session.Reconnect();
        }
        else
        {
            // 처음 연결
            _sessions.TryAdd(sessionId, new()
            {
                SessionId = sessionId,
                UserId = BitConverter.ToUInt64(payload.AsSpan()[..8]),
            });
        }
    }

    public void DisconnectSession(ulong sessionId)
    {
        var session = GetSession(sessionId);
        session.Disconnect();
        _removeQueue.EnQueue(session);
    }

    public void RemoveSession(Session session)
    {
        if (session.Remove())
        {
            _sessions.TryRemove(session.SessionId, out _);
        }
    }

    public Session GetSession(ulong sessionId)
    {
        return _sessions.GetValueOrDefault(sessionId) ?? throw new Exception("session 누락");
    }
}
