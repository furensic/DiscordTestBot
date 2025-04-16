using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Timer = System.Timers.Timer;

namespace DiscordTestBot;

internal class Program {
    private static   Timer               ReminderTimer; // replace with Cron Job
    private readonly DiscordSocketClient _client;
    private          string              discordToken = "";

    public Program() {
        _client                 =  new DiscordSocketClient();
        _client.MessageReceived += MessageHandler;
        _client.Log             += LogAsync;

        // create a new timer using a calculated TimeSpan that gets the Ticks from 20 Hours.
        ReminderTimer           =  new Timer(new TimeSpan(TimeSpan.FromHours(20).Ticks)); // replace with Cron Job
        ReminderTimer.Elapsed   += ReminderMediMsg; // add function to EventHandler
        ReminderTimer.AutoReset =  true;
        ReminderTimer.Enabled   =  true; // enable the timer
    }


    private void ReminderMediMsg(object? sender, ElapsedEventArgs e) {
        var UserId = _client.GetUser(808428033056178286); // redundant
        MessageToChannelAsync(UserId.Id).Wait();
    }

    // benötigt parameter
    private async Task MessageToChannelAsync(ulong userId) {
        var GuildId = _client.GetGuild(1053851905291468930); // redundant
        Console.WriteLine(GuildId.Name);                     // nicht benötigt

        // Werte parametrisieren
        await _client.GetGuild(1053851905291468930)
                     .GetTextChannel(1362140103341768905)
                     .SendMessageAsync($"<@{userId}> Meowditation Time :3"); // geht evtl auch besser
    }

    public async Task StartBotAsync() {
        // Get bot token from environment variable
        discordToken = Environment.GetEnvironmentVariable("DiscordBotToken") ?? string.Empty;

        // Check if bot token was set
        if (string.IsNullOrWhiteSpace(discordToken)) {
            Console.WriteLine(
                    "Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
            Environment.Exit(1);
        }

        // start the discord client object
        await _client.StartAsync();
        await _client.LoginAsync(TokenType.Bot, discordToken); // login to API with token
        await _client.SetCustomStatusAsync("rechtsanwalt nick sarafi sers!"); // set discord status, duh
        await Task.Delay(-1); // keep application open, prevents program from being closed due to awaiting an infinitely running Delay Task.
    }

    // Method hooked to Event when a message is received by the discord socket client, taking a SocketMessage as parameter
    private async Task MessageHandler(SocketMessage msg) {
        if (msg.Author.IsBot) return; // prevents infinite loops

        // await MessageToChannelAsync();
    }

    private async Task ReplyAsync(SocketMessage msg, string response) {
        await msg.Channel
                 .SendMessageAsync(
                         response); // Asynchronously sends a message "string" onto Channel(ISocketMessageChannel)
    }

    // /* Meditaion Reminder */
    // private async Task ReminderMediMsg(SocketMessage msg, string response)
    // {

    //     var ChannelName = msg.Channel.Name;

    //     await msg.Channel.SendMessageAsync($"{ChannelName}: {response}");

    //     var UserIdPing = msg.Author.Id;
    //     System.Console.WriteLine(UserIdPing);

    //     await msg.Channel.SendMessageAsync($"{msg.Author.Mention}");


    // }

    private static async Task Main(string[] args) {
        var myBot = new Program();   // instantiate new class
        await myBot.StartBotAsync(); // run Init method of this class
    }

    // Hooked method onto Logger Interface of Discord.NET library. This can be replaced by SeriLog or .NET Logger library later
    private Task LogAsync(LogMessage log) {
        if (log.Exception is CommandException cmdException)
            Console.WriteLine(
                    $"[Command/{log.Severity}] {cmdException.Command.Aliases.First()} Error in Channel [{cmdException.Context.Channel.Name}]: {cmdException.Context.Message}");
        else
            Console.WriteLine($"[General/{log.Severity}:] {log.Message}");

        return Task.CompletedTask;
    }
}