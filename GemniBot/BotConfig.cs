namespace GemniBot;

public class BotConfig
{
    public ulong Server { get; set; }
    public ulong Category { get; set; }

    public BotConfig(ulong serverId, ulong categoryId)
    {
        Server = serverId;
        Category = categoryId;
    }
}