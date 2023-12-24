using ElectronNET.API;

namespace TcpDebugger;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseElectron(args);
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapControllers();
            //endpoints.MapRazorPages();

            if (HybridSupport.IsElectronActive)
            {
                CreateWindow();
            }
        });
    }

    private async void CreateWindow()
    {
        var window = await Electron.WindowManager.CreateWindowAsync();
        window.OnClosed += () =>
        {
            Electron.App.Quit();
        };
    }
}


//internal class Program
//{
//    public static void Main(string[] args)
//    {
//        new StartUp().Run(args).Wait();
//    }
//}

//internal class StartUp
//{
//    public async Task Run(string[] args)
//    {
//        var builder = WebApplication.CreateBuilder(args);

//        builder.Services.AddControllers();
//        builder.Services.AddEndpointsApiExplorer();
//        builder.Services.AddSwaggerGen();
//        builder.WebHost.UseElectron(args);
//        builder.Services.AddElectron();

//        var app = builder.Build();

//        if (app.Environment.IsDevelopment())
//        {
//            app.UseSwagger();
//            app.UseSwaggerUI();
//        }

//        app.UseHttpsRedirection();
//        app.UseAuthorization();
//        app.MapControllers();

//        // app.Run();
//        await app.StartAsync();

//        await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions(), "http://localhost:5272");

//        app.WaitForShutdown();
//    }
//}