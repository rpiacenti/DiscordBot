using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Message by " + Context.User.Username);
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed);

        }

        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string Selection = options[r.Next(0,options.Length)];

            var embed = new EmbedBuilder();
            embed.WithTitle("Choice for by " + Context.User.Username);
            embed.WithDescription(Selection);
            embed.WithColor(new Color(0, 255, 0));
            embed.WithThumbnailUrl("https://www.flyingmag.com/sites/flyingmag.com/files/styles/655_1x_/public/import/embedded/sites/all/files/_images/201308/FLY0813_Disney_01.jpg?itok=oQeBXbIx");
            //Context.User.GetAvatarUrl

            await Context.Channel.SendMessageAsync("", false, embed);

        }
    }
}
