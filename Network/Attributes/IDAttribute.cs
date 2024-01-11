namespace Network.Attributes;

[AttributeUsage(AttributeTargets.Parameter)]
public abstract class ListenerParameterAttribute : Attribute
{
}

public class IDAttribute : ListenerParameterAttribute
{
}

public class SocketAttribute : ListenerParameterAttribute
{
}

public class JsonBodyAttribute : ListenerParameterAttribute
{
}