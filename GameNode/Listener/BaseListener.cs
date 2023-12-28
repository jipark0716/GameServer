using System.Reflection;
using Boostrap.DI;
using Common.Extensions;
using Common.Queues;
using GameNode.Packets;

namespace GameNode.Listener;

public abstract class BaseListener
{
    private readonly Dictionary<ushort, MethodInfo> _actions;
    private readonly IQueue<ClientMessage> _queue;

    public BaseListener(IQueue<ClientMessage> queue)
    {
        _queue = queue;
        _actions = GetType()
            .GetMethods<GameActionAttribute>()
            .ToDictionary(o => o.GetCustomAttribute<GameActionAttribute>()?.ActionType ?? throw new());
    }

    private void Send(Session.Session target, ushort actionType, byte[] payload) => _queue.EnQueue(new()
    {
        SessionId = target.SessionId,
        ProtocolType = actionType,
        Payload = payload,
    });

    public void Send(IEnumerable<Session.Session> targets, ushort actionType, byte[] payload)
        => targets.Each(targets => Send(targets, actionType, payload));

    public void OnMessage(ushort actionType, byte[] payload, Session.Session session)
    {
        if (_actions.TryGetValue(actionType, out var action) is false)
        {
            return;
        }

        action.Invoke(this, action.GetParameters().Select(o => GetArgument(o, payload)).ToArray());
    }

    private object? GetArgument(ParameterInfo parameter, byte[] payload)
    {
        if (parameter.GetCustomAttribute<PayloadAttribute>() is not null)
        {
            // protobuf
            var parser = parameter.ParameterType.GetProperty("Parser").GetValue(null);
            if (parser is null) return null;

            var parseAction = parser.GetType().GetMethod("ParseFrom");
            if (parseAction is null) return null;

            return parseAction.Invoke(parseAction, [payload]);
        }

        return null;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class GameActionAttribute(ushort actionType) : Attribute
{
    public ushort ActionType { get; private set; } = actionType;
}

[AttributeUsage(AttributeTargets.Parameter)]
public class PayloadAttribute : Attribute
{
    
}