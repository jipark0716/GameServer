﻿using Microsoft.AspNetCore.Builder;
using NetworkGateway.Websocket;
using NetworkGateway;

namespace GameNode;

internal class Program
{
    static void Main(string[] args)
    {
        new StartUp(args).Run();
        string input = Console.ReadLine();
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