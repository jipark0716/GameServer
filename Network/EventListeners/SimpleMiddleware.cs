using Network.Packets;

namespace Network.EventListeners;

public class SimpleMiddleware(Func<Author, bool> action) : IClientMesssageMiddleware
{
    public void Run(OnClientMessageListener.Listener listener, Author author, byte[] body)
    {
        if (action(author))
        {
            listener(author, body);
        }
    }
}