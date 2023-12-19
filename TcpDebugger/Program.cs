using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace TcpDebugger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseElectron(args);
            Run(builder).Wait();
        }

        private static async Task Run(WebApplicationBuilder builder)
        {
            // Is optional, but you can use the Electron.NET API-Classes directly with DI (relevant if you wont more encoupled code)
            builder.Services.AddElectron();

            var app = builder.Build();

            await app.StartAsync();

            // Open the Electron-Window here
            await Electron.WindowManager.CreateWindowAsync();

            app.WaitForShutdown();
        }
    }
}
