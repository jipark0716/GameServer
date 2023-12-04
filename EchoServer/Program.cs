
using Boostrap.DI;
using EchoServer.Services;

namespace EchoServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.DI();
            var app = builder.Build();
            app.UseWebSockets();
            var server = app.Services.CreateScope().ServiceProvider.GetRequiredService<WebsocketService>();
            app.Map("/", server.GetLIsnter());
            app.Run();
        }
    }
}