using Network.Packets;
using Serilog;

namespace Network.EventListeners;

public class AuthorizeMiddleware : IClientMesssageMiddleware
{
    public void Run(OnClientMessageListener.Listener listener, Author author, byte[] body)
    {
        if (author.UserId is null)
        {
            Log.Information("인증 실패 connectionId:{connectionId}", author.ConnectionId);
            return;
        }
        listener(author, body);
    }
}