using System.Reflection;
using System.Text.Json;
using Network.Attributes;
using Network.Exceptions;
using Network.Packets;

namespace Network.Rooms.Traits;

public abstract class BaseTrait : IRoom
{
    private delegate void Listener(Author author, byte[] body);
    public RoomState RoomState { get; }
    private readonly Dictionary<ushort, Listener> _actions;
    private readonly IRoom _room;

    protected BaseTrait(IRoom room, RoomState state)
    {
        RoomState = state;        
        _actions = [];
        _room = room;
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
            _ => throw new("not support parameter type"),
        };
    }
    
    public virtual bool Leave(Author author) => _room.Leave(author);

    public virtual void Join(Author author) => _room.Join(author);

    public void OnMessage(Author author, ushort actionType, byte[] body)
    {
        try
        {
            _room.OnMessage(author, actionType, body);
        }
        catch (NotSupportActionException)
        {
            if (_actions.TryGetValue(actionType, out var action) is false)
            {
                DefaultAction(author, actionType, body);
            }
            
            action?.Invoke(author, body);
        }
    }
    
    protected virtual void DefaultAction(Author author, ushort actionType, byte[] body)
        => throw new NotSupportActionException();
}