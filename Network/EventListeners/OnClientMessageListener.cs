using System.Reflection;
using System.Text.Json;
using Network.Attributes;
using Network.Packets;

namespace Network.EventListeners;

public class OnClientMessageListener(object instance)
{
    public delegate void Listener(Author author, byte[] body);
    
    public object Instance { private get; set; } = instance; 
    
    private readonly Dictionary<ushort, Listener> _actions = new();
    public Action<ushort, Author, byte[]>? DefaultAction { private get; set; }
    
    public void OnMessage(ushort actionType, Author author, byte[] payload)
    {
        if (_actions.TryGetValue(actionType, out var action) is false)
        {
            DefaultAction?.Invoke(actionType, author, payload);
            return;
        }

        action.Invoke(author, payload);
    }

    public void AddAction(ushort actionType, string methodName, params IClientMesssageMiddleware[] middlewares)
    {
        var methodInfo = Instance.GetType().GetMethod(methodName);
        ObjectDisposedException.ThrowIf(
            methodInfo is null,
            new Exception("not found action"));
        
        var parameters = methodInfo.GetParameters();


        Listener action = (author, body) =>
        {
            var args = parameters.Select(o => GetArgument(o, author, body));
            methodInfo.Invoke(Instance, args.ToArray());
        };
        foreach (var middleware in middlewares.Reverse())
        {
            var copiedAction = action;
            action = (author, body) => middleware.Run(copiedAction, author, body);
        }
        _actions.Add(actionType, action);
    }

    private object? GetArgument(ParameterInfo parameterInfo, Author author, byte[] body)
    {
        var parameterAttribute = parameterInfo.GetCustomAttribute<ListenerParameterAttribute>();
        return parameterAttribute switch
        {
            AuthorAttribute => author,
            JsonBodyAttribute => JsonSerializer.Deserialize(body, parameterInfo.ParameterType),
            _ => throw new("not support parameter type"),
        };
    }
}