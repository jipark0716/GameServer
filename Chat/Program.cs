using Microsoft.Extensions.Hosting;
using Serilog;
using Util.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Chat;

internal static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();


        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddConfig<ChatConfig>(args);
        builder.Services.AddHostedService<ChatApplication>();
        var host = builder.Build();
        host.Run();
    }
}