namespace Network.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public abstract class ListenerParameterAttribute : Attribute;

public class AuthorAttribute : ListenerParameterAttribute;

public class JsonBodyAttribute : ListenerParameterAttribute;

public class JwtAttribute : ListenerParameterAttribute;

[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute(ushort type) : Attribute
{
    public readonly ushort Type = type;
}