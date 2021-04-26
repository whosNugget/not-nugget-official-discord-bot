using DSharpPlus;
using System;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using DSharpPlus.CommandsNext;
using System.Reflection;
using NuggetOfficial.Commands;
using DSharpPlus.EventArgs;
using NuggetOfficial.Actions;
using NuggetOfficial.Data.Converters;
using Microsoft.Extensions.DependencyInjection;
using DSharpPlus.Entities;
using System.Linq;

namespace NuggetOfficial
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().GetAwaiter().GetResult();
		}

		static async Task MainAsync()
		{
			DiscordClient discord = new DiscordClient(new DiscordConfiguration()
			{
				Token = ConfigurationManager.AppSettings.Get("botToken"),
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.All
			});

			IServiceCollection services = new ServiceCollection().AddSingleton<Random>();
			if (ConfigurationManager.AppSettings.AllKeys.Contains("registeredGuild"))
			{
				services.AddSingleton(await discord.GetGuildAsync(ulong.Parse(ConfigurationManager.AppSettings.Get("registeredGuild"))));
			}

			CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
			{
				StringPrefixes = new[] { "!" },
				EnableMentionPrefix = false,
				Services = services.BuildServiceProvider()
			});

			//discord.GuildMemberAdded += MemberValidation.TrackMemberJoinGuild;
			//discord.GuildMemberRemoved += MemberValidation.TrackMemberLeaveGuild;

			//commands.RegisterCommands<ModerationModule>();
			commands.RegisterCommands<VoiceChannelModule>();
			commands.RegisterConverter(new ServerRegionConverter());
			commands.RegisterConverter(new ChannelPublicityConverter());
			commands.RegisterConverter(new PermitDenyStringToBoolConverter());

			await discord.ConnectAsync();
			//TODO figure out why this isnt authenticated and fix it
			//await discord.UpdateStatusAsync(new DiscordActivity { Name = "streaming development", ActivityType = ActivityType.Streaming, StreamUrl = "https://twitch.tv/not__nugget"}, UserStatus.Online);

			await Task.Delay(-1);
		}
	}
}
