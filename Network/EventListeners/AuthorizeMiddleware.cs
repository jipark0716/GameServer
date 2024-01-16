using Network.Packets;

namespace Network.EventListeners;

public class AuthorizeMiddleware : IClientMesssageMiddleware
{
    public void Run(OnClientMessageListener.Listener listener, Author author, byte[] body)
    {
        if (author.UserId is null)
        {
            return;
        }
        listener(author, body);
    }
}