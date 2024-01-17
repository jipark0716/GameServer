using Serilog;

namespace Chat;

internal static class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true)
            .CreateLogger();

            
        ChatApplication application = new(1024, 7000);
        application.StartAsync().Wait();
        Console.ReadLine();
    }
}