using System.Collections.Immutable;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordTestBot;

class Program {
    private readonly DiscordSocketClient _client;
    private string discordToken = "";

    public Program() {
        this._client                 =  new DiscordSocketClient();
        this._client.MessageReceived += MessageHandler;
        this._client.Log             += LogAsync;
    }

    public async Task StartBotAsync() {
        discordToken = Environment.GetEnvironmentVariable("DiscordBotToken");
        if (string.IsNullOrWhiteSpace(discordToken)) {
            Console.WriteLine(
                    "Error: No discord token found. Please provide a token via the DISCORD_TOKEN environment variable.");
            Environment.Exit(1);
        }
        
        await this._client.StartAsync();
        await this._client.LoginAsync(TokenType.Bot, discordToken);
        await Task.Delay(-1);
    }

    private async Task MessageHandler(SocketMessage msg) {
        if (msg.Author.IsBot) return;

        await ReplyAsync(msg, "C# response");
    }

    private async Task ReplyAsync(SocketMessage msg, string response) {
        await msg.Channel.SendMessageAsync(response);
    }

    static async Task Main(string[] args) {
        var myBot = new Program();
        await myBot.StartBotAsync();
    }

    private Task LogAsync(LogMessage log) {
        if (log.Exception is CommandException cmdException) {
            Console.WriteLine($"[Command/{log.Severity}] {cmdException.Command.Aliases.First()} Error in Channel [{cmdException.Context.Channel.Name}]: {cmdException.Context.Message}");
        }
        else {
            Console.WriteLine($"[General/{log.Severity}:] {log.Message}");
        }
        
        return Task.CompletedTask;
    }
}