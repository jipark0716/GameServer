using System.Reflection;
using System.Text.Json;
using Network.Attributes;
using Network.Exceptions;
using Network.Packets;
using Util.Extensions;

namespace Network.Node.Traits;

public abstract class BaseTrait : IGameNode
{
    private delegate void Listener(Author author, byte[] body);
    private readonly IGameNode _node;
    private readonly Dictionary<ushort, Listener> _actions;
    private readonly JwtDecoder _jwtDecoder;
    
    protected BaseTrait(
        IGameNode node,
        JwtDecoder jwtDecoder)
    {
        _jwtDecoder = jwtDecoder;
        _node = node;
        _actions = [];
        foreach (var method in GetType().GetMethods())
        {
            switch (method.GetCustomAttribute<ActionAttribute>())
            {
                case { } attribute:
                    _actions.Add(attribute.Type, (author, body) =>
                        method.Invoke(this, GetArguments(method, author, body)));
                    break;
            }
        }
    }

    private object?[] GetArguments(MethodInfo method, Author author, byte[] body)
        => method.GetParameters().Select(o => GetArgument(o, author, body)).ToArray();

    private object? GetArgument(ParameterInfo parameterInfo, Author author, byte[] body)
    {
        var parameterAttribute = parameterInfo.GetCustomAttribute<ListenerParameterAttribute>();
        return parameterAttribute switch
        {
            AuthorAttribute => author,
            JsonBodyAttribute => JsonSerializer.Deserialize(body, parameterInfo.ParameterType),
            JwtAttribute => _jwtDecoder.Decode(body).Serialize(parameterInfo.ParameterType),
            _ => throw new("not support parameter type"),
        };
    }

    
    public virtual void OnDisconnect(Author author) => _node.OnDisconnect(author);

    public virtual void OnConnect(Author author) => _node.OnConnect(author);

    public void OnMessage(Author author, ushort actionType, byte[] body)
    {
        try
        {
            _node.OnMessage(author, actionType, body);
        }
        catch (NotSupportActionException)
        {
            if (_actions.TryGetValue(actionType, out var action) is false)
            {
                DefaultAction(author, actionType, body);
                return;
            }
            
            action.Invoke(author, body);
        }
    }

    protected virtual void DefaultAction(Author author, ushort actionType, byte[] body)
        => throw new NotSupportActionException();
}