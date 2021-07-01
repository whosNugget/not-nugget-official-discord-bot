using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public abstract class EmbedInteractionWizard<T> where T : struct
	{
		protected readonly CommandContext context;

		protected readonly List<Func<Task>> wizardSteps;

		protected DiscordMessage interactionMessage;
		protected T result;

		public EmbedInteractionWizard(CommandContext context)
		{
			this.context = context;
			wizardSteps = new List<Func<Task>>();
			result = new T();
		}

		public virtual Task SetupWizard() => Task.CompletedTask;
		public virtual async Task<T> GetResult() => await Task.FromResult(result);

		protected virtual Task PreStep() => Task.CompletedTask;
		protected virtual Task PostStep() => Task.CompletedTask;
	}
}