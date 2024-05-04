using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace DiscordChatAI.Helpers;

public class DiscordBot
{
    private static DiscordBot? _instance;
    private DiscordSocketClient? _client;

    public static DiscordBot Instance
    {
        get { return _instance ??= new DiscordBot(); }
    }

    public async Task Start(string token)
    {
        await using var services = ConfigureServices();
        _client = services.GetRequiredService<DiscordSocketClient>();

        _client.Log += LogAsync;
        services.GetRequiredService<CommandService>().Log += LogAsync;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Here we initialize the logic required to register our commands.
        await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
        await Task.Delay(-1);
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());

        return Task.CompletedTask;
    }

    private ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All
            })
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<CommandService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<GoogleGeminiProClient>()
            .BuildServiceProvider();
    }
}

public class CommonModule : ModuleBase<SocketCommandContext>
{
    [Command("chat")]
    [Summary("Chat with AI")]
    public async Task ChatAsync([Remainder] [Summary("What do you want to say to AI")] string message)
    {
        if (string.IsNullOrEmpty(GoogleGeminiProClient.Instance.ApiKey))
        {
            await Context.Message.ReplyAsync("AI is not setup correctly!");
            return;
        }

        var response = await GoogleGeminiProClient.Instance.GenerateString(message);
        if (string.IsNullOrEmpty(response))
        {
            await Context.Message.ReplyAsync("Failed to get Response from AI. Error must be on bot or AI.");
            return;
        }

        await Context.Message.ReplyAsync(response);
    }
}

public class CommandHandlingService
{
    private readonly CommandService _commands;
    private readonly DiscordSocketClient _discord;
    private readonly IServiceProvider _services;

    public CommandHandlingService(IServiceProvider services)
    {
        _commands = services.GetRequiredService<CommandService>();
        _discord = services.GetRequiredService<DiscordSocketClient>();
        _services = services;

        // Hook CommandExecuted to handle post-command-execution logic.
        _commands.CommandExecuted += CommandExecutedAsync;
        // Hook MessageReceived so we can process each message to see
        // if it qualifies as a command.
        _discord.MessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync()
    {
        // Register modules that are public and inherit ModuleBase<T>.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        // Ignore system messages, or messages from other bots
        if (rawMessage is not SocketUserMessage message)
            return;
        if (message.Source != MessageSource.User)
            return;

        // This value holds the offset where the prefix ends
        var argPos = 0;
        // Perform prefix check. You may want to replace this with
        // (!message.HasCharPrefix('!', ref argPos))
        // for a more traditional command format like !help.
        // if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos))
        if (!message.HasCharPrefix('!', ref argPos))
            return;

        var context = new SocketCommandContext(_discord, message);
        // Perform the execution of the command. In this method,
        // the command service will perform precondition and parsing check
        // then execute the command if one is matched.
        await _commands.ExecuteAsync(context, argPos, _services);
        // Note that normally a result will be returned by this format, but here
        // we will handle the result in CommandExecutedAsync,
    }

    private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        // command is unspecified when there was a search failure (command not found); we don't care about these errors
        if (!command.IsSpecified)
            return;

        // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
        if (result.IsSuccess)
            return;

        // the command failed, let's notify the user that something happened.
        await context.Channel.SendMessageAsync($"error: {result}");
    }
}