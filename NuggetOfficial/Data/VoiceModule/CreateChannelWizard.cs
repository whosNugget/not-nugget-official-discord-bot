﻿using DSharpPlus.CommandsNext;
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
	/// <summary>
	/// 
	/// </summary>
	public struct VoiceChannelCreationData
	{
		/// <summary>
		/// 
		/// </summary>
		public static VoiceChannelCreationData CreationCancelled { get => new VoiceChannelCreationData { Cancelled = true }; }
		/// <summary>
		/// 
		/// </summary>
		public static VoiceChannelCreationData CreationErrored { get => new VoiceChannelCreationData { Success = false }; }

		/// <summary>
		/// 
		/// </summary>
		public bool Cancelled { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public bool Success { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public string ChannelName { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsLimited { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public int UserLimit { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public ChannelPublicity SelectedPublicity { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		public VoiceRegion SelectedRegion { get; private set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="resultData"></param>
		public VoiceChannelCreationData(CreateChannelWizard.ResultData resultData)
		{
			Cancelled = resultData.Cancelled;
			Success = !resultData.Cancelled && !resultData.TimedOut;
			ChannelName = resultData.ChannelName;
			IsLimited = resultData.IsLimited;
			UserLimit = resultData.UserLimit;

			switch (resultData.SelectedVisibility.Name)
			{
				case ":unlock:":
					SelectedPublicity = ChannelPublicity.Public;
					break;
				case ":closed_lock_with_key:":
					SelectedPublicity = ChannelPublicity.Private;
					break;
				case ":lock_with_ink_pen:":
					SelectedPublicity = ChannelPublicity.Hidden;
					break;
				case ":gem:":
					SelectedPublicity = ChannelPublicity.Supporter;
					break;
				default:
					SelectedPublicity = ChannelPublicity.Unknown;
					Success = false;
					break;
			}

			switch (resultData.SelectedRegion.Name)
			{
				case ":flag_br:":
					SelectedRegion = VoiceRegion.Brazil;
					break;
				case ":flag_eu:":
					SelectedRegion = VoiceRegion.Europe;
					break;
				case ":flag_hk:":
					SelectedRegion = VoiceRegion.HongKong;
					break;
				case ":flag_in:":
					SelectedRegion = VoiceRegion.India;
					break;
				case ":flag_jp:":
					SelectedRegion = VoiceRegion.Japan;
					break;
				case ":flag_ru:":
					SelectedRegion = VoiceRegion.Russia;
					break;
				case ":flag_sg:":
					SelectedRegion = VoiceRegion.Singapore;
					break;
				case ":flag_za:":
					SelectedRegion = VoiceRegion.SouthAfrica;
					break;
				case ":flag_au:":
					SelectedRegion = VoiceRegion.Sydney;
					break;
				case ":arrow_up:":
					SelectedRegion = VoiceRegion.USCentral;
					break;
				case ":arrow_right:":
					SelectedRegion = VoiceRegion.USEast;
					break;
				case ":arrow_down:":
					SelectedRegion = VoiceRegion.USSouth;
					break;
				case ":arrow_left:":
					SelectedRegion = VoiceRegion.USWest;
					break;
				case ":green_square:":
					SelectedRegion = VoiceRegion.Automatic;
					break;
				default:
					SelectedRegion = VoiceRegion.Unknown;
					Success = false;
					break;
			}
		}
	}

	//TODO i want to refactor this to make it much tidyer...idk its just ugly to me rn
	/// <summary>
	/// 
	/// </summary>
	public class CreateChannelWizard
	{
		enum AllowedFlag { None = 0, Rename = 1, Public = 2, Private = 4, Hidden = 8, Supporter = 16, Limited = 32, RegionEdit = 64 }
		public struct ResultData
		{
			public bool TimedOut { get; set; }
			public bool Cancelled { get; set; }
			public string ChannelName { get; set; }
			public bool IsLimited { get; set; }
			public int UserLimit { get; set; }
			public DiscordEmoji SelectedVisibility { get; set; }
			public DiscordEmoji SelectedRegion { get; set; }
		}

		static bool emojiPopulated = false;
		//
		static DiscordEmoji yes;
		static DiscordEmoji no;
		//
		static DiscordEmoji cancel;
		//
		static DiscordEmoji @public;
		static DiscordEmoji @private;
		static DiscordEmoji hidden;
		static DiscordEmoji supporter;
		//
		static DiscordEmoji brazil;
		static DiscordEmoji europe;
		static DiscordEmoji hongkong;
		static DiscordEmoji india;
		static DiscordEmoji japan;
		static DiscordEmoji russia;
		static DiscordEmoji singapore;
		static DiscordEmoji southafrica;
		static DiscordEmoji sydney;
		static DiscordEmoji uscentral;
		static DiscordEmoji useast;
		static DiscordEmoji ussouth;
		static DiscordEmoji uswest;
		static DiscordEmoji auto;
		//
		static IEnumerable<DiscordEmoji> regionList;

		readonly CommandContext ctx = null;
		readonly GuildData guildData = null;

		AllowedFlag allowedFlag = AllowedFlag.None;
		DiscordMessage responseInteractionMessage = null;
		ResultData result = new ResultData();

		public CreateChannelWizard(CommandContext ctx, GuildData guildData)
		{
			this.ctx = ctx;
			this.guildData = guildData;
			GenerateAllowedFlag();
			if (!emojiPopulated) PopulateEmoji();
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

		void PopulateEmoji()
		{
			yes = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:", false);
			no = DiscordEmoji.FromName(ctx.Client, ":negative_squared_cross_mark:", false);
			cancel = DiscordEmoji.FromName(ctx.Client, ":no_entry_sign:", false);

			@public = DiscordEmoji.FromName(ctx.Client, ":unlock:", false);
			@private = DiscordEmoji.FromName(ctx.Client, ":closed_lock_with_key:", false);
			hidden = DiscordEmoji.FromName(ctx.Client, ":lock_with_ink_pen:", false);
			supporter = DiscordEmoji.FromName(ctx.Client, ":gem:", false);

			brazil = DiscordEmoji.FromName(ctx.Client, ":flag_br:", false);
			europe = DiscordEmoji.FromName(ctx.Client, ":flag_eu:", false);
			hongkong = DiscordEmoji.FromName(ctx.Client, ":flag_hk:", false);
			india = DiscordEmoji.FromName(ctx.Client, ":flag_in:", false);
			japan = DiscordEmoji.FromName(ctx.Client, ":flag_jp:", false);
			russia = DiscordEmoji.FromName(ctx.Client, ":flag_ru:", false);
			singapore = DiscordEmoji.FromName(ctx.Client, ":flag_sg:", false);
			southafrica = DiscordEmoji.FromName(ctx.Client, ":flag_za:", false);
			sydney = DiscordEmoji.FromName(ctx.Client, ":flag_au:", false);
			uscentral = DiscordEmoji.FromName(ctx.Client, ":arrow_up:", false);
			useast = DiscordEmoji.FromName(ctx.Client, ":arrow_right:", false);
			ussouth = DiscordEmoji.FromName(ctx.Client, ":arrow_down:", false);
			uswest = DiscordEmoji.FromName(ctx.Client, ":arrow_left:", false);
			auto = DiscordEmoji.FromName(ctx.Client, ":green_square:", false);

			regionList = new[] { brazil, europe, hongkong, india, japan, russia, singapore, southafrica, sydney, uscentral, useast, ussouth, uswest, auto };

			emojiPopulated = true;
		}

		public async Task<VoiceChannelCreationData> CreateResponseMessage()
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

			if (error) return VoiceChannelCreationData.CreationErrored;

			IEnumerable<Func<Task>> steps = CreateWizardSteps();
			foreach (Func<Task> task in steps)
			{
				await PreTask();
				await task();
				await PostTask();

				if (result.TimedOut) return VoiceChannelCreationData.CreationErrored;
				if (result.Cancelled) return VoiceChannelCreationData.CreationCancelled;
			}

			return await AwaitCompletedSteps();
		}

		IEnumerable<Func<Task>> CreateWizardSteps()
		{
			List<Func<Task>> taskSteps = new List<Func<Task>>();
			if (allowedFlag.HasFlag(AllowedFlag.Rename)) taskSteps.Add(AwaitRenameInteraction);
			if (allowedFlag.HasFlag(AllowedFlag.Private | AllowedFlag.Hidden | AllowedFlag.Supporter)) taskSteps.Add(AwaitAccesibilityInteraction);
			if (allowedFlag.HasFlag(AllowedFlag.Limited)) taskSteps.Add(AwaitLimitedInteraction);
			if (allowedFlag.HasFlag(AllowedFlag.RegionEdit)) taskSteps.Add(AwaitRegionInteraction);
			return taskSteps;
		}

		async Task PreTask()
		{
			await responseInteractionMessage?.DeleteAllReactionsAsync("Creation wizard step advancement - Clear previous emojis - PreTask()");
		}

		async Task PostTask()
		{
			if (result.TimedOut) { await ResultTimedOut(); return; }
			if (result.Cancelled) await WizardCancelled();
		}

		async Task AwaitRenameInteraction()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel Rename", $"{ctx.Member.Mention}: Do you want to reaname the channel?", DiscordColor.Blue, "Wizard rename step").Build());

			await responseInteractionMessage.CreateReactionAsync(yes);
			await responseInteractionMessage.CreateReactionAsync(no);
			await responseInteractionMessage.CreateReactionAsync(cancel);

			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			this.result.TimedOut = result.TimedOut;

			if (this.result.TimedOut || result.Result.Emoji.Equals(no) || (this.result.Cancelled = result.Result.Emoji.Equals(cancel))) return;

			await AwaitRenameResponse();
		}

		async Task AwaitRenameResponse()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel Rename", $"{ctx.Member.Mention}: The first 25 characters of your next message will be the new name of your channel", DiscordColor.Blue, "Wizard rename step").Build());

			InteractivityResult<DiscordMessage> result = await ctx.Channel.GetNextMessageAsync(ctx.Member);
			this.result.TimedOut = result.TimedOut;

			if (!this.result.TimedOut && !string.IsNullOrWhiteSpace(result.Result.Content)) this.result.ChannelName = result.Result.Content.Substring(0, result.Result.Content.Length >= 25 ? 25 : result.Result.Content.Length);
		}

		//TODO upgrade so it can be multiple, not just one
		async Task AwaitAccesibilityInteraction()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel accesibility", $"{ctx.Member.Mention}: Who would you like to allow access to the channel by default?", DiscordColor.Blue, "Wizard accesibility step").Build());

			if (allowedFlag.HasFlag(AllowedFlag.Public)) await responseInteractionMessage.CreateReactionAsync(@public);
			if (allowedFlag.HasFlag(AllowedFlag.Private)) await responseInteractionMessage.CreateReactionAsync(@private);
			if (allowedFlag.HasFlag(AllowedFlag.Hidden)) await responseInteractionMessage.CreateReactionAsync(hidden);
			if (allowedFlag.HasFlag(AllowedFlag.Supporter)) await responseInteractionMessage.CreateReactionAsync(supporter);
			await responseInteractionMessage.CreateReactionAsync(cancel);

			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			this.result.TimedOut = result.TimedOut;

			if (this.result.TimedOut || result.Result.Emoji.Equals(no) || (this.result.Cancelled = result.Result.Emoji.Equals(cancel))) return;

			this.result.SelectedVisibility = result.Result.Emoji;
		}

		async Task AwaitLimitedInteraction()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Specify user limit on channel", $"{ctx.Member.Mention}: Do you want the channel to be limited to a certain number of connected users, or free for allowed members to join?", DiscordColor.Blue, "Wizard limited user step").Build());

			await responseInteractionMessage.CreateReactionAsync(yes);
			await responseInteractionMessage.CreateReactionAsync(no);
			await responseInteractionMessage.CreateReactionAsync(cancel);

			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			if (this.result.TimedOut = result.TimedOut || result.Result.Emoji.Equals(no) || (this.result.Cancelled = result.Result.Emoji.Equals(cancel))) return;
			this.result.IsLimited = true;

			await AwaitLimitedNumberResponse();
		}

		async Task AwaitLimitedNumberResponse()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Enter channel user limit", $"{ctx.Member.Mention}: Please send a message containing only a number between 2 and 99. This number will be the maximum allowed connected users for the channel", DiscordColor.Blue, "Wizard limited user step").Build());
			int outResult = 0;

			InteractivityResult<DiscordMessage> result = await ctx.Channel.GetNextMessageAsync(ctx.Member);
			_ = int.TryParse(result.Result.Content, out outResult);
			bool success = outResult > 1 && outResult <= 99;

			if (this.result.TimedOut = result.TimedOut || !success) return;
			this.result.UserLimit = outResult;
		}

		//TODO bitrate

		async Task AwaitRegionInteraction()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Select channel region", $"{ctx.Member.Mention}: Please react to this message with the desired region the voice channel should use. React with :green_square: to automatically select the region", DiscordColor.Blue, "Wizard region select step").Build());

			foreach (var emoji in regionList)
			{
				await responseInteractionMessage.CreateReactionAsync(emoji);
			}
			await responseInteractionMessage.CreateReactionAsync(cancel);

			InteractivityResult<MessageReactionAddEventArgs> result = await responseInteractionMessage.WaitForReactionAsync(ctx.Member);
			if (this.result.TimedOut = result.TimedOut || (this.result.Cancelled = result.Result.Emoji.Equals(cancel))) return;

			this.result.SelectedRegion = result.Result.Emoji;
		}

		async Task<VoiceChannelCreationData> AwaitCompletedSteps()
		{
			return await Task.FromResult(new VoiceChannelCreationData(result));
		}

		async Task WizardCancelled()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Cancelled", $"{ctx.Member.Mention} cancelled the creation of the channel", DiscordColor.Red, "Error: Creation cancelled").Build());
		}

		async Task ResultTimedOut()
		{
			await responseInteractionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Timed out", "The interactivity timeout for this wizard was reached", DiscordColor.Red, "Error: Timed out").Build());
		}

		DiscordEmbedBuilder GetBuilder(string title, string description, DiscordColor color, string footer)
		{
			return new DiscordEmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color).WithFooter(footer).WithThumbnail(ctx.Member.AvatarUrl);
		}
	}
}