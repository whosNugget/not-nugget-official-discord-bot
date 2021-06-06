using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NuggetOfficialDiscord.Actions.Twitch;
using NuggetTwitchIntegration.LiveMonitor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuggetDiscordBot.Commands
{
	public class TwitchNotificationModule : BaseCommandModule
	{
		TwitchLiveMonitor monitor = null;
		TwitchNotification notifier = null;

		[Command("inittwitchnotify")]
		public async Task RegisterNotificationChannel(CommandContext ctx, DiscordChannel channel, DiscordRole mentionRole = null, params string[] channelNotifyList)
		{			
			if (channelNotifyList is null || channelNotifyList.Length == 0) return; //TODO change

			if (notifier is null)
			{
				notifier = new TwitchNotification(channel, mentionRole);
			}

			if (monitor is null)
			{
				monitor = new TwitchLiveMonitor(new List<string>(channelNotifyList), notifier.OnChannelGoLive);
			}

			DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
				.WithTitle("Twitch Notification Initialization")
				.WithDescription($"Notifications will be sent to {channel.Mention}{(mentionRole is null ? "" : $", and users with the role {mentionRole.Mention} will be notified")}");

			foreach (string twtichChannel in channelNotifyList) builder.AddField("monitoring channel:", twtichChannel, true);

			builder.WithColor(DiscordColor.Green)
				.WithFooter("Sucess");

			await ctx.RespondAsync(builder.Build());
		}

		[Command("testnotify")]
		public async Task TestNotify(CommandContext ctx)
		{
			await notifier.OnChannelGoLive("not__nugget", "test notification", "test game", "https://www.google.com/url?sa=i&url=https%3A%2F%2Fknowpathology.com.au%2F2018%2F07%2F26%2Fwhat-is-a-screening-test%2F&psig=AOvVaw30uPxEqHNzuAR7zAD_csnk&ust=1621202583954000&source=images&cd=vfe&ved=0CAIQjRxqFwoTCMDjirXYzPACFQAAAAAdAAAAABAD", "https://static-cdn.jtvnw.net/jtv_user_pictures/776ee5f2-cbe1-41ec-87c3-009260392cb5-profile_image-150x150.png");
		}
	}
}
