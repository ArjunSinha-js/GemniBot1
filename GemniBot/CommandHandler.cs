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

    public void SetTextChannelPrefix(string prefix)
    {
        _textChannelPrefix = prefix;
    }

    public CommandHandler(DiscordSocketClient client, CommandService commands)
    {
        _commands = commands;
        _client = client;
    }
    
    public async Task InstallCommandsAsync()
    {
        // Hook the MessageReceived event into our command handler
        _client.MessageReceived += HandleCommandAsync;
        
        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),  services: null);
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {

        var message = messageParam as SocketUserMessage;
        Console.WriteLine($"Message chanel: {message.Channel}");
        Console.WriteLine($"Message: {message}");
        if (message == null) return;

        
        
        int argPos = 0;
        
        if (!(message.HasStringPrefix(_textChannelPrefix, ref argPos) || 
              message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
            return;


        var context = new SocketCommandContext(_client, message);

        await _commands.ExecuteAsync(
            context: context, 
            argPos: argPos,
            services: null);
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

    [Command("ask")]
    [Summary("Answering users question")]

    public async Task AskAsync([Remainder] [Summary("The text to echo")] string echo)
    {
        string json = await File.ReadAllTextAsync("key.json");
        Key openAiKey = JsonConvert.DeserializeObject<Key>(json)!;
        string key = openAiKey.OpenAIKey;

        OpenAIAPI api = new OpenAIAPI(new APIAuthentication(key));
        var chat = api.Chat.CreateConversation();
        chat.AppendSystemMessage("You are a walking wikipedia, but a bit angry, just give shord and bit agressive answers for users");
        chat.AppendUserInput("How much does iphone 13 cost?");
        chat.AppendExampleChatbotOutput("According to Apple.com Iphone 13 costs 999$");
        chat.AppendUserInput("Is this an animal? House");
        chat.AppendExampleChatbotOutput("No");
        chat.AppendUserInput(echo);
        string answer = await chat.GetResponseFromChatbotAsync();
        await ReplyAsync(answer);
    }

    [Command("tell")]
    [Summary("Tell the short version of text")]

    public async Task Telling([Remainder] [Summary("Tell the short version of text")] string echo)
    {
        string json = await File.ReadAllTextAsync("key.json");
        Key openAiKey = JsonConvert.DeserializeObject<Key>(json)!;
        string key = openAiKey.OpenAIKey;

        OpenAIAPI api = new OpenAIAPI(new APIAuthentication(key));
        var chat = api.Chat.CreateConversation();
        chat.AppendSystemMessage("You are an assistant moderator. You get messages from users, and you need to summarize what the person in the message is saying in a concise, to the point and without water");
        chat.AppendUserInput("I would like to draw your attention to an unpleasant situation related to the withdrawal of funds. At the moment my account on the site shows a write-off of UAH 200, but these funds have not arrived to my bank account."
+" The details of the transaction are as follows: Amount: UAH 200         Date: 23.09.2023         Transaction Number: TRX123456789"
+ " Please clarify this situation and help me to return the funds to my bank account. If you need additional information, please let me know and I will provide all the necessary data.");
        chat.AppendExampleChatbotOutput("Problem: cannot withdrawal. Amount: 200UAH. Date: 23.09.2023. Transaction Number: TRX123456789"
        +"Retelled message text: I'm facing an issue with a withdrawal of UAH 200. My account shows the deduction, but the funds haven't reached my bank account. "
        +"Please investigate and assist in returning the funds to my bank account. I can provide more information if needed.");
        chat.AppendUserInput(echo);
        string answer = await chat.GetResponseFromChatbotAsync();
        await ReplyAsync(answer);

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