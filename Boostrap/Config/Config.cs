using Boostrap.DI;
using System.Net;

namespace Boostrap.Config;

[LifeCycle(LifeCycle.Singleton)]
public class Config
{
    public readonly TcpConfig Tcp = new();
}
