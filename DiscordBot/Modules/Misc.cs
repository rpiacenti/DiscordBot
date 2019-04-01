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
using System.Net;
using Newtonsoft.Json;
using Discord.Rest;
using System.Runtime.InteropServices.ComTypes;

namespace DiscordBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("Warn")]
        [RequireUserPermission(GuildPermission.Administrator )]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGroupUser user)
        {
            var userAccount = UserAccounts.getAccount((SocketUser)user);
            userAccount.NumberOfWarnings++;
            UserAccounts.SaveAccounts();

            //punishiment check
            if(userAccount.NumberOfWarnings >= 3)
            {
                //await user.Guild.AddBanAsync(user, 5);
            }
            else if(userAccount.NumberOfWarnings >=2)
            {
                await Context.Channel.SendMessageAsync("You received more than 2 warning! If you recive more than 3 warnings you could be banned!");
            }
            else if (userAccount.NumberOfWarnings >= 1)
            {
                await Context.Channel.SendMessageAsync("You received your first warning! If you recive more than 3 warnings you could be banned!");
            }
        }

        [Command("Kick")]
        [RequireUserPermission (GuildPermission.KickMembers)]
        [RequireBotPermission (GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, string reason = "No reason provided")
        {
           // await user.KickAsync(reason);
        }

        [Command("Ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, string reason = "No reason provided")
        {
           // await user.Guild.AddBanAsync(user, 5, reason );
        }

        [Command("WhatLevelIs")]
        public async Task WhatLevelIs(uint xp)
        {
            uint level = (uint)Math.Sqrt(xp / 50); 
            await Context.Channel.SendMessageAsync("The level is " + level);
        }

        [Command("react")]
        public async Task HandleReactionMessage()
        {
            RestUserMessage msg = await Context.Channel.SendMessageAsync("React to me!");
            //msg.Id
            Global.MessageIdToTrack = msg.Id;


        }

        [Command("person")]
        public async Task GetRandomPerson()
        {
            string json = "";
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("https://randomuser.me/api/?gender=female&nat=BR");
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);
            string gender = dataObject.results[0].gender.ToString();
            string firstName = dataObject.results[0].name.first.ToString();
            string lastName = dataObject.results[0].name.last.ToString();
            string avatarURL = dataObject.results[0].picture.large.ToString();

            var embed = new EmbedBuilder();
            embed.WithThumbnailUrl(avatarURL);
            embed.WithTitle("Genereted Person");
            embed.AddInlineField("Firt Name:", char.ToUpper(firstName[0]) + firstName.Substring(1));
            embed.AddInlineField("Last Name:", char.ToUpper(lastName[0]) + lastName.Substring(1));

            await Context.Channel.SendMessageAsync("", embed: embed);

        }

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
            await dmChannel.SendMessageAsync("https://discord.gg/J4SJCFe");


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

        [Command("dice")]
        public async Task PlayDice()
        {
  
            Random r = new Random();
            string selection = r.Next(1, 6) + "W.png";

            string startupPath = AppDomain.CurrentDomain.BaseDirectory;
            string targetPath = startupPath + "Resources\\";
            string fileName = startupPath + selection;
            string ImageUrl = $"attachment://{fileName}";

            FileStream SourceStream = File.Open(fileName, FileMode.Open);

            await Context.Channel.SendFileAsync(SourceStream, selection, "Rolled Dice by " + Context.User.Username,false);

        }
    }
}
