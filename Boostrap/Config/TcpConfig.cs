using System.Net;

namespace Boostrap.Config;

public class TcpConfig
{
    public readonly IPEndPoint IPEndPoint = new(IPAddress.Any, 5243);
    public readonly int BackLog = 100;
    public readonly bool IsDelay = false;
}
