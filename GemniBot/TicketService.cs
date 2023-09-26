using Newtonsoft.Json;

namespace GemniBot;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft;


public class TicketService : ModuleBase<SocketCommandContext>
{
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
     
        string usersJson = await File.ReadAllTextAsync("users.json");
        Console.WriteLine(usersJson);
        var users = JsonConvert.DeserializeObject<List<User>>(usersJson)!;
        Console.WriteLine(users);
        ulong result = 0;
        bool a = false;
        foreach (var u in users)
        {
            if (Context.User.Id == u.Id)
            {
                result = u.ChatId;
                a = true;
                break;
            }
        }

        if (a == false)
        {
            var textChannel = await guild.CreateTextChannelAsync($"Ticket from {Context.Message.Author}", properties =>
            {
                properties.CategoryId = category.Id;
            });
           
            User user = new User() { ChatId = textChannel.Id, Id = Context.User.Id };
            users.Add(user);
            Console.WriteLine("Start convert");
            string output = JsonConvert.SerializeObject(users);
            Console.WriteLine("End");

            await File.WriteAllTextAsync("users.json", output);
        
            await textChannel.SendMessageAsync($"Here is new text channel {textChannel.Id}");
            await ReplyAsync($"Text channel '{textChannel.Name}' has been created!");
            return;
        }

       var textChannelRes = guild.GetTextChannel(result); 
       await textChannelRes.SendMessageAsync("Welcome to channel");
       

    }

    [Command("s")]

    public async Task SentMessageInChannel([Remainder] string echo)
    {
        Console.WriteLine("Start");
        string usersJson = await File.ReadAllTextAsync("users.json");
        
        var users = JsonConvert.DeserializeObject<List<User>>(usersJson);

        ulong usersChat = 0;
        
        bool result = false;
        
        string json = await File.ReadAllTextAsync("botconfig.json");

        Console.WriteLine(json);
        
        var config = JsonConvert.DeserializeObject<BotConfig>(json)!;
        
        var guild = Context.Client.GetGuild(config.Server);
        
        var category = guild.GetCategoryChannel(config.Category);
        foreach (var user in users)
        {
            Console.WriteLine("ForEach");
            if (user.Id == Context.User.Id)
            {
                Console.WriteLine("f");
                usersChat = user.ChatId;
                result = true;
                break;
            }
        }
        Console.WriteLine(result);
        if (result)
        {
            var textChannel = guild.GetTextChannel(usersChat);
            await textChannel.SendMessageAsync(echo);
        }
    }


}

public class TicketMethods
{
    
}