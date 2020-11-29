using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace DiscordEconomy
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTask().GetAwaiter().GetResult();
        }
        
        public async static Task MainTask()
        {
           DiscordSocketConfig config = new DiscordSocketConfig { MessageCacheSize = 100, ConnectionTimeout = 5000, WebSocketProvider = Discord.Net.Providers.WS4Net.WS4NetProvider.Instance };
            DiscordSocketClient client = new DiscordSocketClient(config);
            client.Log += message =>
            {
                return Task.Run(() => Console.WriteLine(message.Exception?.Message ?? message.Message));
            };
            await client.LoginAsync(TokenType.Bot, @"MzY1ODY0MzI4MjgyNTA1MjE2.DLkhSA.fE20OTrd8XumtAQPm4AvHNbrDuE");
            await client.StartAsync();

            client.Ready += () =>
            {
                return Task.Run(() =>
                {
                    Console.WriteLine("Bot is connected");
                    SocketTextChannel channel = client.Guilds.First().GetTextChannel(256795890302320640);
                    WriteTask(channel);
                });
            };
            BotManager bot = new BotManager(client);


            await Task.Delay(-1);
        }

        private async static void WriteTask(SocketTextChannel channel)
        {
            await Task.Run(() =>
            {
                string input = string.Empty;
                while (input != "exit")
                {
                    if (!string.IsNullOrEmpty(input))
                        channel.SendMessageAsync(input).Wait();
                    input = Console.ReadLine();
                }
            });
        }
    }
}
