using Network;
using Network.Node;
using Network.Rooms;
using PenguinParty.Repositories;
using Util.Extensions;

namespace PenguinParty;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();
        var config = builder.Services.AddConfig<PenguinPartyConfig>(args);
        builder.Services.AddSingleton(config.NetworkConfig);
        builder.Services.AddSingleton<Chat.NodeFactory>();
        builder.Services.AddSingleton(new JwtDecoder(config.JwtKey));
        builder.Services.AddSingleton<CardRepository>();
        builder.Services.AddSingleton<IRoomRepository, PenguinPartyRoomRepository>();
        builder.Services.AddSingleton<IGameNode>(o => o.GetService<Chat.NodeFactory>()?.Create()!);
        builder.Services.AddHostedService<Application>();
        var host = builder.Build();
        host.Run();
    }
}