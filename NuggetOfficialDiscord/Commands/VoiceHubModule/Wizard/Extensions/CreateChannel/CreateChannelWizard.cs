using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions;
using NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Wizard
{
    /// <summary>
    /// Channel creation wizard object. Use this object to initialize and advance the wizard steps
    /// </summary>
    public class CreateChannelWizard : EmbedInteractionWizard<CreateChannelWizardResult>
    {
        #region Private variables
        private static readonly string[] channelPublicity = { ":unlock:", ":closed_lock_with_key:", ":lock_with_ink_pen:", ":gem:" };
        private static readonly string[] regionList = { ":flag_br:", ":flag_eu:", ":flag_hk:", ":flag_in:", ":flag_ru:", ":flag_sg:", ":flag_za:", ":flag_au:", ":flag_us:", ":green_square:" };
        private static readonly string[] usaSubregionList = { ":arrow_up:", ":arrow_right:", ":arrow_down:", ":arrow_left:" };

		private readonly ChannelAuthorities authorities;
		#endregion

		#region Constructors
		//TODO see if it is possible to remove this duplicated code
		public CreateChannelWizard(CommandContext context, GuildData guildData) : base(context)
        {
            authorities = guildData.GetMemberPermissions(context.Member).Authorities;
            if (!emojiPopulated) InitializeEmojiContainer();
        }
        public CreateChannelWizard(CommandContext context, DiscordChannel channel, GuildData guildData) : base(context, channel)
		{
            authorities = guildData.GetMemberPermissions(context.Member).Authorities;
            if (!emojiPopulated) InitializeEmojiContainer();
        }
		#endregion

		#region Overrides

        public override async Task SetupWizard()
        {
            if (authorities == ChannelAuthorities.CompletelyUnauthorized || !authorities.HasFlag(ChannelAuthorities.CanCreateChannels))
            {
                result.Valid = false;
                result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                result.ErrorString = "Calling member does not have permissin to create voice channels";
                return;
            }

            DiscordEmbedBuilder builder = new DiscordEmbedBuilder().WithTitle("Channel creation wizard").WithThumbnail(context.Member.DefaultAvatarUrl);
            interactionMessage = await context.Message.RespondAsync(builder.Build());

            wizardSteps.AddRange(CreateWizardSteps());
        }

        public override async Task<CreateChannelWizardResult> GetResult()
        {
            int stepIndex = 0;
            do
            {
                await PreStep();
                await wizardSteps[stepIndex]();
                await PostStep();
            }
            while (result.Valid && ++stepIndex < wizardSteps.Count);

            return await Task.FromResult(result);
        }

		protected override void InitializeEmojiContainer()
		{
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":white_check_mark:", false), true);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":negative_squared_cross_mark:", false), false);
            reactionEmotes.SetEmojiValue<object>(DiscordEmoji.FromName(context.Client, ":no_entry_sign:", false), null);

            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":unlock:", false), ChannelAccessibility.Public);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":closed_lock_with_key:", false), ChannelAccessibility.Private);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":lock_with_ink_pen:", false), ChannelAccessibility.Hidden);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":gem:", false), ChannelAccessibility.Supporter);
            
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_br:", false), VoiceRegion.Brazil);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_eu:", false), VoiceRegion.Europe);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_hk:", false), VoiceRegion.HongKong);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_in:", false), VoiceRegion.India);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_ru:", false), VoiceRegion.Russia);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_sg:", false), VoiceRegion.Singapore);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_za:", false), VoiceRegion.SouthAfrica);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_au:", false), VoiceRegion.Sydney);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":flag_us:", false), VoiceRegion.US);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":arrow_up:", false), VoiceRegion.USCentral);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":arrow_right:", false), VoiceRegion.USEast);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":arrow_down:", false), VoiceRegion.USSouth);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":arrow_left:", false), VoiceRegion.USWest);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":green_square:", false), VoiceRegion.USWest);
		}
        
        protected override IEnumerable<Func<Task>> CreateWizardSteps()
        {
            List<Func<Task>> taskSteps = new List<Func<Task>>();
            if (authorities.HasFlag(ChannelAuthorities.CanRenameChannels)) taskSteps.Add(AwaitRenameInteraction);
            if (authorities.HasFlag(ChannelAuthorities.CanCreatePrivateChannels)
                || authorities.HasFlag(ChannelAuthorities.CanCreateSupporterChannels)
                || authorities.HasFlag(ChannelAuthorities.CanCreateHiddenChannels))
                taskSteps.Add(AwaitAccesibilityInteraction);
            if (authorities.HasFlag(ChannelAuthorities.CanCreateLimitedChannels)) taskSteps.Add(AwaitLimitedInteraction);
            if (authorities.HasFlag(ChannelAuthorities.CanModifyChannelBitrate)) taskSteps.Add(AwaitBitrateInteraction);
            if (authorities.HasFlag(ChannelAuthorities.CanModifyChannelRegion)) taskSteps.Add(AwaitRegionInteraction);
            return taskSteps;
        }

        protected override async Task PreStep()
        {
            await interactionMessage?.DeleteAllReactionsAsync("Creation wizard step advancement - Clear previous emojis - PreTask()");
        }

        protected override async Task PostStep()
        {
            if (!result.Valid)
            {
                if (result.InvalidationReason == WizardInvalidationReason.Cancelled) await WizardCancelledResponse();
                if (result.InvalidationReason == WizardInvalidationReason.TimedOut) await ResultTimedOutResponse();
                if (result.InvalidationReason == WizardInvalidationReason.UnknownError) await UnknownErrorResponse();
            }
        }
        #endregion

        #region Private Helper Methods
        private bool CheckResultCancelled(InteractivityResult<MessageReactionAddEventArgs> result)
        {
            bool cancelled;

            if (cancelled = result.Result.Emoji.Equals(reactionEmotes.Cancel))
            {
                this.result.Valid = false;
                this.result.InvalidationReason = WizardInvalidationReason.Cancelled;
                this.result.ErrorString = "The request was cancelled by the user";
            }

            return cancelled;
        }

        private bool CheckResultTimedOut<T>(InteractivityResult<T> result)
        {
            if (result.TimedOut)
            {
                this.result.Valid = false;
                this.result.InvalidationReason = WizardInvalidationReason.TimedOut;
                this.result.ErrorString = "The request timed out";
            }

            return result.TimedOut;
        }

        private VoiceRegion GetVoiceRegionFromEmoji(DiscordEmoji emoji)
        {
            if (!reactionEmotes.GetEmojiValue(emoji, out VoiceRegion region))
			{
                region = VoiceRegion.Unknown;
                result.Valid = false;
				result.InvalidationReason = WizardInvalidationReason.InvalidInput;
				result.ErrorString = "The provided emoji was not a valid voice region emoji";
			}

            return region;
        }

        private ChannelAccessibility GetChannelAccessabilityFromEmoji(DiscordEmoji emoji)
        {
            if (!reactionEmotes.GetEmojiValue(emoji, out ChannelAccessibility accessibility))
			{
                accessibility = ChannelAccessibility.Unknown;
                result.Valid = false;
                result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                result.ErrorString = "The provided emoji was not a valid accessability emoji";
            }

            return accessibility;
        }

        private async Task WizardCancelledResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Cancelled", $"{context.Member.Mention} cancelled the creation of the channel", DiscordColor.Red, "Error: Creation cancelled").Build());
        }

        private async Task ResultTimedOutResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Timed out", "The interactivity timeout for this wizard was reached", DiscordColor.Red, "Error: Timed out").Build());
        }

        private async Task UnknownErrorResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Unknown error", "An unknown error has occurred. Please try again. If this keeps occuring, reach out to a server admin", DiscordColor.Red, "Error: Unknown").Build());
        }
        #endregion

        #region Wizard Task Steps
        private async Task AwaitRenameInteraction()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel Rename", $"{context.Member.Mention}: Do you want to reaname the channel?", DiscordColor.Blue, "Wizard rename step").Build());

            await interactionMessage.CreateReactionAsync(reactionEmotes.Yes);
            await interactionMessage.CreateReactionAsync(reactionEmotes.No);
            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(reactionEmotes.No)) return;

            await AwaitRenameResponse();
        }

        private async Task AwaitRenameResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel Rename", $"{context.Member.Mention}: The first 25 characters of your next message will be the new name of your channel", DiscordColor.Blue, "Wizard rename step").Build());

            var result = await context.Channel.GetNextMessageAsync(context.Member);
            if (CheckResultTimedOut(result)) return;

            if (string.IsNullOrWhiteSpace(result.Result.Content))
            {
                this.result.Valid = false;
                this.result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                return;
            }

            this.result.ChannelName = result.Result.Content.Substring(0, result.Result.Content.Length >= 25 ? 25 : result.Result.Content.Length);
        }

        private async Task AwaitAccesibilityInteraction()
        {
            //Cool idea but a little impractical - Keep this around though because it was a fun mini rabbit hole
            //ChannelAuthorities totalAuth = ChannelAuthorities.CanCreateAnyChannel;
            //ChannelAuthorities userAuth = totalAuth & authorities;
            //int hiMod = (int)totalAuth % (int)userAuth;
            //int loMod = (int)userAuth % (int)totalAuth;
            //if (hiMod < loMod) return; //NEED TO TEST, BUT I BELIEVE WHEN HI IS < THAN LO, THE USER CAN ONLY CREATE ONE 

            int userAuthInt = (int)(authorities & ChannelAuthorities.CanCreateAnyChannel);
            if (userAuthInt == 32 || userAuthInt == 64 || userAuthInt == 128 || userAuthInt == 256)
            {
                this.result.ChannelAccessability = userAuthInt == 32 ? ChannelAccessibility.Private : userAuthInt == 64 ? ChannelAccessibility.Public : userAuthInt == 128 ? ChannelAccessibility.Hidden : ChannelAccessibility.Supporter;
                return;
            }

            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel accesibility", $"{context.Member.Mention}: Who would you like to allow access to the channel by default?", DiscordColor.Blue, "Wizard accesibility step").Build());

            if (authorities.HasFlag(ChannelAuthorities.CanCreatePublicChannels)) await interactionMessage.CreateReactionAsync(reactionEmotes[channelPublicity[0]]);
            if (authorities.HasFlag(ChannelAuthorities.CanCreatePrivateChannels)) await interactionMessage.CreateReactionAsync(reactionEmotes[channelPublicity[1]]);
            if (authorities.HasFlag(ChannelAuthorities.CanCreateHiddenChannels)) await interactionMessage.CreateReactionAsync(reactionEmotes[channelPublicity[2]]);
            if (authorities.HasFlag(ChannelAuthorities.CanCreateSupporterChannels)) await interactionMessage.CreateReactionAsync(reactionEmotes[channelPublicity[3]]);
            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            this.result.ChannelAccessability = GetChannelAccessabilityFromEmoji(result.Result.Emoji);
        }

        private async Task AwaitLimitedInteraction()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Specify user limit on channel", $"{context.Member.Mention}: Do you want the channel to be limited to a certain number of connected users, or free for allowed members to join?", DiscordColor.Blue, "Wizard limited user step").Build());

            await interactionMessage.CreateReactionAsync(reactionEmotes.Yes);
            await interactionMessage.CreateReactionAsync(reactionEmotes.No);
            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(reactionEmotes.No)) return;

            await AwaitLimitedNumberResponse();
        }

        private async Task AwaitLimitedNumberResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Enter channel user limit", $"{context.Member.Mention}: Please send a message containing only a number between 2 and 99. This number will be the maximum allowed connected users for the channel", DiscordColor.Blue, "Wizard limited user step").Build());

            InteractivityResult<DiscordMessage> result = await context.Channel.GetNextMessageAsync(context.Member);
            if (CheckResultTimedOut(result)) return;

            if (!(int.TryParse(result.Result.Content, out int outResult) && outResult > 1 && outResult <= 99))
            {
                this.result.Valid = false;
                this.result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                this.result.ErrorString = "The provided user limit string was invalid. It must be a whole number between 1 and 99";
                return;
            }

            this.result.UserLimit = outResult;
        }

        private async Task AwaitBitrateInteraction()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Change channel bitrate", "Would you like to change the channel's bitrate?", DiscordColor.Blue, "Wizard bitrate step").Build());

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(reactionEmotes.No))
            {
                this.result.Bitrate = 64000;
                return;
            }

            await AwaitBitrateResponse();
        }

        private async Task AwaitBitrateResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Change channel bitrate",
                "Enter a valid number of your desired bitrate",
                DiscordColor.Blue,
                "Wizard bitrate step")
                .AddField("Valid number range", "Enter a number between 8000 and 96000")
                .Build());

            var result = await context.Channel.GetNextMessageAsync(context.Member);
            if (CheckResultTimedOut(result)) return;

            if (!(int.TryParse(result.Result.Content, out int outResult) && outResult >= 8000 && outResult <= 96000))
            {
                this.result.Valid = false;
                this.result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                this.result.ErrorString = "The provided bitrate input was invalid. It must be a whole number between 8000 and 96000";
                return;
            }

            this.result.UserLimit = outResult;
        }

        private async Task AwaitRegionInteraction()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Select channel region", $"{context.Member.Mention}: Please react to this message with the desired region the voice channel should use. React with :green_square: to automatically select the region", DiscordColor.Blue, "Wizard region select step").Build());

            foreach (var name in regionList)
            {
                await interactionMessage.CreateReactionAsync(reactionEmotes[name]);
            }
            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(reactionEmotes[regionList[^1]]))
            {
                await AwaitUSSubregionInteraction();
                return;
            }

            this.result.ChannelVoiceRegion = GetVoiceRegionFromEmoji(result.Result.Emoji);
        }

        private async Task AwaitUSSubregionInteraction()
        {
            //That method doesn't have much removal of extra code so I am okay with having one of these in every task
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Select channel region",
                $"{context.Member.Mention}: Please react to this message with the desired region the voice channel should use. React with :green_square: to allow Discord to automatically select the region. React with :region_us: to show the US subregion wizard step.",
                DiscordColor.Blue,
                "Wizard region select step")
                .AddField("US Central", $"Use {reactionEmotes[usaSubregionList[0]]} to select the US Central region")
                .AddField("US East", $"Use {reactionEmotes[usaSubregionList[1]]} to select the US Central region")
                .AddField("US South", $"Use {reactionEmotes[usaSubregionList[2]]} to select the US Central region")
                .AddField("US West", $"Use {reactionEmotes[usaSubregionList[3]]} to select the US Central region")
                .Build());

			foreach (var name in usaSubregionList)
			{
                await interactionMessage.CreateReactionAsync(reactionEmotes[name]);
			}

            await interactionMessage.CreateReactionAsync(reactionEmotes[regionList[^2]]);
            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            this.result.ChannelVoiceRegion = GetVoiceRegionFromEmoji(result.Result.Emoji);
        }
        #endregion

        //TODO remove or make better...theres no reason not to use a default builder list. Either way there's gonna be a repition of code
        private DiscordEmbedBuilder GetBuilder(string title, string description, DiscordColor color, string footer)
        {
            return new DiscordEmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color).WithFooter(footer).WithThumbnail(context.Guild.IconUrl);
        }
    }
}
