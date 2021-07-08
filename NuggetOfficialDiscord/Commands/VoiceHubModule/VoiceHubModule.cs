using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Data;
using NuggetOfficial.Discord.Commands.VoiceHubModule.Wizard;
using NuggetOfficial.Discord.Serialization;
using NuggetOfficialDiscord.Commands.VoiceHubModule.Wizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NuggetOfficial.Discord.Commands.VoiceHubModule
{
    public class VoiceHubModule : BaseCommandModule
    {
        readonly RegisteredGuildData registeredGuildData;

        public VoiceHubModule(RegisteredGuildData registeredGuildData)
        {
            this.registeredGuildData = registeredGuildData;
        }

        #region Authoritive Actions
        #region Overrides
        public override async Task BeforeExecutionAsync(CommandContext ctx)
        {
            if (registeredGuildData.Rebuilding)
            {
                throw new Exception("Cancel execution of command - VoiceRegisteredGuildData is rebuilding from file");
            }

            await ctx.TriggerTypingAsync();
        }
        #endregion
        //Bot should automatically serialize data as soon as a new server is registerred, and update the serialization every time a change is made to the permissions,
        //channels, or any other data that is stored on the file system
        //[Command("testserialize")]
        //public async Task TestSerialize(CommandContext ctx)
        //{
        //	SerializationResult result = Serializer.Serialize("Data/Voice/GuildData/guild_data.json", registeredGuildData);
        //	MessageType messageType = result.Success ? MessageType.Success : MessageType.Error;
        //	await ctx.RespondAsync(CreateEmbedMessage(ctx, messageType, "Serialization Output", new List<KeyValuePair<string, string>>(new[] { new KeyValuePair<string, string>("JSON result", $"{(result.Success ? "Success" : "Error: ")}{result.ErrorMessage}") })));
        //}
        #region Guild registration
        [Command("registerguild")]
        public async Task RegisterGuild(CommandContext ctx, DiscordChannel logChannel, DiscordChannel parentCategory, DiscordChannel waitingRoomVC, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole)
        {
            //store the log channel
            await RegisterGuild(ctx, parentCategory, waitingRoomVC, commandListenChannel, memberRole, mutedRole, botManagerRole);
        }
        [Command("registerguild")]
        public async Task RegisterGuild(CommandContext ctx, DiscordChannel parentCategory, DiscordChannel waitingRoomVC, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole)
        {
            //    MessageType messageType = MessageType.Success;
            //    List<KeyValuePair<string, string>> embedContent = new List<KeyValuePair<string, string>>();

            //    if (!registeredGuildData.RegisterGuild(ctx.Guild, parentCategory, waitingRoomVC, commandListenChannel, memberRole, mutedRole, botManagerRole, out string error))
            //    {
            //        await ctx.Message.RespondAsync(error);
            //        goto Completed;
            //    }

            //    registeredGuildData[ctx.Guild].InitializePermissions(VoiceChannelConfigurationPermissions.Unauthorized, new[] { new KeyValuePair<DiscordRole, VoiceChannelConfigurationPermissions>(memberRole, new VoiceChannelConfigurationPermissions(ChannelCreationAuthority.Authorized, ChannelRenameAuthority.Unauthorized, ChannelCreationQuantityAuthority.Single, ChannelAccesibilityConfigurationAuthority.Private, ChannelRegionConfigurationAuthority.Unauthorized, ChannelBitrateConfigurationAuthority.Unaurhotized)), new KeyValuePair<DiscordRole, VoiceChannelConfigurationPermissions>(mutedRole, VoiceChannelConfigurationPermissions.Unauthorized), new KeyValuePair<DiscordRole, VoiceChannelConfigurationPermissions>(botManagerRole, VoiceChannelConfigurationPermissions.Authorized) }, new[] { new KeyValuePair<DiscordMember, VoiceChannelConfigurationPermissions>(ctx.Member, VoiceChannelConfigurationPermissions.Authorized) });

            //    if (error == string.Empty)
            //    {
            //        embedContent.AddRange(new[]
            //        {
            //            new KeyValuePair<string, string>("Guild", $"{ctx.Guild.Name} - {ctx.Guild.Id}"),
            //            new KeyValuePair<string, string>("Parent Category", $"{parentCategory?.Mention ?? "null"} - {parentCategory?.Id}"),
            //            new KeyValuePair<string, string>("Waiting Room VC", $"{waitingRoomVC?.Mention ?? "null"} - {waitingRoomVC?.Id}"),
            //            new KeyValuePair<string, string>("Command Listen Channel", $"{commandListenChannel?.Mention ?? "null"} - {waitingRoomVC?.Id}"),
            //            new KeyValuePair<string, string>("Member Role", $"{memberRole.Mention} - {memberRole?.Id}"),
            //            new KeyValuePair<string, string>("Muted Role", $"{mutedRole.Mention} - {mutedRole.Id}"),
            //            new KeyValuePair<string, string>("Bot Manager Role", $"{botManagerRole.Mention} - {botManagerRole.Id}")
            //        });
            //        goto Completed;
            //    }

            //    embedContent.Add(new KeyValuePair<string, string>("Error", error));

            //Completed:
            //    await ctx.Message.RespondAsync(CreateEmbedMessage(ctx, messageType, $"Registering guild \"{ctx.Guild.Name}\" ({ctx.Guild.Id})", embedContent));
            //    return;
        }
        #endregion
        #region Member/Role Permissions
        [Command("permit")]
        public async Task Permit(CommandContext ctx, params DiscordRole[] roles)
        {
            AlterPermissionsWizardResult result = await StartPermissionWizardForMemberOrRole(ctx, roles, null);

            //print the result and ask for validation

            //if validated, save and serialize the data
            //otherwise, save nothing and return
        }
        [Command("permit")]
        public async Task Permit(CommandContext ctx, params DiscordMember[] members)
        {
            AlterPermissionsWizardResult result = await StartPermissionWizardForMemberOrRole(ctx, null, members);

            //print the result and ask for validation

            //if validated, save and serialize the data
            //otherwise, save nothing and return
        }
        private async Task<AlterPermissionsWizardResult> StartPermissionWizardForMemberOrRole(CommandContext ctx, DiscordRole[] roles, DiscordMember[] members)
        {
            AlterPermissionsWizard wizard = new AlterPermissionsWizard(ctx, roles, members);
            await wizard.SetupWizard();
            return await wizard.GetResult();
        }
        #endregion
        private bool SerializeModifiedGuildData()
        {
            return false;
        }
        #endregion
        #region User Actions
        [Command("checkperms")] //This command will outline a member's permissions neatly in an embed
        public async Task CheckMemberPermissions(CommandContext ctx)
        {
            await RespondAsync(ctx.Message, "NYI");
        }

        [Command("createvc")]
        public async Task ChannelCreationWizard(CommandContext ctx)
        {
            CreateChannelWizard wizard = new CreateChannelWizard(ctx, registeredGuildData[ctx.Guild]);
            await wizard.SetupWizard();
            CreateChannelWizardResult result = await wizard.GetResult();
            await CreateChannelAndMoveMemberAsync(ctx.Guild, ctx.Member, result.UserLimit, result.Bitrate, result.ChannelVoiceRegion, null);
        }

        //Getting replaced with wizards
        //[Command("createvc")]
        //public async Task CreateVC(CommandContext ctx, ChannelAccessibility publicity = ChannelAccessibility.Public, int? maxUsers = 0, int? bitrate = 64000, VoiceRegion region = VoiceRegion.Automatic, params DiscordMember[] permittedMembers)
        //{
        //	List<KeyValuePair<string, string>> embedResponseFields = new List<KeyValuePair<string, string>>(10);
        //	MessageType messageType = MessageType.Success;
        //	string warning = string.Empty;

        //	if (ValidateServerRegistered(ctx) && ValidateCommandChannel(ctx))
        //	{
        //		if (!ValidateVCParameterInput(ctx.Guild, publicity, maxUsers, bitrate, region, out string[] errors)) //ensure the provided parameters arent shite
        //		{
        //			messageType = MessageType.Error;
        //			goto Completed;
        //		}

        //		if (!ValidateMemberCreationPermissions(ctx.Guild, ctx.Member, null, publicity, region, out errors)) //TODO implement channel name
        //		{
        //			messageType = MessageType.Error;
        //			goto Completed;
        //		}

        //		if (ctx.Member.VoiceState is null || ctx.Member.VoiceState.Channel is null || !ctx.Member.VoiceState.Channel.Equals(registeredGuildData[ctx.Guild].WaitingRoomVC)) //ensure the member is in the waiting room
        //		{
        //			//error = $"``You cannot create a voice channel if you arent in the waiting room: {registeredGuildData[ctx.Guild].WaitingRoomVC.Name}``";
        //			messageType = MessageType.Error;
        //			goto Completed;
        //		}

        //		List<DiscordOverwriteBuilder> channelPermissionsBuilderList = GeneratePermissionOverwrites(ctx.Guild, ctx.Member, publicity, permittedMembers);
        //		DiscordChannel createdChannel = await CreateChannelAndMoveMemberAsync(ctx.Guild, ctx.Member, maxUsers, bitrate, region, channelPermissionsBuilderList);
        //		embedResponseFields.Add(new KeyValuePair<string, string>("Channel Publicity", publicity.ToString()));
        //		embedResponseFields.Add(new KeyValuePair<string, string>("Max Users", maxUsers.ToString()));
        //		embedResponseFields.Add(new KeyValuePair<string, string>("Bitrate", bitrate.ToString()));
        //		embedResponseFields.Add(new KeyValuePair<string, string>("Region", region.ToString()));
        //		embedResponseFields.Add(new KeyValuePair<string, string>("Permitted Members", $"{permittedMembers?.Length ?? 0} members permitted"));

        //		if (permittedMembers is null || permittedMembers.Length == 0)
        //		{
        //			warning = $"You have created a private voice channel but have not specified anyone permitted to join. Be sure to whitelist members you want to have access to it";
        //			messageType = MessageType.Warning;
        //			goto Completed;
        //		}

        //		if (permittedMembers.Length > 0)
        //		{
        //			await AttemptInformPermittedMembersDirectly(ctx.Member, createdChannel, permittedMembers);
        //			goto Completed;
        //		}

        //	Completed:
        //		//if (error != string.Empty) embedResponseFields.Add(new KeyValuePair<string, string>("Error", error));
        //		if (warning != string.Empty) embedResponseFields.Add(new KeyValuePair<string, string>("Warning", warning));

        //		await ctx.Message.RespondAsync(CreateEmbedMessage(ctx, messageType, "Member Create Voice Channel", embedResponseFields));
        //	}
        //}

        //TODO implement
        //TODO add overloads for every single param combo
        //[Command("updatevc")]
        //public async Task UpdateVC(CommandContext ctx, ChannelAccessibility publicity = ChannelAccessibility.Public, int? maxUsers = 0, int? bitrate = 64000, VoiceRegion region = VoiceRegion.Automatic)
        //{
        //	await RespondAsync(ctx.Message, "NYI");
        //}

        //TODO implement
        [Command("whitelist")]
        public async Task Whitelist(CommandContext ctx, params DiscordMember[] memberList)
        {
            //check if the calling member has a channel
            //check if the channel is private or hidden
            //loop over each member in the list and give them the connect, speak, and voice activation detection permissions and remove the rest if they aren't muted
            //apply the changes to the voice channel in question

            await RespondAsync(ctx.Message, "NYI");
        }
        [Command("whitelist")]
        public async Task Whitelist(CommandContext ctx, DiscordChannel channel, params DiscordMember[] memberList)
        {
            await RespondAsync(ctx.Message, "NYI");
        }

        [Command("deletevc")]
        public async Task DeleveVC(CommandContext ctx, DiscordChannel targetChannel = null)
        {
            if (ValidateServerRegistered(ctx))
            {
                if (!(registeredGuildData[ctx.Guild]?[ctx.Member] is null) || registeredGuildData[ctx.Guild][ctx.Member].Count == 0)
                {
                    await RespondAsync(ctx.Message, "``You need to create a VC to delete one``");
                    return;
                }

                try
                {
                    DiscordChannel toDelete = registeredGuildData[ctx.Guild][ctx.Member].Count == 1 ? registeredGuildData[ctx.Guild][ctx.Member][0] : ctx.Member.VoiceState?.Channel;
                    if (toDelete is null)
                    {
                        await RespondAsync(ctx.Message, $"``Because you have multiple created channels, you must be in the VC you wish to delete``");
                        return;
                    }

                    if (!registeredGuildData[ctx.Guild][ctx.Member].Contains(toDelete))
                    {
                        await RespondAsync(ctx.Message, $"``You are in a VC you do not own and, therefore, cannot delete it``");
                        return;
                    }

                    await toDelete.DeleteAsync($"{ctx.Member.DisplayName}#{ctx.Member.Discriminator}:{ctx.Member.Id} requested to delete their created channel: {toDelete.Name}:{toDelete.Id}");
                    registeredGuildData[ctx.Guild].RemoveChannel(ctx.Member, toDelete);
                    await RespondAsync(ctx.Message, $"``Deleted channel {toDelete.Name}``");
                }
                catch (Exception e)
                {
                    await RespondAsync(ctx.Message, $"``{e}``");
                }
            }
        }
        #endregion

        #region Asynchronous Private Methods
        //TODO remove, change with more dynamic embed system
        async Task RespondAsync(DiscordMessage responseEntity, string message)
        {
            await responseEntity.RespondAsync(message);
        }

        //TODO thinking this should be moved to the GuildData class
        //TODO also, needs support to be passed the channel name
        async Task<DiscordChannel> CreateChannelAndMoveMemberAsync(DiscordGuild guild, DiscordMember channelCreator, int? maxUsers, int? bitrate, VoiceRegion region/*TODO not yet supported by D#+ but will be soon (hopefully)*/, IEnumerable<DiscordOverwriteBuilder> permissions)
        {
            DiscordChannel createdChannel = null;
            try
            {
                createdChannel = await guild.CreateVoiceChannelAsync(channelCreator.Nickname ?? $"{channelCreator.DisplayName}'s VC", registeredGuildData[guild].ParentCategory, bitrate, maxUsers, permissions, VideoQualityMode.Auto, $"Channel created via command by member {channelCreator.DisplayName}#{channelCreator.Discriminator}:{channelCreator.Id}");
                registeredGuildData[guild].AddChannel(channelCreator, createdChannel); //Add the channel to the registerred guild's data container
            }
            catch (Exception)
            {
                //TODO log, respond letting the creator know the issue that occurred
                return null;
            }

            await channelCreator.ModifyAsync(m => m.VoiceChannel = createdChannel);

            return createdChannel;
        }


        //TODO needs thorough testing
        //async Task AttemptInformPermittedMembersDirectly(DiscordMember channelCreator, DiscordChannel createdVoiceChannel, IEnumerable<DiscordMember> permittedAndAuthorizedMembers)
        //{
        //	DiscordInvite newInvite = await createdVoiceChannel.CreateInviteAsync();
        //	foreach (DiscordMember member in permittedAndAuthorizedMembers)
        //	{
        //		DiscordDmChannel currentMemberDm;
        //		try { currentMemberDm = await member.CreateDmChannelAsync(); }
        //		catch (Exception) { continue; }

        //		if (currentMemberDm is null) continue;

        //		await currentMemberDm.SendMessageAsync($"You were whitelisted to join a VC by {channelCreator.Nickname ?? channelCreator.DisplayName} in {createdVoiceChannel.Guild.Name}. Click this link to immediately join the channel: {newInvite}");
        //	}
        //}
        #endregion

        #region Synchronous Private Methods
        bool ValidateServerRegistered(CommandContext ctx)
        {
            return !(registeredGuildData[ctx.Guild] is null);
        }

        //DiscordEmbed CreateEmbedMessage(CommandContext ctx, MessageType type, string title, List<KeyValuePair<string, string>> fields)
        //{
        //	DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

        //	embed.WithTitle(title);
        //	embed.WithDescription($"{ctx.Prefix}{ctx.Command.Name} invoked by {ctx.Member.DisplayName}#{ctx.Member.Discriminator}");
        //	embed.WithThumbnail(ctx.Member.AvatarUrl);

        //	if (!(fields is null))
        //		foreach (var kvp in fields)
        //			embed.AddField(kvp.Key, kvp.Value);

        //	switch (type)
        //	{
        //		case MessageType.Error:
        //			embed.WithColor(DiscordColor.Red);
        //			break;
        //		case MessageType.Warning:
        //			embed.WithColor(DiscordColor.Orange);
        //			break;
        //		case MessageType.Success:
        //			embed.WithColor(DiscordColor.Green);
        //			break;
        //		case MessageType.Notification:
        //			embed.WithColor(DiscordColor.Blue);
        //			break;
        //	}

        //	embed.WithFooter(type.ToString());

        //	return embed.Build();
        //}

        //bool ValidateCommandChannel(CommandContext ctx)
        //{
        //	return registeredGuildData[ctx.Guild].CommandListenChannel.Equals(ctx.Channel);
        //}

        //bool ValidateVCParameterInput(DiscordGuild guild, ChannelAccessibility publicity, int? maxUsers, int? bitrate, VoiceRegion region, out string[] error)
        //{
        //	//error = string.Empty;

        //	//if (registeredGuildData[guild] is null || registeredGuildData[guild].ParentCategory is null || registeredGuildData[guild].WaitingRoomVC is null || registeredGuildData[guild].CommandListenChannel is null)
        //	//{
        //	//	error = $"``Be sure to have an admin set the parent category, waiting room vc and the command channel before using this command``";
        //	//	goto Completed;
        //	//}
        //	//if (publicity == ChannelPublicity.Unknown)
        //	//{
        //	//	error = "``The publicity option you selected is not supported. Available options are: public|private|supporter|hidden``";
        //	//	goto Completed;
        //	//}
        //	//if (maxUsers < 0 || maxUsers > 99)
        //	//{
        //	//	error = "``You cannot create a channel with a limit less than 0 or larger than 99 people``";
        //	//	goto Completed;
        //	//}
        //	//if (bitrate < 8000 || bitrate > 96000)
        //	//{
        //	//	error = "``You cannot create a channel with a bitrate less than 8kbps or larger than 96kbps``";
        //	//	goto Completed;
        //	//}
        //	//if (region == VoiceRegion.Unknown)
        //	//{
        //	//	error = "``The region option you selected is not supported. Available options are: auto|automatic|brazil|europe|hongkong|india|japan|russia|singapore|southafrica|sydney|uscentral|useast|ussouth|uswest``";
        //	//	goto Completed;
        //	//}

        //	//Completed:
        //	//return error == string.Empty;
        //	error = null;
        //	return false;
        //}

        //TODO need a better way to store/specify permissions, and preferably one where no permissions is left out (because of my OCD)

        //List<DiscordOverwriteBuilder> GeneratePermissionOverwrites(DiscordGuild guild, DiscordMember creator, ChannelAccessibility publicity, params DiscordMember[] permittedMembers)
        //{
        //	//TODO implement the ranking method that allows moderators to specify what roles have access to naming capabilities, publicity options, and bitrate options
        //	List<DiscordOverwriteBuilder> channelPermissionsBuilderList = new List<DiscordOverwriteBuilder>
        //	{
        //		new DiscordOverwriteBuilder(guild.EveryoneRole).Deny(Permissions.AccessChannels | Permissions.UseVoice), //Prevent everyone from viewing any channel
        //		new DiscordOverwriteBuilder(registeredGuildData[guild].MemberRole).Allow(publicity == ChannelAccessibility.Hidden ? Permissions.None : Permissions.AccessChannels).Deny(publicity == ChannelAccessibility.Private ? Permissions.UseVoice : Permissions.None).Deny(publicity == ChannelAccessibility.Hidden ? Permissions.AccessChannels : Permissions.None), //Allow members to see private channels
        //		new DiscordOverwriteBuilder(registeredGuildData[guild].MutedRole).Allow(publicity == ChannelAccessibility.Hidden ? Permissions.None : Permissions.AccessChannels).Deny(Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection | Permissions.Stream) //Dissallow muted members from accessing or viewing
        //	};

        //	//TODO this might be redundant...need to test if the muted role overwrite will disallow the muted role from joining even if this allows them...though i think i know the behavior that the role system will produce
        //	//TODO may need to accomodate other blacklist roles
        //	foreach (var member in permittedMembers)
        //	{
        //		if (member.Roles.Contains(registeredGuildData[guild].MutedRole)) continue; //TODO we want to inform the creator this member was not whitelisted in the vc because they have the muted role
        //		channelPermissionsBuilderList.Add(new DiscordOverwriteBuilder().For(member).Allow(Permissions.AccessChannels | Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection | Permissions.Stream));
        //	}

        //	return channelPermissionsBuilderList;
        //}

        //bool ValidateMemberCreationPermissions(DiscordGuild guild, DiscordMember member, string channelName, ChannelAccessibility requestedPublicity, VoiceRegion requestedRegion, out string[] errors)
        //{
        //	return registeredGuildData[guild].CheckPermission(member, new ChannelCreationParameters(), out errors);
        //}
        #endregion
    }
}
