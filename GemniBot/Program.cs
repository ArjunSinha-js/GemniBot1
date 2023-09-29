using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GemniBot;
using Newtonsoft.Json;    using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;


class Program
{


    private CommandService _commands;

    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
         _serviceProvider = CreateProvider();
    }
    static IServiceProvider CreateProvider()
    {

        var config = new DiscordSocketConfig()
        {
            GatewayIntents = GatewayIntents.All
        };
        var collection = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandler>()
            .AddSingleton<TicketService>();
        collection.AddDbContext<BloggingContext>();
        
        //...
        return collection.BuildServiceProvider();
    }
    
    public static async Task Main(string[] args)
    {
        
        await new Program().MainAsync();
        
    }

    public async Task StartBot()
    {

        string json = await File.ReadAllTextAsync("token.json");
        TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(json)!;
        string token = tokenData.Token;

        
        var _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();

        _client.Log += Log;

        _commands = _serviceProvider.GetRequiredService<CommandService>();
        
            
        var commandHandler = _serviceProvider.GetRequiredService<CommandHandler>();
        await commandHandler.InstallCommandsAsync();
        commandHandler.SetTextChannelPrefix("!");
        

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    public async Task MainAsync()
    {
        await StartBot();
        

    }
    private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        // If the message was not in the cache, downloading it will result in getting a copy of `after`.
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"{message} -> {after}");
    }
    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }


  
}