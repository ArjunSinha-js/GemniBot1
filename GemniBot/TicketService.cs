using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GemniBot;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft;


public class TicketService : ModuleBase<SocketCommandContext>
{

    private readonly BloggingContext _database;
    
    public TicketService(BloggingContext database)
    {
        _database = database;
    }

    [Command("createchannel")]
    public async Task CreateTextChannelAsync()
    {
        
        Console.WriteLine("Start");
        
        string json = await File.ReadAllTextAsync("botconfig.json");

        Console.WriteLine(json);
        
        var config = JsonConvert.DeserializeObject<BotConfig>(json)!;
        
        var guild = Context.Client.GetGuild(config.Server);
        
        var category = guild.GetCategoryChannel(config.Category);
        
        Console.WriteLine($"{guild} - Server Name");
        
        Console.WriteLine($"{category} - Category");

        var userFromDb = await _database.Users.FirstOrDefaultAsync(x => x.Id == Context.User.Id);
        Console.WriteLine($"id in database {userFromDb}");
        if (userFromDb is null)
        {
            
            var textChannel = await guild.CreateTextChannelAsync($"Ticket from {Context.Message.Author}", properties =>
            {
                properties.CategoryId = category.Id;
            });
            
            await _database.AddAsync(new User(){Id = Context.User.Id, ChatId = textChannel.Id});
            
                await _database.SaveChangesAsync();
                
            Console.WriteLine("Saved");

            await textChannel.SendMessageAsync($"Here is new text channel {textChannel.Id}");
            await ReplyAsync($"Text channel '{textChannel.Name}' has been created!");
            return;
        }

       var textChannelRes = guild.GetTextChannel(userFromDb.ChatId);
       await textChannelRes.SendMessageAsync("Welcome to channel");
  






    }

    [Command("s")]

    public async Task SentMessageInChannel([Remainder] string echo)
    {
        Console.WriteLine("Start");

        var userFromDb = await _database.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Context.User.Id);
        
        Console.WriteLine($"Id from Database - {userFromDb}");
        
        string json = await File.ReadAllTextAsync("botconfig.json");

        Console.WriteLine(json);
        
        var config = JsonConvert.DeserializeObject<BotConfig>(json)!;
        
        var guild = Context.Client.GetGuild(config.Server);
        
        var category = guild.GetCategoryChannel(config.Category);

        if (userFromDb != null)
        {
            var textChannel = guild.GetTextChannel(userFromDb.ChatId);
            await textChannel.SendMessageAsync(echo);
        }
    }


}

public class TicketMethods
{
    
}