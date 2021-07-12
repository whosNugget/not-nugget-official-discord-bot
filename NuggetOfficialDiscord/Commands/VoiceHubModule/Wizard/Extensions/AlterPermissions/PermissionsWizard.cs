using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
    public class PermissionsWizard : EmbedInteractionWizard<PermissionsWizardResult>
    {
        #region Private Variables
        private static string[] createRenameReactions = new[] { ":eight_spoked_asterisk:", ":abcd:" };
        private static string[] createQuantityReactions = new[] { ":one:", ":1234:", ":infinity:" };
        private static string[] createAccessibilityReactions = new[] { ":lock:", ":unlock:", ":lock_with_ink_pen:", ":gem:" };

        private readonly DiscordRole[] roles;
        private readonly DiscordMember[] members;

        private readonly DiscordEmbedBuilder cachedBuilder;
        private bool complete = false;
        #endregion

        #region Constructors
        public PermissionsWizard(CommandContext context, DiscordRole[] roles, DiscordMember[] members) : base(context)
        {
            cachedBuilder = new DiscordEmbedBuilder();

            this.roles = roles;
            this.members = members;
        }
        public PermissionsWizard(CommandContext context, DiscordChannel responseChannel, DiscordRole[] roles, DiscordMember[] members) : base(context, responseChannel)
        {
            cachedBuilder = new DiscordEmbedBuilder();

            this.roles = roles;
            this.members = members;
        }
        #endregion

        #region Overrides
        public override async Task SetupWizard()
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder().WithTitle("Permissions wizard").WithThumbnail(context.Member.DefaultAvatarUrl);
            interactionMessage = await context.Message.RespondAsync(builder.Build());

            result = new();
            result.Authorizations = ChannelAuthorizations.Unauthorized;

            CreateWizardSteps();
        }

        public override async Task<PermissionsWizardResult> GetResult()
        {
            int stepIndex = 0;
            do
            {
                await PreStep();
                await wizardSteps[stepIndex]();
                await PostStep();
            }
            while (!complete && result.Valid && ++stepIndex < wizardSteps.Count);

            return await Task.FromResult(result);
        }

        protected override void InitializeEmojiContainer()
        {
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":eight_spoked_asterisk:", false), ChannelAuthorities.CanCreateChannels);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":abcd:", false), ChannelAuthorities.CanRenameChannels);

            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":one:", false), ChannelAuthorities.CanCreateSingleChannel);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":1234:", false), ChannelAuthorities.CanCreateMultipleChannels);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":infinity:", false), ChannelAuthorities.CanCreateInfiniteChannels);

            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":lock:", false), ChannelAuthorities.CanCreatePrivateChannels);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":unlock:", false), ChannelAuthorities.CanCreatePublicChannels);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":lock_with_ink_pen:", false), ChannelAuthorities.CanCreateHiddenChannels);
            reactionEmotes.SetEmojiValue(DiscordEmoji.FromName(context.Client, ":gem:", false), ChannelAuthorities.CanCreateSupporterChannels);
        }

        protected override void CreateWizardSteps()
        {
            wizardSteps.AddRange(new Func<Task>[] { AwaitCreateRenameReactions, AwaitCreationQuantityReaction, AwaitChannelAccessibilityReactions, AwaitChannelLimitReaction, AwaitChannelBitrateReaction, AwaitChannelRegionReaction });
        }

        protected override async Task PreStep()
        {
            await interactionMessage?.DeleteAllReactionsAsync("Permissions wizard step advancement - Clear previous emojis");
        }

        protected override async Task PostStep()
        {
            if (result.InvalidationReason == WizardInvalidationReason.Cancelled) await WizardCancelledResponse();
            if (result.InvalidationReason == WizardInvalidationReason.TimedOut) await ResultTimedOutResponse();
            if (result.InvalidationReason == WizardInvalidationReason.UnknownError) await UnknownErrorResponse();
        }
        #endregion

        #region Private Helpers
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

        private async Task WizardCancelledResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Permissions wizard - Cancelled", $"{context.Member.Mention} cancelled the wizard", DiscordColor.Red, "Error: Wizard cancelled").Build());
        }

        private async Task ResultTimedOutResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Permissions wizard - Timed out", "The interactivity timeout for this wizard was reached", DiscordColor.Red, "Error: Timed out").Build());
        }

        private async Task UnknownErrorResponse()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Permissions wizard - Unknown error", "An unknown error has occurred. Please try again. If this keeps occuring, reach out to the bot developer", DiscordColor.Red, "Error: Unknown").Build());
        }

        //TODO remove or make better...theres no reason not to use a default builder list. Either way there's gonna be a repition of code
        private DiscordEmbedBuilder GetBuilder(string title, string description, DiscordColor color, string footer)
        {
            return new DiscordEmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color).WithFooter(footer).WithThumbnail(context.Guild.IconUrl);
        }
        #endregion

        #region WizardSteps
        private async Task AwaitCreateRenameReactions()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Permissions wizard - Rename", "Should these members or roles be allowed to rename the channels they create?", DiscordColor.Blue, "Permission Wizard Rename Step").Build());

            await interactionMessage.CreateReactionAsync(reactionEmotes[createRenameReactions[0]]);
            await interactionMessage.CreateReactionAsync(reactionEmotes[createRenameReactions[1]]);
            await interactionMessage.CreateReactionAsync(reactionEmotes.Yes);
            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);

            List<DiscordEmoji> reactions = new();
            InteractivityResult<MessageReactionAddEventArgs> result;
            do
            {
                result = await interactionMessage.WaitForReactionAsync(context.User);
                if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

                reactions.Add(result.Result.Emoji);
            } while (!result.Result.Emoji.Equals(reactionEmotes.Yes));

            if (!reactions.Contains(reactionEmotes[createRenameReactions[0]]))
            {
                this.result.Authorizations = ChannelAuthorizations.Unauthorized;
                complete = true;
            }

            reactionEmotes.GetEmojiValue(result.Result.Emoji, out ChannelAuthorities authorities);
            this.result.Authorizations.AddAuthority(authorities);
        }

        private async Task AwaitCreationQuantityReaction()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Permissions wizard - Quantity", "How many channels should these members be allowed to create?", DiscordColor.Blue, "Permission Wizard Rename Step").Build());

            await interactionMessage.CreateReactionAsync(reactionEmotes.Cancel);
            await interactionMessage.CreateReactionAsync(reactionEmotes[createQuantityReactions[0]]);
            await interactionMessage.CreateReactionAsync(reactionEmotes[createQuantityReactions[1]]);
            await interactionMessage.CreateReactionAsync(reactionEmotes[createQuantityReactions[2]]);

            var result = await interactionMessage.WaitForReactionAsync(context.User);
            if (CheckResultTimedOut(result) || CheckResultCancelled(result)) return;

            reactionEmotes.GetEmojiValue(result.Result.Emoji, out ChannelAuthorities authority);
            this.result.Authorizations.AddAuthority(authority);

            if (authority == ChannelAuthorities.CanCreateMultipleChannels)
            await AwaitCreationQuantityMessage();
        }

        private async Task AwaitCreationQuantityMessage()
        {
            await interactionMessage.ModifyAsync(GetBuilder("Permissions wizard - Rename", "Should these members or roles be allowed to rename the channels they create?", DiscordColor.Blue, "Permission Wizard Rename Step").Build());

        }

        private async Task AwaitChannelAccessibilityReactions()
        {

        }

        private async Task AwaitChannelLimitReaction()
        {

        }

        private async Task AwaitChannelBitrateReaction()
        {

        }

        private async Task AwaitChannelRegionReaction()
        {

        }
        #endregion
    }
}