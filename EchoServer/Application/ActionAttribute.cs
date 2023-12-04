using Boostrap.DI;
using System.Reflection.Metadata.Ecma335;

namespace EchoServer.Application
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionAttribute : Attribute
    {
        public readonly ushort Id;
        public ActionAttribute(ushort id)
        {
            Id = id;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class SocketControllerAttribute : LifeCycleAttribute
    {
    }
}
