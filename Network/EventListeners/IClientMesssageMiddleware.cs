using Network.Packets;

namespace Network.EventListeners;

public interface IClientMesssageMiddleware
{
    public void Run(OnClientMessageListener.Listener listener, Author author, byte[] body);
}