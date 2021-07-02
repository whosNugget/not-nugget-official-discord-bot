using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;

namespace NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard
{
	public struct WizardResult
	{
		public bool Valid { get; set; }
		public WizardInvalidationReason InvalidationReason { get; set; }
        public string ErrorString { get; set; }

		public string ChannelName { get; set; }
		public int UserLimit { get; set; }
		public int Bitrate { get; set; }
		public ChannelPublicity ChannelAccessability { get; set; }
		public VoiceRegion ChannelVoiceRegion { get; set; }
    }
}