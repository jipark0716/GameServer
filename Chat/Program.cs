using System.Security.Cryptography;

namespace Chat;

internal static class Program
{
    public static void Main(string[] args)
    {
        ChatApplication application = new(1024, 7000);
        application.StartAsync().Wait();
        Console.ReadLine();
    }
}