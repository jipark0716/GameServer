using ElectronNET.API;

namespace TcpDebugger;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.WebHost.UseElectron(args);
        builder.Services.AddElectron();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        // app.Run();
        app.StartAsync().Wait();

        Electron.WindowManager.CreateWindowAsync("https://localhost:7161").Wait();

        app.WaitForShutdown();

    }
}
