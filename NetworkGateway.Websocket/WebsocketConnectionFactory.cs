using NetworkGateway.Connection;
using System.Net.WebSockets;

namespace NetworkGateway.Websocket
{
    public class WebsocketConnectionFactory : AConnectionFactory
    {
        public WebsocketConnection Create(WebSocket websocket) => new(ConnectionIdSequence, websocket);
    }
}
