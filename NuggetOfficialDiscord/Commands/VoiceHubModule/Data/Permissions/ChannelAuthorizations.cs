using Newtonsoft.Json;
using System.Collections.Generic;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule.Data.Permissions
{
    /// <summary>
    /// Representation of authorities which allow or deny certain configurations when creating or editing channels
    /// </summary>
    [JsonObject]
    public class ChannelAuthorizations
    {
        [JsonIgnore]
        public static ChannelAuthorizations Authorized => new(ChannelAuthorities.CompletelyAuthorized);
        [JsonIgnore]
        public static ChannelAuthorizations Unauthorized => new(ChannelAuthorities.CompletelyUnauthorized);

        [JsonProperty("authorities")]
        public ChannelAuthorities Authorities { get; private set; }

        public ChannelAuthorizations()
        {
            Authorities = ChannelAuthorities.CompletelyUnauthorized;
        }
        public ChannelAuthorizations(ChannelAuthorities authorities)
        {
            Authorities = authorities;
        }
        public ChannelAuthorizations(int authoritiesInt)
        {
            Authorities = (ChannelAuthorities)authoritiesInt;
        }

        public bool VerifyAuthority(ChannelCreationParameters parameters, out string[] errors)
        {
            List<string> errorsList = new();

            if (!Authorities.HasFlag(ChannelAuthorities.CanCreateChannels))
            {
                errorsList.Add("member does not have the authority to create channels");
                goto MajorAuthorityViolationOccurred;
            }

            if (!string.IsNullOrEmpty(parameters.ChannelName) && !Authorities.HasFlag(ChannelAuthorities.CanRenameChannels))
            {
                errorsList.Add("member does not have the authority to rename channels");
            }

            if (!Authorities.HasFlag(ChannelAuthorities.CanCreateInfiniteChannels))
            {
                if (Authorities.HasFlag(ChannelAuthorities.CanCreateSingleChannel) && parameters.ExistingChannels != 0)
                {
                    errorsList.Add("member does not have the authority to create more than one channel");
                    goto ChannelQuantityCheckComplete;
                }

                if (Authorities.HasFlag(ChannelAuthorities.CanCreateMultipleChannels) && (parameters.ExistingChannels + 1) > parameters.MaximumAllowedChannels)
                {
                    errorsList.Add("member has reached their maximum channel limit and needs to delete some existing ones before they can create new ones");
                }
            }
        ChannelQuantityCheckComplete:

            if (!Authorities.HasFlag(ChannelAuthorities.CanModifyChannelRegion) && !(parameters.RequestedRegion is null))
            {
                errorsList.Add("member does not have the authority to modify the channel's voice region");
            }

            if (!Authorities.HasFlag(ChannelAuthorities.CanModifyChannelBitrate) && !(parameters.Bitrate is null))
            {
                errorsList.Add("member does not have the authority to modify the channel's bitrate");
                goto BitrateCheckComplete;
            }
        BitrateCheckComplete:

            switch (parameters.Publicity)
            {
                case ChannelAccessibility.Public:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreatePublicChannels))
                    {
                        errorsList.Add("member does not have the authority to create public channels");
                    }
                    break;
                case ChannelAccessibility.Private:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreatePrivateChannels))
                    {
                        errorsList.Add("member does not have the authority to create private channels");
                    }
                    break;
                case ChannelAccessibility.Supporter:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreateSupporterChannels))
                    {
                        errorsList.Add("member does not have the authority to create supporter channels");
                    }
                    break;
                case ChannelAccessibility.Hidden:
                    if (!Authorities.HasFlag(ChannelAuthorities.CanCreateHiddenChannels))
                    {
                        errorsList.Add("member does not have the authority to create hidden channels");
                    }
                    break;
            }

        MajorAuthorityViolationOccurred:
            return (errors = errorsList.ToArray()).Length == 0;
        }

        public override string ToString()
        {
            return $"Authorizations: {Authorities}";
        }
    }
}
