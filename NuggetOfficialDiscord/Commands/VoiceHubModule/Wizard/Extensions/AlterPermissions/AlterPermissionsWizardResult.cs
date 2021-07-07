using System;
using System.Collections.Generic;
using System.Text;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public struct AlterPermissionsWizardResult : IWizardResult
	{
		public bool Valid { get; set; }
		public WizardInvalidationReason InvalidationReason { get; set; }
		public string ErrorString { get; set; }
	}
}
