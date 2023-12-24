using NetworkGateway.Connection;
using System.Net.WebSockets;

namespace NetworkGateway.Websocket;

public class WebsocketConnectionService(WebsocketConnectionFactory websocketConnectionFactory) : AConnectionService
{
    private readonly WebsocketConnectionFactory _websocketConnectionFactory = websocketConnectionFactory;

    public async Task Listener(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest is false)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var connection = _websocketConnectionFactory.Create(webSocket);
        AddConnection(connection);
        await ReceiveMessage(connection);
    }
    
    private async Task ReceiveMessage(WebsocketConnection connection)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult receiveResult;

        do
        {
            receiveResult = await connection.Websocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (receiveResult.EndOfMessage is false)
            {
                break;
            }
            connection.OnMessage(buffer[..receiveResult.Count]);
        } while (!receiveResult.CloseStatus.HasValue);

        connection.Close();
    }
}
