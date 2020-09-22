//System
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

//Discod.NET
using Discord;
using Discord.Commands;
using Discord.WebSocket;

//HTML AGILITY PACK
using HtmlAgilityPack;

class Commands
{
    public class TeamMoudle : ModuleBase<SocketCommandContext>
    {
        [Command("Team")]
        public async Task Team(params IUser[] usersToExclude)
        {
            DiscordSocketClient targetClient = Context.Client; //Gets the reference to the dude who said the message
            IVoiceChannel userChannel = (Context.User as IVoiceState).VoiceChannel; //Gives reference to the users voice channel

            //User Not In VC Error Handler
            try
            {
                var voiceUsers = Context.Guild.GetVoiceChannel(userChannel.Id).Users; //grabs all the people in the vc
            }
            catch
            {
                //Error Handler Message
                var builderTest = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Join a voice channel to use the team command.",
                    },

                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Requested by " + Context.User,
                        IconUrl = Context.User.GetAvatarUrl()
                    },
                };

                builderTest.WithColor(0xFEFEFE)
                        .Build();

                await Context.Channel.SendMessageAsync(embed: builderTest.Build());
            }
            finally
            {
                var voiceUsers = Context.Guild.GetVoiceChannel(userChannel.Id).Users; //grabs all the people in the vc
                List<SocketGuildUser> usersInChannel = voiceUsers.ToList(); //Puts them in a list

                List<SocketGuildUser> usersToParse = new List<SocketGuildUser>();
                List<SocketGuildUser> usersToRemove = new List<SocketGuildUser>();

                for (int i = 0; i < usersInChannel.Count; i++)
                {
                    usersToParse.Add(Context.Guild.GetUser(usersInChannel[i].Id)); //Converts the all the user's IUsers to SocketGuildUsers
                }
                for (int i = 0; i < usersInChannel.Count; i++) //Removes Bots
                {
                    if (usersInChannel[i].IsBot)
                    {
                        usersToRemove.Add(usersInChannel[i]);
                    }
                }

                for (int i = 0; i < usersToExclude.Length; i++)
                {
                    usersToRemove.Add(Context.Guild.GetUser(usersToExclude[i].Id)); //Converts all the unwanted user's IUsers to SocketGuildUsers
                }

                List<SocketGuildUser> usersToTeam = new List<SocketGuildUser>();
                usersToTeam = usersToParse.Except(usersToRemove).ToList(); //Removes unwanted users

                //Team Creation
                Random rnd = new Random();
                List<SocketGuildUser> sortedUsers = usersToTeam.OrderBy(x => rnd.Next()).ToList(); //Randomly sorts users

                List<SocketGuildUser> blueTeam = new List<SocketGuildUser>(); //Generate a blue team
                List<SocketGuildUser> redTeam = new List<SocketGuildUser>(); //Generate a red team

                List<string> blueTeamUsers = new List<string>(); //Make a list of all blue team players
                List<string> redTeamUsers = new List<string>(); //Make a list of all red team players

                for (int i = 0; i < (sortedUsers.Count / 2); i++) //Add half the players to red team
                {
                    redTeam.Add(sortedUsers[i]);
                    redTeamUsers.Add(sortedUsers[i].Id.ToString()); //Adds the red team players to the red list
                }

                blueTeam = sortedUsers.Except(redTeam).ToList(); //Add the other half to blue team

                for (int i = 0; i < blueTeam.Count; i++)
                {
                    blueTeamUsers.Add(blueTeam[i].Id.ToString()); //Adds the blue team players to the blue list
                }

                string blueText = string.Join(">, <@", blueTeamUsers.ToArray()); //Combines blue string array into a long string
                string redText = string.Join(">, <@", redTeamUsers.ToArray()); //Combines red string array into a long string

                string redMessage = "";
                string blueMessage = "";

                if (!string.IsNullOrWhiteSpace(redText))
                {
                    redMessage = "<@" + redText + "> ";
                }

                if (!string.IsNullOrWhiteSpace(blueText))
                {
                    blueMessage = "<@" + blueText + "> ";
                }

                //Message Handling
                Discord.Color lightBlue = new Discord.Color(0xd4e9f1);
                var builder = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Teams",
                    },

                    Description = "**:red_square: | **" + redMessage + "\n" +
                                  "**:blue_square: | **" + blueMessage,

                    Footer = new EmbedFooterBuilder
                    {
                        Text = "This function is powered by FunkyBot™.",
                        IconUrl = "https://cdn.discordapp.com/attachments/736361743139209307/742053144678105258/Funky_Bot.png"
                    },
                };

                builder.WithColor(lightBlue)
                        .Build();

                await Context.Channel.SendMessageAsync(embed: builder.Build());
            }
        }
    }

    public class SiegeModule : ModuleBase<SocketCommandContext>
    {
        [Command("r6")]
        public async Task Rank(string name, string platform)
        {
            platform = platform.ToLower();

            //Loading Embed Builder
            var builderLoading = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "Loading user data. . .",
                },

                Footer = new EmbedFooterBuilder
                {
                    Text = "Requested by " + Context.User,
                    IconUrl = Context.User.GetAvatarUrl()
                },
            };

            builderLoading.WithColor(0xFEFEFE)
                    .Build();

            var Message = await Context.Channel.SendMessageAsync(embed: builderLoading.Build());
            

            //Load HTML
            var html = @"https://r6.tracker.network/profile/" + platform + "/" + name; //Sets Profile Overiew
         
            //Loads Profile Overview
            HtmlWeb web = new HtmlWeb();
            var doc = web.Load(html);

            //Invalid Username Error Handler
            try
            {
                HtmlNode requestedNameTest = doc.DocumentNode.SelectSingleNode("(//h1[@class='trn-profile-header__name'])"); //Loads Name and Profile Views
                var requestedNameSpanTest = requestedNameTest.Element("span"); //Loads Name
            }
            catch
            {
                //Error Embed Builder
                var builderTest = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Could not find user " + name + ".",
                    },

                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Requested by " + Context.User,
                        IconUrl = Context.User.GetAvatarUrl()
                    },
                };

                builderTest.WithColor(0xFEFEFE)
                        .Build();

                //await Context.Channel.SendMessageAsync(embed: builderTest.Build());
                await Message.ModifyAsync(msg => msg.Embed = builderTest.Build());
            }
            finally
            {
                var htmlKD = html + "/seasons"; //Sets Season Page

                //Loads Season Page
                HtmlWeb webKD = new HtmlWeb();
                var docKD = webKD.Load(htmlKD);

                HtmlNode requestedKD = docKD.DocumentNode.SelectSingleNode("(//div[@class='trn-defstat__value'])"); //Loads KD from Seasons Page

                HtmlNode requestedName = doc.DocumentNode.SelectSingleNode("(//h1[@class='trn-profile-header__name'])"); //Loads Name and Profile Views
                var requestedNameSpan = requestedName.Element("span"); //Loads Name

                HtmlNode requestedLevel = doc.DocumentNode.SelectSingleNode("(//div[@class='trn-defstat__value'])"); //Loads Level
                HtmlNode requestedRank = doc.DocumentNode.SelectSingleNode("(//div[@class='trn-defstat__value'])[3]"); //Loads Rank
                HtmlNode requestedHeadshot = doc.DocumentNode.SelectSingleNode("(//div[@data-stat='PVPAccuracy'])"); //Loads Headshot %
                HtmlNode requestedAvatar = doc.DocumentNode.SelectSingleNode("(//div[@class='trn-profile-header__content trn-card__content'])//img"); //Loads Requested Avatar     

                SiegeData siegedata = new SiegeData(); //Loads dictionaries from seperate SiegeData.cs

                string rankIcon = (string)siegedata.rankImages[requestedRank.InnerHtml.Trim('\n')]; //Loads rank icon

                Discord.Color embedColor = new Discord.Color(siegedata.rankColors[requestedRank.InnerHtml.Trim('\n')]); //Loads embed color

                //Loads MMR
                string mmr;

                if (requestedRank.InnerText.Trim('\n') == "Not ranked yet.") //Loads unranked mmr
                {
                    HtmlNode unrankedMMR = doc.DocumentNode.SelectSingleNode("(//div[@style='font-size: 2rem;'])"); //Loads MMR
                    mmr = unrankedMMR.InnerText.Trim('\n'); //Sets MMR
                }
                else //Loads ranked mmr
                {
                    HtmlNode requestedMMR = doc.DocumentNode.SelectSingleNode("(//div[@class='r6-season-rank__progress-fill'])"); //Loads Season Stats Table
                    var requestedMMRSpan = requestedMMR.Element("span"); //Loads MMR
                    mmr = requestedMMRSpan.InnerText.Trim('\n'); //Sets MMR
                }

                //Embed Builder
                var builder = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = requestedNameSpan.InnerText.Trim('\n') + "'s Stats",
                        IconUrl = requestedAvatar.GetAttributeValue("src", "")
                    },

                    Description = "**Level**\n> " + requestedLevel.InnerText.Trim('\n')
                                + "\n**Seasonal KD**\n> " + requestedKD.InnerText.Trim('\n')
                                + "\n**MMR**\n> " + mmr
                                + "\n**Headshot %**\n> " + requestedHeadshot.InnerText.Trim('\n')
                                + "\n[View Full Stats](" + html + ")",

                    ThumbnailUrl = rankIcon,

                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Requested by " + Context.User,
                        IconUrl = Context.User.GetAvatarUrl()
                    },
                };

                builder.WithColor(embedColor)
                        .Build();

                //await Context.Channel.SendMessageAsync(embed: builder.Build());
                await Message.ModifyAsync(msg => msg.Embed = builder.Build());
            }
        }
    }

    public class AmongUsMoudle : ModuleBase<SocketCommandContext>
    {
        [Command("unmute", RunMode = RunMode.Async)]
        public async Task unmute(params IUser[] deadPersons)
        {
            IVoiceChannel userChannel = (Context.User as IVoiceState).VoiceChannel; //Gives reference to the users voice channel

            //User Not In VC Error Handler
            try
            {
                var voiceUsers = Context.Guild.GetVoiceChannel(userChannel.Id).Users; //grabs all the people in the vc
            }
            catch
            {
                //Error Handler Message
                var builderTest = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Join a voice channel to use the Unmute command.",
                    },

                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Requested by " + Context.User,
                        IconUrl = Context.User.GetAvatarUrl()
                    },
                };

                builderTest.WithColor(0xFEFEFE)
                        .Build();

                await Context.Channel.SendMessageAsync(embed: builderTest.Build());
            }
            finally
            {
                ManageUserMute manageUserMute = new ManageUserMute();
                await ReplyAsync(embed: manageUserMute.ManageMute(Context, false, deadPersons));
            }
        }

        [Command("mute", RunMode = RunMode.Async)]
        public async Task NewMute()
        {
            IVoiceChannel userChannel = (Context.User as IVoiceState).VoiceChannel; //Gives reference to the users voice channel

            //User Not In VC Error Handler
            try
            {
                var voiceUsers = Context.Guild.GetVoiceChannel(userChannel.Id).Users; //grabs all the people in the vc
            }
            catch
            {
                //Error Handler Message
                var builderTest = new EmbedBuilder
                {
                    Author = new EmbedAuthorBuilder
                    {
                        Name = "Join a voice channel to use the mute command.",
                    },

                    Footer = new EmbedFooterBuilder
                    {
                        Text = "Requested by " + Context.User,
                        IconUrl = Context.User.GetAvatarUrl()
                    },
                };

                builderTest.WithColor(0xFEFEFE)
                        .Build();

                await Context.Channel.SendMessageAsync(embed: builderTest.Build());
            }
            finally
            {
                ManageUserMute manageUserMute = new ManageUserMute();
                await ReplyAsync(embed: manageUserMute.ManageMute(Context, true));
            }
        }
    }

    [Group("help")]
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        string soupyzLLCImg = "https://cdn.discordapp.com/attachments/734935840282771598/741382917653004431/S_Monochrome.png";
        Discord.Color white = new Discord.Color(0xFEFEFE);

        [Command()]
        public async Task Help(int num = 0)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = "SoupyzLLC Documentation",
                },

                ThumbnailUrl = soupyzLLCImg,

                Description = "`.help team`\n`.help r6`\n`.help mute`\n`.help unmute`",

                Footer = new EmbedFooterBuilder
                {
                    Text = "Requested by " + Context.User,
                    IconUrl = Context.User.GetAvatarUrl()
                },
            };

            builder.WithColor(white)
                    .Build();

            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }

        [Command("team")]
        public async Task HelpTeam(int num = 0)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = ".team Documentation",
                },

                ThumbnailUrl = soupyzLLCImg,

                Description = "`.team @User1 @User2`\nCreates two teams from people in your voice channel (excluding bots).\n*User(s) can be excluded from the team draft by mentioning them.",

                Footer = new EmbedFooterBuilder
                {
                    Text = "Requested by " + Context.User,
                    IconUrl = Context.User.GetAvatarUrl()
                },
            };

            builder.WithColor(white)
                    .Build();

            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }

        [Command("r6")]
        public async Task HelpRSix(int num = 0)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = ".r6 Documentation",
                },

                ThumbnailUrl = soupyzLLCImg,

                Description = "`.r6 PlayerUplayName Platform`\nSearches [r6tracker](https://r6.tracker.network/) for the specified player's stats.",

                Footer = new EmbedFooterBuilder
                {
                    Text = "Requested by " + Context.User,
                    IconUrl = Context.User.GetAvatarUrl()
                },
            };

            builder.WithColor(white)
                    .Build();

            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }

        [Command("mute")]
        public async Task HelpMute(int num = 0)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = ".mute Documentation",
                },

                ThumbnailUrl = soupyzLLCImg,

                Description = "`.mute`\nServer mutes everyone in your voice channel.",

                Footer = new EmbedFooterBuilder
                {
                    Text = "Requested by " + Context.User,
                    IconUrl = Context.User.GetAvatarUrl()
                },
            };

            builder.WithColor(white)
                    .Build();

            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }

        [Command("unmute")]
        public async Task HelpUnmute(int num = 0)
        {
            var builder = new EmbedBuilder
            {
                Author = new EmbedAuthorBuilder
                {
                    Name = ".unmute Documentation",
                },

                ThumbnailUrl = soupyzLLCImg,

                Description = "`.unmute @User1 @User2`\nUnserver mutes everyone in your voice channel.\n*User(s) can be excluded from the unmute by mentioning them.",

                Footer = new EmbedFooterBuilder
                {
                    Text = "Requested by " + Context.User,
                    IconUrl = Context.User.GetAvatarUrl()
                },
            };

            builder.WithColor(white)
                    .Build();

            await Context.Channel.SendMessageAsync(embed: builder.Build());
        }
    }
}
