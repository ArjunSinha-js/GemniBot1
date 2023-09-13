using Discord;
using Discord.Commands;
using Discord.WebSocket;
using GemniBot;
using Newtonsoft.Json;

class Program
{

    private CommandService _commands;
    private DiscordSocketClient _client;
    
    public static async Task Main(string[] args)
    {
        await new Program().MainAsync();
    }

    public async Task StartBot()
    {
        string json = await File.ReadAllTextAsync("token.json");
        TokenData tokenData = JsonConvert.DeserializeObject<TokenData>(json)!;
        string token = tokenData.Token;

        
        _client = new DiscordSocketClient();
        _client.Log += Log;

        _commands = new CommandService();
        
        var commandHandler = new CommandHandler(_client, _commands);
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