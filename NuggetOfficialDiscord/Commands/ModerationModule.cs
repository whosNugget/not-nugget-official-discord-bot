using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Commands
{
	public class ModerationModule : BaseCommandModule
	{
		[Command("warn")]
		public async Task WarnCommand(CommandContext ctx)
		{
			/*this command should apply a warning the provided member. warnings will accumulate and can be retrieved by staff at any point in time*/
			await ctx.Message.RespondAsync($"Command **{nameof(SoftBanCommand)}** is currently not implemented");
		}

		[Command("mute")]
		public async Task MuteCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(MuteCommand)}** is currently not implemented");
		}

		[Command("sban")]
		public async Task SoftBanCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(SoftBanCommand)}** is currently not implemented");
		}

		[Command("ban")]
		public async Task BanCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(BanCommand)}** is currently not implemented");
		}

		[Command("clear")]
		public async Task ClearCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(BanCommand)}** is currently not implemented");
		}

		[Command("flush")]
		public async Task FlushCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(BanCommand)}** is currently not implemented");
		}

		[Command("purge")]
		public async Task PurgeCommand(CommandContext ctx, int purgeCount)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(BanCommand)}** is currently not implemented");
		}
		[Command("purge")]
		public async Task PurgeCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(BanCommand)}** is currently not implemented");
		}

		[Command("report")]
		public async Task ReportCommand(CommandContext ctx)
		{
			/**/
			await ctx.Message.RespondAsync($"Command **{nameof(BanCommand)}** is currently not implemented");
		}
	}
}
