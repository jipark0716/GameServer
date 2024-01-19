using Serilog;

namespace PenguinParty;

internal static class Program
{
    public static void Main(string[] args)
    {
        var a = new int[4];
        a[5] = 0;
        return;
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

            
        PenguinPartyApplication application = new(1024, 7000, new(5));
        application.StartAsync().Wait();
        Console.ReadLine();
    }
}