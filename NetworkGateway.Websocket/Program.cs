
namespace NetworkGateway.Websocket
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();
            app.UseWebSockets();

            WebsocketConnectionService websocketConnectionService = new();
            app.Map("/", websocketConnectionService.Listener);
            app.Run();
        }
    }
}
