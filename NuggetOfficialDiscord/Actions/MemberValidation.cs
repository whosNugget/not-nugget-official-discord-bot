using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace NuggetDiscordBot.Actions
{
	public static class MemberValidation
	{
		public static async Task OnMemberJoinGuild(DiscordClient member, GuildMemberAddEventArgs args)
		{
			/*See if this member exists in the tracked information and perform an action if they violated a rule*/
			await Task.CompletedTask;
		}

		public static async Task OnMemberLeaveGuild(DiscordClient member, GuildMemberRemoveEventArgs args)
		{
			/*Should track information about the member as they leave, primarily if they were currently muted or soft-banned, and save it somewhere so information can be retrieved on rejoin in case they attemted to evade a mute/sban*/
			await Task.CompletedTask;
		}
	}
}
