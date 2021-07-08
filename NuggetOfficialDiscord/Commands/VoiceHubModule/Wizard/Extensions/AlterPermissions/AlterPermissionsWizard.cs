using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard.Extensions.AlterPermissions
{
	public class AlterPermissionsWizard : EmbedInteractionWizard<AlterPermissionsWizardResult>
	{
		private readonly DiscordRole[] roles;
		private readonly DiscordMember[] members;

		public AlterPermissionsWizard(CommandContext context, DiscordRole[] roles, DiscordMember[] members) : base(context)
		{
			this.roles = roles;
			this.members = members;
		}
		public AlterPermissionsWizard(CommandContext context, DiscordChannel responseChannel, DiscordRole[] roles, DiscordMember[] members) : base(context, responseChannel)
		{
			this.roles = roles;
			this.members = members;
		}

		protected override void InitializeEmojiContainer()
		{
			throw new NotImplementedException();
		}

		protected override IEnumerable<Func<Task>> CreateWizardSteps()
		{
			throw new NotImplementedException();
		}
	}
}
