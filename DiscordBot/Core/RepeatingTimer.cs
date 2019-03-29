using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordBot.Core
{
    internal static class RepeatingTimer
    {
        private static Timer loopingTimer;
        private static SocketTextChannel channel;

        internal static Task StartTime()
        {
            channel = Global.Client.GetGuild(560508202823974914).GetTextChannel(560508202823974916);

            loopingTimer = new Timer()
            {
                Interval = 5000,
                AutoReset  = true,
                Enabled = true
            };

            loopingTimer.Elapsed += OntimerTicked;




            return Task.CompletedTask;
        }

        private static async void OntimerTicked(object sender, ElapsedEventArgs e)
        {

            // await channel.SendMessageAsync("Ping!"); 
            
        }
    }
}
