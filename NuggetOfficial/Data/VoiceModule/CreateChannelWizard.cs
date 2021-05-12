using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using NuggetOfficial.Authority;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficial.Data.VoiceModule
{
	public class CreateChannelWizard
	{
		enum AllowedFlag { None = 0, Rename = 1, Public = 2, Private = 4, Hidden = 8, Supporter = 16, Limited = 32, RegionEdit = 64 }

		readonly CommandContext ctx = null;
		readonly GuildData guildData = null;

		AllowedFlag allowedFlag = AllowedFlag.None;
		DiscordMessage responseInteractionMessage = null;
		bool timedOut = false;

		public CreateChannelWizard(CommandContext ctx, GuildData guildData)
		{
			this.ctx = ctx;
			this.guildData = guildData;
			GenerateAllowedFlag();
		}

		void GenerateAllowedFlag()
		{
			VoiceChannelConfigurationPermissions permissions = guildData.GetMemberPermissions(ctx.Member);

			if (permissions.ChannelRenameAuthority == ChannelRenameAuthority.Authorized) allowedFlag |= AllowedFlag.Rename;
			if (permissions.ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Private)) allowedFlag |= AllowedFlag.Private;
			if (permissions.ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Hidden)) allowedFlag |= AllowedFlag.Hidden;
			if (permissions.ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Supporter)) allowedFlag |= AllowedFlag.Supporter;
			if (permissions.ChannelAccesibilityConfigurationAuthority.HasFlag(ChannelAccesibilityConfigurationAuthority.Limited)) allowedFlag |= AllowedFlag.Limited;
			if (permissions.ChannelRegionConfigurationAuthority == ChannelRegionConfigurationAuthority.Authorized) allowedFlag |= AllowedFlag.RegionEdit;
		}

		public async Task CreateResponseMessage()
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder().WithTitle("Channel creation wizard").WithThumbnail(ctx.Member.AvatarUrl);
			bool error = false;

			if (allowedFlag == AllowedFlag.None)
			{
				builder.WithColor(DiscordColor.Red);
				builder.WithDescription($"{ctx.Member.Mention} does not have permission to create voice channels");
				builder.WithFooter("Error");
				error = true;
			}

			responseInteractionMessage = await ctx.Message.RespondAsync(builder.Build());

			if (error) return;

			IEnumerable<Func<Task>> steps = CreateWizardSteps();
			foreach (Func<Task> task in steps)
			{
				await PreTask();
				await task();
				await PostTask();

				if (timedOut)
				{
					await ResultTimedOut();
					return;
				}
			}
		}

		IEnumerable<Func<Task>> CreateWizardSteps()
		{
			List<Func<Task>> taskSteps = new List<Func<Task>>();
			if (allowedFlag.HasFlag(AllowedFlag.Rename)) taskSteps.Add(AwaitRenameResponse);
			if (allowedFlag.HasFlag(AllowedFlag.Private | AllowedFlag.Hidden | AllowedFlag.Supporter)) taskSteps.Add(AwaitAccesibilityInteraction);
			if (allowedFlag.HasFlag(AllowedFlag.Limited)) taskSteps.Add(AwaitLimitedInteraction);
			if (allowedFlag.HasFlag(AllowedFlag.RegionEdit)) taskSteps.Add(AwaitRegionInteraction);
			return taskSteps;
		}

		async Task PreTask()
		{
			await responseInteractionMessage.DeleteAllReactionsAsync("Creation wizard step advancement - Clear previous emojis - PreTask()");
		}

		async Task PostTask()
		{
		}

		async Task AwaitRenameResponse()
		{
			InteractivityResult<DiscordMessage> result = await responseInteractionMessage.GetNextMessageAsync(msg => true);
			timedOut = result.TimedOut;
		}

		async Task AwaitAccesibilityInteraction()
		{

			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			timedOut = result.TimedOut;
		}

		async Task AwaitLimitedInteraction()
		{
			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			timedOut = result.TimedOut;

			//If not timed out and a limited channel was selected, begin waiting for the response
		}

		async Task AwaitLimitedNumberResponse()
		{
			InteractivityResult<DiscordMessage> result = await responseInteractionMessage.GetNextMessageAsync(msg => true);
			timedOut = result.TimedOut;
		}

		async Task AwaitRegionInteraction()
		{
			
			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			timedOut = result.TimedOut;
		}

		async Task ResultTimedOut()
		{
			DiscordEmbedBuilder builder = new DiscordEmbedBuilder().WithTitle("Channel creation wizard - Timed out").WithDescription("The interactivity timeout for this wizard was reached").WithColor(DiscordColor.Red).WithFooter("Error: Timed out");
			await responseInteractionMessage.ModifyAsync(builder.Build());
		}
	}
}
