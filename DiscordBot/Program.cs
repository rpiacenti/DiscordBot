using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            //string name = "Peter";
            //string botName = "Maercy-BOT2";
            //string message = Utilities.GetFormattedAlert("WELCOME_&NAME_&BOTNAME", name, botName);
            //Console.WriteLine(message);
            //Console.ReadLine();
            //return;

            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose
            });
            _client.Log += Log;
            _handler = new CommandHandler();
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
        }
    }
}
