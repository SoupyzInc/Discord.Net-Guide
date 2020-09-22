//System
using System.Collections.Generic;
using System.Linq;

//Discord.Net
using Discord;
using Discord.Commands;
using Discord.WebSocket;

class ManageUserMute
{
    public Embed ManageMute(SocketCommandContext Context, bool doMute, params IUser[] deadPeople)
    {
        IVoiceChannel userChannel = (Context.User as IVoiceState).VoiceChannel; //Gives reference to the users voice channel
        var voiceUsers = Context.Guild.GetVoiceChannel(userChannel.Id).Users; //grabs all the people in the vc
        List<SocketGuildUser> usersInChannel = voiceUsers.ToList(); //Puts them in a list

        List<SocketGuildUser> usersToMute = new List<SocketGuildUser>();
        List<SocketGuildUser> usersToRemove = new List<SocketGuildUser>();
        List<SocketGuildUser> deadPeopleToRemove = new List<SocketGuildUser>();

        for (int i = 0; i < usersInChannel.Count; i++)
        {
            usersToMute.Add(Context.Guild.GetUser(usersInChannel[i].Id)); //Converts the all the user's IUsers to SocketGuildUsers
        }
        for (int i = 0; i < usersInChannel.Count; i++) //Removes Bots
        {
            if (usersInChannel[i].IsBot)
            {
                usersToRemove.Add(usersInChannel[i]);
            }
        }

        foreach (var person in deadPeople)
        {
            deadPeopleToRemove.Add(Context.Guild.GetUser(person.Id));
        }

        usersToMute = usersToMute.Except(usersToRemove).ToList();

        MuteOperation(doMute, usersToMute, deadPeopleToRemove);

        var builderTest = new EmbedBuilder
        {
            Author = new EmbedAuthorBuilder
            {
                Name = "Please wait 10 seconds to use the Mute/Unmute command again.",
            },

            Footer = new EmbedFooterBuilder
            {
                Text = "Requested by " + Context.User,
                IconUrl = Context.User.GetAvatarUrl()
            },
        };

        builderTest.WithColor(0xFEFEFE)
                .Build();
        return builderTest.Build();
    }

    public async void MuteOperation(bool Operation, List<SocketGuildUser> UsersToMute, List<SocketGuildUser> deadPeople)
    {
        if (Operation == true)
        {
            foreach (var user in UsersToMute)
            {
                await user.ModifyAsync(m => { m.Mute = true; });
            }
        }
        else if (Operation == false)
        {
            UsersToMute = UsersToMute.Except(deadPeople).ToList();
            foreach (var user in UsersToMute)
            {
                await user.ModifyAsync(m => { m.Mute = false; });
            }
        }
    }
}