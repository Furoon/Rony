using Discord;
using Discord.Commands;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rony
{
    class MyBot
    {
        DiscordClient discord;
        CommandService commands;
        Random rand;
        string[] triggerMoin;
        string[] freshestMemes = Directory.GetFiles("meme/");
        string[] randomTexts; //Variable
        string[] jokeList = File.ReadAllLines("txt/jokes.txt");

        public MyBot()
        {
            rand = new Random();    //Random Generator
            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });


            discord.UsingCommands(x =>
            {
                x.PrefixChar = '!';
                x.AllowMentionPrefix = true;
            });

            commands = discord.GetService<CommandService>();
            RegisterMemeCommand();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MzUyNzcwODEzOTk4NjYxNjMy.DImEdQ.mQT5_TVh8gDkEhWsiuzD69WzC9o", TokenType.Bot);
            });
        }
        //----------------------------------------------COMMANDS---------------------------------------
        private void RegisterMemeCommand()
        {
            commands.CreateCommand("meme")
                .Do(async (e) =>
                {
                    int randomMemeIndex = rand.Next(freshestMemes.Length);
                    string memeToPost = freshestMemes[randomMemeIndex];
                    await e.Channel.SendFile(memeToPost);
                    //await e.Channel.SendMessage(memeToPost);
                });
            {
                commands.CreateCommand("joke")
                        .Do(async (e) =>
                        {
                            int randomJokeIndex = rand.Next(jokeList.Length);
                            string jokeToPost = jokeList[randomJokeIndex];
                            await e.Channel.SendMessage(jokeToPost);
                        });
            }
            {
                commands.CreateCommand("hello")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendMessage($"Moin @{e.User.Name} ^-^/");
                    });
            }
            {
                commands.CreateCommand("kick")
                    .Parameter("User", ParameterType.Required)
                    .Do(async e =>
                    {
                        try
                        {
                            if (e.User.ServerPermissions.KickMembers)
                            {
                                User user = null;
                                try
                                {
                                    // try to find the user
                                    user = e.Server.FindUsers(e.GetArg("User")).First();
                                }
                                catch (InvalidOperationException)
                                {
                                    await e.Channel.SendMessage($"Couldn't kick user {e.GetArg("User")} (not found).");
                                    return;
                                }
                                // double safety check
                                if (user == null) await e.Channel.SendMessage($"Couldn't kick user {e.GetArg("User")} (not found).");
                                await user.Kick();
                                await e.Channel.SendMessage($"{user.Name} was kicked from the server!");
                            }
                            else
                            {
                                await e.Channel.SendMessage($"{e.User.Name} you don't have the permission to kick.");
                            }
                        }
                        catch (Exception ex)
                        {
                            // needs a better error handling haven't changed it since i tested it xD
                            await e.Channel.SendMessage(ex.Message);
                        }
                    });
            }
            {
                commands.CreateCommand("poke")
                .Parameter("target", ParameterType.Required)
                .Do(async (e) =>
                {
                    ulong id;
                    User u = null;
                    string findUser = e.Args[0];

                    if (!string.IsNullOrWhiteSpace(findUser))
                    {
                        if (e.Message.MentionedUsers.Count() == 1)
                            u = e.Message.MentionedUsers.FirstOrDefault();
                        else if (e.Server.FindUsers(findUser).Any())
                            u = e.Server.FindUsers(findUser).FirstOrDefault();
                        else if (ulong.TryParse(findUser, out id))
                            u = e.Server.GetUser(id);
                    }
                    Console.WriteLine("[" + e.Server.Name + "]" + e.User.Name + " just poked " + u);
                    await e.Channel.SendMessage(e.Args[0] + " wurde angestupst ");
                    await u.SendMessage("HEY, du wurdest von " + e.User.Name + " vom Server " + e.Server.Name + " angestupst");
                });
            }
            //Client.UserJoind
        }
        //----------------------------------------------COMMANDS END---------------------------------------
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}