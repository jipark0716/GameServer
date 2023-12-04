using Boostrap.DI;
using EchoServer.Dtos;

namespace EchoServer.Services
{
    [LifeCycle(LifeCycle.Scoped)]
    public class WebsocketService
    {
        private readonly WebsocketNetworkService _networkService;

        public WebsocketService(WebsocketNetworkService networkService)
        {
            _networkService = networkService;
        }

        public RequestDelegate GetLIsnter()
        {
            return Listener;
        }

        public async Task Listener(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest is false)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            WebsocketSession session = new(_networkService.SessionIndex, webSocket);
            await _networkService.AddSession(session);
        }
    }
}
