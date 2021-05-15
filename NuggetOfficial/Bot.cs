using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuggetOfficial.Actions.Serialization;
using NuggetOfficial.Commands;
using NuggetOfficial.Data.Converters;
using NuggetOfficial.Data.VoiceModule;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace NuggetOfficial
{
	class Bot
	{
		static VoiceRegisteredGuildData guildDataReference = null;

		static void Main()
		{
			guildDataReference = Serializer.Deserialize<VoiceRegisteredGuildData>(ConfigurationManager.AppSettings.Get("voiceDataPath"));

			Run().GetAwaiter().GetResult();
		}

		static async Task Run()
		{
			DiscordClient discord = new DiscordClient(new DiscordConfiguration()
			{
				Token = ConfigurationManager.AppSettings.Get("botToken"),
				TokenType = TokenType.Bot,
				Intents = DiscordIntents.All
			});

			discord.GuildDownloadCompleted += OnGuildDownloadComplete;

			IServiceCollection services = new ServiceCollection().AddSingleton(guildDataReference);

			CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration
			{
				StringPrefixes = new[] { "!" },
				EnableMentionPrefix = false,
				Services = services.BuildServiceProvider()
			});

			discord.UseInteractivity(new InteractivityConfiguration
			{
				PollBehaviour = PollBehaviour.DeleteEmojis,
				Timeout = TimeSpan.FromSeconds(30)
			});

			//discord.GuildMemberAdded += MemberValidation.TrackMemberJoinGuild;
			//discord.GuildMemberRemoved += MemberValidation.TrackMemberLeaveGuild;

			//commands.RegisterCommands<ModerationModule>();
			//commands.RegisterCommands<MusicModule>();
			commands.RegisterCommands<VoiceChannelModule>();
			commands.RegisterConverter(new ServerRegionConverter());
			commands.RegisterConverter(new ChannelPublicityConverter());
			commands.RegisterConverter(new PermitDenyStringToBoolConverter());

			await discord.ConnectAsync();
			//TODO figure out why this isnt authenticated and fix it
			//await discord.UpdateStatusAsync(new DiscordActivity { Name = "streaming development", ActivityType = ActivityType.Streaming, StreamUrl = "https://twitch.tv/not__nugget"}, UserStatus.Online);

			await Task.Delay(-1);
		}

		static async Task OnGuildDownloadComplete(DiscordClient sender, GuildDownloadCompletedEventArgs e)
		{
			await AttemptRebuildVoiceRegisteredGuildDataAsync(sender);
		}

		static async Task AttemptRebuildVoiceRegisteredGuildDataAsync(DiscordClient client)
		{
			string error;
			if ((error = await guildDataReference.RebuildDeserializedDataFromClient(client)) == string.Empty)
			{
				client.Logger.Log(LogLevel.Information, "Data deserialized from disk");
				return;
			}

			//log the error
			client.Logger.Log(LogLevel.Information, error);
		}
	}
}
