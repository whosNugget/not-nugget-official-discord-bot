using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuggetOfficial.Actions.Serialization;
using NuggetOfficial.Bot;
using NuggetOfficial.Commands;
using NuggetOfficial.Data.Converters;
using NuggetOfficial.Data.VoiceModule;
using System;
using System.Threading.Tasks;

namespace NuggetOfficial
{
	public class NuggetBot : DiscordBot
	{
		readonly VoiceRegisteredGuildData guildDataReference = null;

		/// <summary>
		/// Create a new bot instance without attempting to deserialize existing information
		/// </summary>
		public NuggetBot() { }
		/// <summary>
		/// Create a new bot instance, giving it a location to attempt deserialization of existing information from
		/// </summary>
		/// <param name="deserializePath">Path to attempt deserializing of existing information</param>
		public NuggetBot(string deserializePath)
		{
			guildDataReference = Serializer.Deserialize<VoiceRegisteredGuildData>(deserializePath);
		}

		/// <summary>
		/// Run the bot. Currently, this method never returns controll to the calling class. This will not always be the case, but 
		/// </summary>
		/// <param name="botToken"></param>
		/// <returns></returns>
		public override async Task Run(string botToken)
		{
			DiscordClient discord = new DiscordClient(new DiscordConfiguration()
			{
				Token = botToken,
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

			//TODO need to find a better way to return control without terminating an application
			await Task.Delay(-1);
		}

		async Task OnGuildDownloadComplete(DiscordClient sender, GuildDownloadCompletedEventArgs e)
		{
			await AttemptRebuildVoiceRegisteredGuildDataAsync(sender);
		}

		async Task AttemptRebuildVoiceRegisteredGuildDataAsync(DiscordClient client)
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
