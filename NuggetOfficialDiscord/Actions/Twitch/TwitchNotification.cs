using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Actions.Twitch
{
	public class TwitchNotification
	{
		/// <summary>
		/// The channel this notifier will notify when fired
		/// </summary>
		public DiscordChannel NotificationChannel { get; }
		/// <summary>
		/// The role the message will mention. When null, will mention everyone
		/// </summary>
		public DiscordRole MentionRole { get; }
		/// <summary>
		/// Specifies whether or not the bot will publish the message to announcement channels
		/// </summary>
		public bool ShouldPublish { get; }

		/// <summary>
		/// Create a new notifier with the provided data
		/// </summary>
		/// <param name="notificationChannel">Channel to notify</param>
		/// <param name="mentionRole">Role to mention</param>
		/// <param name="shouldPublish">Should the bot publish the mention for announcement channels</param>
		public TwitchNotification(DiscordChannel notificationChannel, DiscordRole mentionRole, bool shouldPublish = true)
		{
			NotificationChannel = notificationChannel;
			MentionRole = mentionRole;
			ShouldPublish = shouldPublish;
		}

		/// <summary>
		/// Creates and sends an embed with the provided data to the current notification channel, and crossposts it to all subscribing channels
		/// </summary>
		/// <param name="channelName">Name of the channel who went live</param>
		/// <param name="channelTitle">Title of the stream</param>
		/// <param name="channelGame">Name of the game being played</param>
		/// <param name="channelImageUrl">URL of the channel's profile picture</param>
		/// <returns>Task representing the outcome of this method</returns>
		public async Task OnChannelGoLive(string channelName, string channelTitle, string channelGame, string streamThumbnail, string channelImageUrl)
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithTitle($"{channelName} has gone live!")
				.AddField(channelTitle, $"Playing: {channelGame}")
				.WithUrl($"http://www.twitch.tv/{channelName}")
				.WithThumbnail(streamThumbnail)
				.WithImageUrl(channelImageUrl)
				.WithColor(DiscordColor.Orange)
				.WithFooter("Hope to see you there!");

			await NotificationChannel.CrosspostMessageAsync(await NotificationChannel.SendMessageAsync($"{MentionRole.Mention} - http://www.twitch.tv/{channelName}", builder.Build()));
		}
	}
}
