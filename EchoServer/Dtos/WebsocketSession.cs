using System.Net.WebSockets;

namespace EchoServer.Dtos
{
    public class WebsocketSession : Session
    {
        public readonly WebSocket Socket;

        public WebsocketSession(ulong uid, WebSocket socket) : base(uid)
        {
            Socket = socket;
        }
    }
}
