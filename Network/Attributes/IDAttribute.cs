namespace Network.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public abstract class ListenerParameterAttribute : Attribute
{
}

public class AuthorAttribute : ListenerParameterAttribute
{
}

public class JsonBodyAttribute : ListenerParameterAttribute
{
}