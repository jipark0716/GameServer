using Serilog;
using Util.Entity.Context;
using Util.Extensions;

namespace PenguinParty;

internal static class Program
{
    public static void Main(string[] args)
    {Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddDbContext<GameContext>();
        var config = builder.Services.AddConfig<PenguinPartyConfig>(args);
        builder.Services.AddDbContextPool<GameContext>(config.Database.Game);
        builder.Services.AddHostedService<PenguinPartyApplication>();
        var host = builder.Build();
        host.Run();
    }
}