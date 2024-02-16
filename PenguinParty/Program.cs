using Network;
using Network.Node;
using Network.Node.Traits;
using Network.Rooms;
using PenguinParty.Repositories;
using Serilog;
using Util.Extensions;

namespace PenguinParty;

internal static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder();
        var config = builder.Services.AddConfig<PenguinPartyConfig>(args);
        builder.Services.AddSingleton(config.NetworkConfig);
        builder.Services.AddSingleton(new JwtDecoder(config.JwtKey));
        builder.Services.AddSingleton<CardRepository>();
        builder.Services.AddSingleton<IRoomRepository, PenguinPartyRoomRepository>();
        builder.Services.AddSingleton<IGameNode>(typeof(BasicGameNode), typeof(AuthTrait), typeof(RoomTrait));
        builder.Services.AddHostedService<Application>();
        var host = builder.Build();
        host.Run();
    }
}