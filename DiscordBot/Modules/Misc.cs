using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Core.UserAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NReco.ImageGenerator;
using System.IO;

namespace DiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("hello")]
        public async Task Hello(string color = "red")
        {
            string css = "<style>\nh1\n{\ncolor: " + color + ";\n}\n</style>";
            string html = $"<h1>Hello {Context.User.Username }!</h1>";
            var converter = new NReco.ImageGenerator.HtmlToImageConverter
            {
                Width = 250,
                Height = 90
            };
            var jpgBytes = converter.GenerateImage(css + html, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), "hello.jpg");

        }

        [Command("myStats")]
        public async Task MyStats([Remainder] string arg = "")
        {
            SocketUser target = null;
            var mentionedUser = Context.Message.MentionedUsers.FirstOrDefault();
            target = mentionedUser ?? Context.User;

            var account = UserAccounts.getAccount(target);
            await Context.Channel.SendMessageAsync($"{ target.Username } have {account.XP} XP and {account.Points} points");

        }

        [Command("addXP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task addXP(uint xp)
        {
            var account = UserAccounts.getAccount(Context.User);
            account.XP += xp;
            UserAccounts.SaveAccounts();
            await Context.Channel.SendMessageAsync($"You gained {xp} XP and now you have a total of {account.XP} XP.");
        }

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
