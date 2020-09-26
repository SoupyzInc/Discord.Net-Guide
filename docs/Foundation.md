# Installation
In this tutorial we will use [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/) as our IDE of choice. Once Visual Studio is installed, let's make a new project. Open Visual Studio and "Create a new project".

Create a project targeting `C# Console App (.NET Core)`, then hit "Next" in the bottom right.

A new screen will pop up asking you to configure your project. Here you can rename your project and change where it is saved to. I will rename my project to "ExampleBot". When you have finished configuring your project, hit "Create" in the bottom right.

We can now install Discord.Net using NuGet. In the top tool bar select "Tools". Then under "NuGet Package Manager", hit "Manage NuGet Packages for Solution...".

![](https://github.com/SoupyzInc/Discord.Net-Guide/blob/master/WikiImages/Installation/NuGetManager.png)

In the top right, make sure you have the Package Source set to "All".

![](https://github.com/SoupyzInc/Discord.Net-Guide/blob/master/WikiImages/Installation/PackageSource.png)

Under the "Browse" tab, search for and install "Discord.Net". Make sure the "Project" box on the right hand side is ticked.

![](https://github.com/SoupyzInc/Discord.Net-Guide/blob/master/WikiImages/Installation/NuGetInstaller.png)

Our Visual Studio Project should now be set up. We can now begin to make the actual bot.

# Creating a Bot
Go to the [Discord Developer Portal](https://discord.com/developers/applications) and hit "New Application". You can name this application whatever you want. On the left side click on "Bot", then on the new screen, click on "Add Bot". Confirm your action. Set the "Username" of the bot to what you want the bot's name to be. If you want others to be able to invite bot's to their servers, turn on "Public Bot".

Now we can add the bot to your server. You must have the `Manage Server` permission in a server in order to invite the bot to that server. I would recommend making a server specifically for testing your bot, as to not annoy your friends. On the left side hit "OAuth2". On the new screen under "Scopes", check "Bot". Under "Bot Permissions" assign the bot any permissions you want it to have. If you are unsure what you want it to have, you can just give it "Administrator". 

![](https://cdn.discordapp.com/attachments/756953114065633321/756970209503477908/unknown.png)

Copy the link and open it. You should see the option to invite the bot to any server where you have the `Manage Server` permission. Invite the bot to the server you intend to use/test it on. 
