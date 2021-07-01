namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Data
{
	public class VoiceChannelModuleSettings
    {
        /// <summary>
        /// Should members who are attempting to create VCs be required to execute commands within a specified channel? All command invocations will be met with an error or
        /// will be ignored (configurable)
        /// </summary>
        public bool RequireOccupanceOfWaitingRoomVCWhenMemberChannelCreationAttemptOccurs { get; set; }

        /// <summary>
        /// Should commands recieved by members who are not in a specified waiting room be ignored?
        /// </summary>
        public bool IgnoreCommandInvocationWhenVCOccupanceConditionNotMet { get; set; }

        /// <summary>
        /// When wizard commands are used, should the voice module system create a DM channel with the caller and perform all wizard steps there? This can help reduce clutter,
        /// but also takes the focus away from the server
        /// </summary>
        public bool CreateAndUseDMChannelsForWizards { get; set; }
    }
}
