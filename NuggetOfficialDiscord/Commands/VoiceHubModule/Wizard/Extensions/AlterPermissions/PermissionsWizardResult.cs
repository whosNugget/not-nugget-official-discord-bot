using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public struct PermissionsWizardResult : IWizardResult
	{
		public bool Valid { get; set; }
		public WizardInvalidationReason InvalidationReason { get; set; }
		public string ErrorString { get; set; }

        public ChannelAuthorizations Authorizations { get; set; }
    }
}
