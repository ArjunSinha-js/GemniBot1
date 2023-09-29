using System.Reflection;
using System.Threading.Channels;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using OpenAI_API;

namespace GemniBot;



public class CommandHandler
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private string _textChannelPrefix = "!";
    private IServiceProvider _serviceProvider;


    public void SetTextChannelPrefix(string prefix)
    {
        _textChannelPrefix = prefix;
    }

    public CommandHandler(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
    {
        _commands = commands;
        _serviceProvider = serviceProvider;
        _client = client;
    }
    
    public async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;
        
        
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),  services: _serviceProvider);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {

        var message = messageParam as SocketUserMessage;
        Console.WriteLine($"Message chanel: {message.Channel}");
        Console.WriteLine($"Message: {message}");
        if (message is null) return;

        
        
        int argPos = 0;
        
        if (!(message.HasStringPrefix(_textChannelPrefix, ref argPos) || 
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;


        var context = new SocketCommandContext(_client, message);

        await _commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: _serviceProvider);
    }
}


public class InfoModule : ModuleBase<SocketCommandContext>
{

    

    
    [Command("say")]
    [Summary("Echoes a message.")]
    public Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
    {

        ReplyAsync(echo);
        Console.WriteLine($"Sent message: {echo}");
        return Task.CompletedTask;
    }
    
    
}
[Group("sample")]
public class SampleModule : ModuleBase<SocketCommandContext>
{

    [Command("square")]
    [Summary("Squares a number.")]
    public async Task SquareAsync(
        [Summary("The number to square.")] 
        int num)
    {

        await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
    }


    [Command("userinfo")]
    [Summary
        ("Returns info about the current user, or the user parameter, if one passed.")]
    [Alias("user", "whois")]
    public async Task UserInfoAsync(
        [Summary("The (optional) user to get info from")]
        SocketUser user = null)
    {
        var userInfo = user ?? Context.Client.CurrentUser;
        await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}