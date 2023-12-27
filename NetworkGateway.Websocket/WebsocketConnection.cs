using NetworkGateway.Connection;
using System.Net.WebSockets;

namespace NetworkGateway.Websocket;

public class WebsocketConnection(ulong connectionId, WebSocket websocket) : BaseConnection(connectionId)
{
    public readonly WebSocket Websocket = websocket;

    public override void Close()
    {
        base.Close();
        Websocket.Dispose();
    }

    public override void Send(byte[] payload)
    {
        Websocket.SendAsync(
            new ArraySegment<byte>(payload, 0, payload.Length),
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
    }
}
