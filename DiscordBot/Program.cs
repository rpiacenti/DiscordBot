using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Core;

namespace DiscordBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;
        
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });
            _client.Log += Log;
            _client.Ready += RepeatingTimer.StartTime;
            _client.ReactionAdded += OnReactionAdded;
            _handler = new CommandHandler();
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            Global.Client = _client;
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await ConsoleInput();
            await Task.Delay(-1);
        }

        private async Task ConsoleInput()
        {
            string input = string.Empty;
            while (input != null || input.Trim().ToLower() != "block")
            {
                input = Console.ReadLine();
                if (input.Trim().ToLower() == "message")
                    CoonsoleSendMessage();
            }
            
        }

        private async void CoonsoleSendMessage()
        {
            Console.WriteLine("Select the Guild:");
            var guild = getSelectedGuild(_client.Guilds);
            var textChannel = GetSelectedTextChannel(guild.TextChannels);
            var msg = string.Empty;
            while (msg.Trim() == string.Empty)
            {
                Console.WriteLine("Your message: ");
                msg = Console.ReadLine();
            }

            await textChannel.SendMessageAsync(msg);
        }

        private SocketTextChannel GetSelectedTextChannel(IEnumerable<SocketTextChannel> channels)
        {
            var TextChannels = channels.ToList();
            var maxIndex = TextChannels.Count();
            for (var i = 0; i < maxIndex; i++)
            {
                Console.WriteLine($"{i} - {TextChannels[i].Name}");
            }

            var selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                var success = int.TryParse(Console.ReadLine().Trim(), out selectedIndex);
                //Console.WriteLine(success);
                if (!success) Console.WriteLine("That was a invalid index, try again.");

            }

            return TextChannels[selectedIndex];

        }

        private SocketGuild getSelectedGuild(IEnumerable<SocketGuild> guilds)
        {
            var socketGuilds = guilds.ToList();
            var maxIndex = socketGuilds.Count();
            for(var i = 0; i < maxIndex; i++)
            {
                Console.WriteLine($"{i} - {socketGuilds[i].Name}");
            }

            var selectedIndex = -1;
            while (selectedIndex < 0 || selectedIndex > maxIndex)
            {
                var success = int.TryParse(Console.ReadLine().Trim(), out selectedIndex);
                if (!success)
                {
                    Console.WriteLine("That was a invalid index, try again.");
                    selectedIndex = -1;
                }

            }

            return socketGuilds[selectedIndex];

        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction )
        {
            if(reaction .MessageId == Global.MessageIdToTrack)
            {
                if(reaction .Emote.Name == "👌")
                {
                    await channel.SendMessageAsync(reaction.User.Value.Username + " says ok.");
                }
            }
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
