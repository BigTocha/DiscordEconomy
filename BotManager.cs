using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace DiscordEconomy
{
    public class BotManager
    {
        private readonly DiscordSocketClient _client;
        private CommandService _commands = new CommandService();

        public BotManager(DiscordSocketClient client)
        {
            _client = client;
            Initialize();
        }

        private async void Initialize()
        {
            _client.MessageReceived += ClientOnMessageReceived;
            ReactInterface.Client = _client;
            IEnumerable<ModuleInfo> modules =  await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            Console.WriteLine(modules.Count());
            Console.WriteLine(string.Join(", ", _commands.TypeReaders.Select(x => $"{x.Key.FullName} {x}")));
        }

        private async Task ClientOnMessageReceived(SocketMessage socketMessage)
        {
            SocketUserMessage message = socketMessage as SocketUserMessage;
            if (message == null) return;
            int argPos = 0;
            if (!(message.HasCharPrefix('%', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
                return;
            CommandContext context = new CommandContext(_client, message);
            IResult result = await _commands.ExecuteAsync(context, argPos);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
