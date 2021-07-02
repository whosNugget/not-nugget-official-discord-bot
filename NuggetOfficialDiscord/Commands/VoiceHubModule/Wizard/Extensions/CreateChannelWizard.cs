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
    public class CreateChannelWizard : EmbedInteractionWizard<WizardResult>
    {
        #region Emoji
        private static bool emojiPopulated = false;
        //
        private static DiscordEmoji yes;
        private static DiscordEmoji no;
        //
        private static DiscordEmoji cancel;
        //
        private static DiscordEmoji @public;
        private static DiscordEmoji @private;
        private static DiscordEmoji hidden;
        private static DiscordEmoji supporter;
        //
        private static DiscordEmoji brazil;
        private static DiscordEmoji europe;
        private static DiscordEmoji hongkong;
        private static DiscordEmoji india;
        private static DiscordEmoji japan;
        private static DiscordEmoji russia;
        private static DiscordEmoji singapore;
        private static DiscordEmoji southafrica;
        private static DiscordEmoji sydney;
        private static DiscordEmoji usa;
        private static DiscordEmoji auto;
        //
        private static DiscordEmoji uscentral;
        private static DiscordEmoji useast;
        private static DiscordEmoji ussouth;
        private static DiscordEmoji uswest;
        //
        private static IEnumerable<DiscordEmoji> regionList;
        #endregion

        private readonly ChannelAuthorities authorities;

        public CreateChannelWizard(CommandContext context, GuildData guildData) : base(context)
        {
            authorities = guildData.GetMemberPermissions(context.Member).Authorities;
            if (!emojiPopulated) PopulateEmoji();
        }

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

        public override async Task<WizardResult> GetResult()
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

        #region Private Setup Methods
        private IEnumerable<Func<Task>> CreateWizardSteps()
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

        private void PopulateEmoji()
        {
            yes = DiscordEmoji.FromName(context.Client, ":white_check_mark:", false);
            no = DiscordEmoji.FromName(context.Client, ":negative_squared_cross_mark:", false);
            cancel = DiscordEmoji.FromName(context.Client, ":no_entry_sign:", false);

            @public = DiscordEmoji.FromName(context.Client, ":unlock:", false);
            @private = DiscordEmoji.FromName(context.Client, ":closed_lock_with_key:", false);
            hidden = DiscordEmoji.FromName(context.Client, ":lock_with_ink_pen:", false);
            supporter = DiscordEmoji.FromName(context.Client, ":gem:", false);

            brazil = DiscordEmoji.FromName(context.Client, ":flag_br:", false);
            europe = DiscordEmoji.FromName(context.Client, ":flag_eu:", false);
            hongkong = DiscordEmoji.FromName(context.Client, ":flag_hk:", false);
            india = DiscordEmoji.FromName(context.Client, ":flag_in:", false);
            japan = DiscordEmoji.FromName(context.Client, ":flag_jp:", false);
            russia = DiscordEmoji.FromName(context.Client, ":flag_ru:", false);
            singapore = DiscordEmoji.FromName(context.Client, ":flag_sg:", false);
            southafrica = DiscordEmoji.FromName(context.Client, ":flag_za:", false);
            sydney = DiscordEmoji.FromName(context.Client, ":flag_au:", false);
            usa = DiscordEmoji.FromName(context.Client, ":flag_us:", false);
            uscentral = DiscordEmoji.FromName(context.Client, ":arrow_up:", false);
            useast = DiscordEmoji.FromName(context.Client, ":arrow_right:", false);
            ussouth = DiscordEmoji.FromName(context.Client, ":arrow_down:", false);
            uswest = DiscordEmoji.FromName(context.Client, ":arrow_left:", false);
            auto = DiscordEmoji.FromName(context.Client, ":green_square:", false);

            regionList = new[] { brazil, europe, hongkong, india, japan, russia, singapore, southafrica, sydney, usa, auto };

            emojiPopulated = true;
        }
        #endregion

        #region Private Helper Methods
        private bool CheckResultCancelled(InteractivityResult<MessageReactionAddEventArgs> result)
        {
            bool cancelled;

            if (cancelled = result.Result.Emoji.Equals(cancel))
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
            VoiceRegion region = VoiceRegion.Unknown;

            switch (emoji.Name)
            {
                case ":flag_br:":
                    region = VoiceRegion.Brazil;
                    break;
                case ":flag_eu:":
                    region = VoiceRegion.Europe;
                    break;
                case ":flag_hk:":
                    region = VoiceRegion.HongKong;
                    break;
                case ":flag_in:":
                    region = VoiceRegion.India;
                    break;
                case ":flag_jp:":
                    region = VoiceRegion.Japan;
                    break;
                case ":flag_ru:":
                    region = VoiceRegion.Russia;
                    break;
                case ":flag_sg:":
                    region = VoiceRegion.Singapore;
                    break;
                case ":flag_za:":
                    region = VoiceRegion.SouthAfrica;
                    break;
                case ":flag_au:":
                    region = VoiceRegion.Sydney;
                    break;
                case ":arrow_up:":
                    region = VoiceRegion.USCentral;
                    break;
                case ":arrow_right:":
                    region = VoiceRegion.USEast;
                    break;
                case ":arrow_down:":
                    region = VoiceRegion.USSouth;
                    break;
                case ":arrow_left:":
                    region = VoiceRegion.USWest;
                    break;
                case ":green_square:":
                    region = VoiceRegion.Automatic;
                    break;
                default:
                    result.Valid = false;
                    result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                    result.ErrorString = "The provided emoji was not a valid voice region emoji";
                    break;
            }

            return region;
        }

        private ChannelPublicity GetChannelAccessabilityFromEmoji(DiscordEmoji emoji)
        {
            ChannelPublicity publicity = ChannelPublicity.Unknown;

            switch (emoji.Name)
            {
                case ":unlock:":
                    publicity = ChannelPublicity.Public;
                    break;
                case ":closed_lock_with_key:":
                    publicity = ChannelPublicity.Private;
                    break;
                case ":lock_with_ink_pen:":
                    publicity = ChannelPublicity.Hidden;
                    break;
                case ":gem:":
                    publicity = ChannelPublicity.Supporter;
                    break;
                default:
                    result.Valid = false;
                    result.InvalidationReason = WizardInvalidationReason.InvalidInput;
                    result.ErrorString = "The provided emoji was not a valid accessability emoji";
                    break;
            }

            return publicity;
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

            await interactionMessage.CreateReactionAsync(yes);
            await interactionMessage.CreateReactionAsync(no);
            await interactionMessage.CreateReactionAsync(cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(no)) return;

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
                this.result.ChannelAccessability = userAuthInt == 32 ? ChannelPublicity.Private : userAuthInt == 64 ? ChannelPublicity.Public : userAuthInt == 128 ? ChannelPublicity.Hidden : ChannelPublicity.Supporter;
                return;
            }

            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Channel accesibility", $"{context.Member.Mention}: Who would you like to allow access to the channel by default?", DiscordColor.Blue, "Wizard accesibility step").Build());

            if (authorities.HasFlag(ChannelAuthorities.CanCreatePublicChannels)) await interactionMessage.CreateReactionAsync(@public);
            if (authorities.HasFlag(ChannelAuthorities.CanCreatePrivateChannels)) await interactionMessage.CreateReactionAsync(@private);
            if (authorities.HasFlag(ChannelAuthorities.CanCreateHiddenChannels)) await interactionMessage.CreateReactionAsync(hidden);
            if (authorities.HasFlag(ChannelAuthorities.CanCreateSupporterChannels)) await interactionMessage.CreateReactionAsync(supporter);
            await interactionMessage.CreateReactionAsync(cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            this.result.ChannelAccessability = GetChannelAccessabilityFromEmoji(result.Result.Emoji);
        }

        private async Task AwaitLimitedInteraction()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Channel creation wizard - Specify user limit on channel", $"{context.Member.Mention}: Do you want the channel to be limited to a certain number of connected users, or free for allowed members to join?", DiscordColor.Blue, "Wizard limited user step").Build());

            await interactionMessage.CreateReactionAsync(yes);
            await interactionMessage.CreateReactionAsync(no);
            await interactionMessage.CreateReactionAsync(cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(no)) return;

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

            if (result.Result.Emoji.Equals(no))
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

            foreach (var emoji in regionList)
            {
                await interactionMessage.CreateReactionAsync(emoji);
            }
            await interactionMessage.CreateReactionAsync(cancel);

            var result = await interactionMessage.WaitForReactionAsync(context.Member);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            if (result.Result.Emoji.Equals(usa))
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
                .AddField("US Central", $"Use {uscentral} to select the US Central region")
                .AddField("US East", $"Use {useast} to select the US Central region")
                .AddField("US South", $"Use {ussouth} to select the US Central region")
                .AddField("US West", $"Use {uswest} to select the US Central region")
                .Build());
            await interactionMessage.CreateReactionAsync(uscentral);
            await interactionMessage.CreateReactionAsync(useast);
            await interactionMessage.CreateReactionAsync(ussouth);
            await interactionMessage.CreateReactionAsync(uswest);
            await interactionMessage.CreateReactionAsync(auto);
            await interactionMessage.CreateReactionAsync(cancel);

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
