using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            string selection = options[r.Next(0,options.Length)];

            var embed = new EmbedBuilder();
            embed.WithTitle("Choice for by " + Context.User.Username);
            embed.WithDescription(selection);
            embed.WithColor(new Color(0, 255, 0));
            embed.WithThumbnailUrl("https://www.flyingmag.com/sites/flyingmag.com/files/styles/655_1x_/public/import/embedded/sites/all/files/_images/201308/FLY0813_Disney_01.jpg?itok=oQeBXbIx");
            //Context.User.GetAvatarUrl

            await Context.Channel.SendMessageAsync("", false, embed);
            DataStorage.AddPairToStorage(Context.User.Username + DateTime.Now.ToLongDateString(), selection);
        }

        [Command("secret")]
        public async Task RevealSecret([Remainder]string arg = "")
        {
            if (!UserIsSecretOwner((SocketGuildUser)Context.User))
            {
                await Context.Channel.SendMessageAsync(":x: You need the SecretOwner2 role to do that. " + Context.User.Mention);
                return;
            }
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(Utilities.GetAlert("SECRET")); 
        }

        private bool UserIsSecretOwner(SocketGuildUser user)
        {

            string targetRoleName = "SecretOwner2";
            var result = from r in user.Guild.Roles
                         where r.Name == targetRoleName
                         select r.Id;
            ulong roleId = result.FirstOrDefault();
            if (roleId == 0)
            {
                return false;
            }
            var targetRole = user.Guild.GetRole(roleId);
            Console.WriteLine($"Role ID: {roleId} and Target role: {targetRole} and {user.Roles.Contains(targetRole)}");
            return user.Roles.Contains(targetRole);
        }

        [Command("data")]
        public async Task GetData([Remainder]string arg = "")
        {

            await Context.Channel.SendMessageAsync("Data Has " + DataStorage.GetPairsCount() + " pairs.");
            DataStorage.AddPairToStorage("Count" + DataStorage.GetPairsCount(), "TheCount" + DataStorage.GetPairsCount());

        }
    }
}
