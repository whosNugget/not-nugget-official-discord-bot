namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public enum WizardInvalidationReason : byte
	{
		NotInvalid = 0,
		TimedOut = 1,
		Cancelled = 2,
		InvalidInput = 4,
		UnknownError = 8
	}
}
