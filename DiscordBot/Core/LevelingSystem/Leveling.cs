using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DiscordBot.Core.LevelingSystem
{
    internal static class Leveling
    {
        internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel)
        {
            //if the user has a timeout, igone them

            var userAccount = UserAccounts.UserAccounts.getAccount(user);
            uint oldLevel = userAccount.LevelNumber;
            userAccount.XP += 50;
            UserAccounts.UserAccounts.SaveAccounts();
            uint newLevel = userAccount.LevelNumber;

            if(oldLevel != newLevel)
            {
                // the user leveled up
                var embed = new EmbedBuilder();
                embed.WithColor(67, 160, 71);
                embed.WithTitle("LEVEL UP!");
                embed.WithDescription(user.Username +  " just leveded up!");
                embed.AddInlineField("LEVEL", newLevel);
                embed.AddInlineField("XP", userAccount.XP);

                await channel.SendMessageAsync("", embed : embed);
            }
        }
    }
}
