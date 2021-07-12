using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions;
using System;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
    public class PermissionsWizard : EmbedInteractionWizard<PermissionsWizardResult>
    {
        #region Private Variables
        private readonly DiscordRole[] roles;
        private readonly DiscordMember[] members;
        #endregion

        #region Constructors
        public PermissionsWizard(CommandContext context, DiscordRole[] roles, DiscordMember[] members) : base(context)
        {
            this.roles = roles;
            this.members = members;
        }
        public PermissionsWizard(CommandContext context, DiscordChannel responseChannel, DiscordRole[] roles, DiscordMember[] members) : base(context, responseChannel)
        {
            this.roles = roles;
            this.members = members;
        }
        #endregion

        #region Overrides
        public override async Task SetupWizard()
        {
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder().WithTitle("Permissions wizard").WithThumbnail(context.Member.DefaultAvatarUrl);
            interactionMessage = await context.Message.RespondAsync(builder.Build());

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
            while (result.Valid && ++stepIndex < wizardSteps.Count);

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

        }

        private async Task AwaitCreationQuantityReaction()
        {

        }

        private async Task AwaitCreationQuantityMessage()
        {

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