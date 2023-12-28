using Microsoft.AspNetCore.Builder;
using NetworkGateway.Websocket;
using NetworkGateway;

namespace GameNode.PenguinParty;

internal class Program
{
    static void Main(string[] args)
    {
        A a = new();
        B b = new();
        b.AddAction(a);
        b = new();
        a.Action();
        return;
        Console.WriteLine("start");
        new StartUp(args).Run();
        string input = Console.ReadLine();
    }
}

public class B
{
    public void AddAction(A a)
    {
        a.Actions += () => Action(3);
    }

    public void Action(int i)
    {
        Console.WriteLine("GLKJASd");
    }
}

public class A
{
    public event Action Actions;
    public void Action()
    {
        Actions?.Invoke();
    }
}

internal class StartUp(string[] args)
{
    public void Run()
    {
        Task.Run(StartGateway);
        Task.Run(StartGameNode);
    }

    public void StartGameNode()
    {
        Thread.Sleep(TimeSpan.FromSeconds(3));
        Client client = new(1);
        client.AddEventListener(o => new GameListener(o));
        client.Start();
    }


    public void StartGateway()
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        app.UseWebSockets();

        WebsocketConnectionService websocketConnectionService = new(new());
        app.Map("/", websocketConnectionService.Listener);
        Task.Run(() =>
        {
            new Server(websocketConnectionService).Start();
        });
        app.Run();
    }
}