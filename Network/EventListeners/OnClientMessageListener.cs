using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using Network.Attributes;

namespace Network.EventListeners;

public class OnClientMessageListener(object instance)
{
    public object Instance { private get; set; } = instance; 
    
    private readonly Dictionary<ushort, Action<ulong, Socket, byte[]>> _actions = new();
    public Action<ushort, ulong, Socket, byte[]>? DefaultAction { private get; set; }
    
    public void OnMessage(ushort actionType, ulong id, Socket socket, byte[] payload)
    {
        if (_actions.TryGetValue(actionType, out var action) is false)
        {
            DefaultAction?.Invoke(actionType, id, socket, payload);
            return;
        }

        action.Invoke(id, socket, payload);
    }

    public void AddAction(ushort actionType, string methodName)
    {
        var methodInfo = Instance.GetType().GetMethod(methodName);
        ObjectDisposedException.ThrowIf(
            methodInfo is null,
            new Exception("not found action"));
        
        var parameters = methodInfo.GetParameters();
        
        _actions.Add(actionType, (id, socket, body) =>
        {
            var args = parameters.Select(o => GetArgument(o, id, socket, body));
            methodInfo.Invoke(Instance, args.ToArray());
        });
    }

    private object? GetArgument(ParameterInfo parameterInfo, ulong id, Socket socket, byte[] body)
    {
        var parameterAttribute = parameterInfo.GetCustomAttribute<ListenerParameterAttribute>();
        return parameterAttribute switch
        {
            IDAttribute => id,
            SocketAttribute => socket,
            JsonBodyAttribute => JsonSerializer.Deserialize(body, parameterInfo.ParameterType),
            _ => throw new("not support parameter type"),
        };
    }
}