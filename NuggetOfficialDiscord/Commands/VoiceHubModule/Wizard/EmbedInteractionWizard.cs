using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public abstract class EmbedInteractionWizard<T> where T : struct, IWizardResult
	{
		protected static WizardEmoteContainer reactionEmotes;
		protected static bool emojiPopulated = false;

		protected readonly CommandContext context;

		protected readonly List<Func<Task>> wizardSteps;

		protected DiscordChannel responseChannel;
		protected DiscordMessage interactionMessage;
		protected T result;

		public EmbedInteractionWizard(CommandContext context)
		{
			reactionEmotes ??= new WizardEmoteContainer(context.Client);

			this.context = context;
			responseChannel = context.Channel;
			wizardSteps = new List<Func<Task>>();
			result = new T();
		}
		public EmbedInteractionWizard(CommandContext context, DiscordChannel responseChannel) : this(context)
		{
			if (responseChannel is null)
			{
				throw new ArgumentNullException(nameof(responseChannel), "explicit response channel cannot be null");
			}

			this.responseChannel = responseChannel;
		}

		public virtual async Task SetupWizard() => await Task.CompletedTask;
		public virtual async Task<T> GetResult() => await Task.FromResult(result);
		
		protected abstract void InitializeEmojiContainer();
		protected abstract void CreateWizardSteps();

		protected virtual Task PreStep() => Task.CompletedTask;
		protected virtual Task PostStep() => Task.CompletedTask;
	}
}