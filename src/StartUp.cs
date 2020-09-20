//System
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

//Discod.NET
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class StartUp
{
    private static DiscordSocketClient _client;
    private static CommandService _commands;
    private static readonly IServiceProvider _services;

    private static Task Log(LogMessage message)
    {
        switch (message.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
        Console.ResetColor();

        // If you get an error saying 'CompletedTask' doesn't exist,
        // your project is targeting .NET 4.5.2 or lower. You'll need
        // to adjust your project's target framework to 4.6 or higher
        // (instructions for this are easily Googled).
        // If you *need* to run on .NET 4.5 for compat/other reasons,
        // the alternative is to 'return Task.Delay(0);' instead.
        return Task.CompletedTask;
    }

    private static async Task Main()
    {
        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            // How much logging do you want to see?
            LogLevel = LogSeverity.Info,

            // If you or another service needs to do anything with messages
            // (eg. checking Reactions, checking the content of edited/deleted messages),
            // you must set the MessageCacheSize. You may adjust the number as needed.
            //MessageCacheSize = 50,

            // If your platform doesn't have native WebSockets,
            // add Discord.Net.Providers.WS4Net from NuGet,
            // add the `using` at the top, and uncomment this line:
            //WebSocketProvider = WS4NetProvider.Instance
        });

        _commands = new CommandService(new CommandServiceConfig
        {
            // Again, log level:
            LogLevel = LogSeverity.Info,

            // There's a few more properties you can set,
            // for example, case-insensitive commands.
            CaseSensitiveCommands = false,
        });

        // Subscribe the logging handler to both the client and the CommandService.
        _client.Log += Log;
        _commands.Log += Log;

        // Setup your DI container.
        //_services = ConfigureServices();

        //Discord Stuff
        // Call the Program construct, folloorwed by the 
        // MainAsync method and wait until it finishes (which should be never).
        //new Program().MainAsync().GetAwaiter().GetResult();

        // Centralize the logic for commands into a separate method.
        await InitCommands();

        string token = System.IO.File.ReadAllText(@"\token.txt"); //Grabs token from text file.
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static async Task InitCommands()
    {
        // Either search the program and add all Module classes that can be found.
        // Module classes MUST be marked 'public' or they will be ignored.
        // You also need to pass your 'IServiceProvider' instance now,
        // so make sure that's done before you get here.
        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        // Or add Modules manually if you prefer to be a little more explicit:
        //await _commands.AddModuleAsync<SomeModule>(_services);
        // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

        // Subscribe a handler to see if a message invokes a command.
        _client.MessageReceived += HandleCommandAsync;
    }

    private static async Task HandleCommandAsync(SocketMessage arg)
    {
        // Bail out if it's a System Message.
        var msg = arg as SocketUserMessage;
        if (msg == null) return;

        // We don't want the bot to respond to itself or other bots.
        if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;

        // Create a number to track where the prefix ends and the command begins
        int pos = 0;
        // Replace the ',' with whatever character
        // you want to prefix your commands with.
        // Uncomment the second half if you also want
        // commands to be invoked by mentioning the bot instead.
        if (msg.HasCharPrefix('<', ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */)
        {
            // Create a Command Context.
            var context = new SocketCommandContext(_client, msg);

            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully).
            var result = await _commands.ExecuteAsync(context, pos, _services);

            // Uncomment the following lines if you want the bot
            // to send a message if it failed.
            // This does not catch errors from commands with 'RunMode.Async',
            // subscribe a handler for '_commands.CommandExecuted' to see those.
            /*
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                await msg.Channel.SendMessageAsync(result.ErrorReason);
            */
        }
    }
}
