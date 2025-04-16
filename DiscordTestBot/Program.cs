using System.Collections.Immutable;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using System.Reactive.Concurrency;


namespace DiscordTestBot;

class Program
{
    private readonly DiscordSocketClient _client;
    private string discordToken = "";

    private static System.Timers.Timer ReminderTimer;
    public Program()
    {
        this._client = new DiscordSocketClient();
        this._client.MessageReceived += MessageHandler;
        this._client.Log += LogAsync;

        ReminderTimer = new System.Timers.Timer(new TimeSpan(TimeSpan.FromHours(20).Ticks));
        ReminderTimer.Elapsed += ReminderMediMsg;
        ReminderTimer.AutoReset = true;
        ReminderTimer.Enabled = true;
    }



    private void ReminderMediMsg(object? sender, ElapsedEventArgs e)
    {
        var UserId = _client.GetUser(808428033056178286);
        MessageToChannelAsync(UserId.Id).Wait();

    }



    private async Task MessageToChannelAsync(ulong UserId)
    {
        var GuildId = _client.GetGuild(1053851905291468930);
        System.Console.WriteLine(GuildId.Name);

        await _client.GetGuild(1053851905291468930)
        .GetTextChannel(1362140103341768905)
        .SendMessageAsync($"<@{UserId}> Meowditation Time :3");
    }






    public async Task StartBotAsync()
    {
        discordToken = Environment.GetEnvironmentVariable("DiscordBotToken");
        if (string.IsNullOrWhiteSpace(discordToken))
        {
            Console.WriteLine(
                    "Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
            Environment.Exit(1);
        }

        await this._client.StartAsync();
        await this._client.LoginAsync(TokenType.Bot, discordToken);
        await Task.Delay(-1);
    }

    private async Task MessageHandler(SocketMessage msg)
    {
        if (msg.Author.IsBot) return;

        // await MessageToChannelAsync();

    }

    private async Task ReplyAsync(SocketMessage msg, string response)
    {
        await msg.Channel.SendMessageAsync(response);
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

    static async Task Main(string[] args)
    {
        var myBot = new Program();
        await myBot.StartBotAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        if (log.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{log.Severity}] {cmdException.Command.Aliases.First()} Error in Channel [{cmdException.Context.Channel.Name}]: {cmdException.Context.Message}");
        }
        else
        {
            Console.WriteLine($"[General/{log.Severity}:] {log.Message}");
        }

        return Task.CompletedTask;
    }



}


