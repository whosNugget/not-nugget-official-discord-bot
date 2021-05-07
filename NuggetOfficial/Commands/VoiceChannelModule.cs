using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NuggetOfficial.Data;
using NuggetOfficial.Authority;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuggetOfficial.Data.VoiceModule;

namespace NuggetOfficial.Commands
{
	public class VoiceChannelModule : BaseCommandModule
	{
		//TODO this should reference a database (or databases) for efficient and lightweight data storage for all this information. This is IMPERATIVE if this bot will
		//ever be sharded and publically released (which is a possibility, though I think it will always be a mini passion project that will only ever be used in my server
		//and maybe some friend's servers if they ask. If this bot is going to be in multiple servers, however, it is also required to use an external method of storing data,
		//as the bot currently stores information for a single registerred guild, and it has no capability of holding information on multiple servers at the moment.
		//When the time to convert this bot to a database instance, this class will need a massive refactor. It will be a fun development challenge, though :)

		//TODO full conversion from responding with messages to responding with embeds required

		enum MessageType
		{
			Error,
			Warning,
			Success,
			Notification,
		}

		readonly VoiceRegisteredGuildData registeredGuildData;

		public VoiceChannelModule(VoiceRegisteredGuildData registeredGuildData)
		{
			this.registeredGuildData = registeredGuildData;
		}

		public override async Task BeforeExecutionAsync(CommandContext ctx) => await ctx.TriggerTypingAsync();

		[Command("registerguild")]
		public async Task RegisterGuild(CommandContext ctx, DiscordChannel parentCategory, DiscordChannel waitingRoomVC, DiscordChannel commandListenChannel, DiscordRole memberRole, DiscordRole mutedRole, DiscordRole botManagerRole)
		{
			MessageType messageType = MessageType.Success;
			List<KeyValuePair<string, string>> embedContent = new List<KeyValuePair<string, string>>();

			if (!registeredGuildData.RegisterGuild(ctx.Guild, parentCategory, waitingRoomVC, commandListenChannel, memberRole, mutedRole, botManagerRole, out string error))
			{
				await ctx.Message.RespondAsync(error);
				goto Completed;
			}

			registeredGuildData[ctx.Guild].InitializePermissions(VoiceChannelCreationPermissions.Unauthorized, new[] { new KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>(memberRole, new VoiceChannelCreationPermissions(ChannelCreationAuthority.Authorized, ChannelRenameAuthority.Unauthorized, ChannelCreationQuantityAuthority.Single, ChannelAccesibilityConfigurationAuthority.Private, ChannelRegionConfigurationAuthority.Unauthorized)), new KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>(mutedRole, VoiceChannelCreationPermissions.Unauthorized), new KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>(botManagerRole, VoiceChannelCreationPermissions.Authorized) }, new[] { new KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>(ctx.Member, VoiceChannelCreationPermissions.Authorized) });

			if (error == string.Empty)
			{
				embedContent.AddRange(new[]
				{
					new KeyValuePair<string, string>("Guild", $"{ctx.Guild.Name} - {ctx.Guild.Id}"),
					new KeyValuePair<string, string>("Parent Category", $"{parentCategory?.Mention ?? "null"} - {parentCategory?.Id}"),
					new KeyValuePair<string, string>("Waiting Room VC", $"{waitingRoomVC?.Mention ?? "null"} - {waitingRoomVC?.Id}"),
					new KeyValuePair<string, string>("Command Listen Channel", $"{commandListenChannel?.Mention ?? "null"} - {waitingRoomVC?.Id}"),
					new KeyValuePair<string, string>("Member Role", $"{memberRole.Mention} - {memberRole?.Id}"),
					new KeyValuePair<string, string>("Muted Role", $"{mutedRole.Mention} - {mutedRole.Id}"),
					new KeyValuePair<string, string>("Bot Manager Role", $"{botManagerRole.Mention} - {botManagerRole.Id}")
				});
				goto Completed;
			}

			embedContent.Add(new KeyValuePair<string, string>("Error", error));

		Completed:
			await ctx.Message.RespondAsync(CreateEmbedMessage(ctx, messageType, $"Registering guild \"{ctx.Guild.Name}\" ({ctx.Guild.Id})", embedContent));
			return;
		}

		//TODO this command should only be allowed to be called by people with Admin powers or bot-master roles
		[Command("permit")]
		public async Task Permit(CommandContext ctx, DiscordRole role) //TODO more complex/more options
		{
			//TODO allow server owners to specify moderator roles/members that can utilize these commands
			if (ValidateServerRegistered(ctx))
			{
				if (ctx.Member.PermissionsIn(ctx.Channel).HasFlag(Permissions.ManageChannels) || ctx.Member.Roles.Contains(registeredGuildData[ctx.Guild].BotManagerRole))
				{
					await Task.Run(() => registeredGuildData[ctx.Guild].UpdatePermissions(null, new[] { new KeyValuePair<DiscordRole, VoiceChannelCreationPermissions>(role, new VoiceChannelCreationPermissions(ChannelCreationAuthority.Authorized, ChannelRenameAuthority.Authorized, ChannelCreationQuantityAuthority.Single, ChannelAccesibilityConfigurationAuthority.Private, ChannelRegionConfigurationAuthority.Authorized)) }, null));
				}
			}
		}
		[Command("permit")]
		public async Task Permit(CommandContext ctx, DiscordMember member) //TODO more complex/more options
		{
			//TODO allow server owners to specify moderator roles/members that can utilize these commands
			if (ValidateServerRegistered(ctx))
			{
				if (ctx.Member.PermissionsIn(ctx.Channel).HasFlag(Permissions.ManageChannels) || ctx.Member.Roles.Contains(registeredGuildData[ctx.Guild].BotManagerRole))
				{
					await Task.Run(() => registeredGuildData[ctx.Guild].UpdatePermissions(null, null, new[] { new KeyValuePair<DiscordMember, VoiceChannelCreationPermissions>(member, new VoiceChannelCreationPermissions(ChannelCreationAuthority.Authorized, ChannelRenameAuthority.Authorized, ChannelCreationQuantityAuthority.Single, ChannelAccesibilityConfigurationAuthority.Private, ChannelRegionConfigurationAuthority.Authorized)) }));
				}
			}
		}

		[Command("createvcwizard")]
		public async Task ChannelCreationWizard(CommandContext ctx)
		{
			await ctx.RespondAsync("I'm not a miracle worker this shit gonna take a while to implement");
		}

		[Command("checkperms")] //This command will outline a member's permissions neatly in an embed
		public async Task CheckMemberPermissions(CommandContext ctx)
		{
			await RespondAsync(ctx.Message, "NYI");
		}

		//TODO create version for rename
		[Command("createvc")]
		public async Task CreateVC(CommandContext ctx, ChannelPublicity publicity = ChannelPublicity.Public, int? maxUsers = 0, int? bitrate = 64000, VoiceRegion region = VoiceRegion.Automatic, params DiscordMember[] permittedMembers)
		{
			List<KeyValuePair<string, string>> embedResponseFields = new List<KeyValuePair<string, string>>(10);
			MessageType messageType = MessageType.Success;
			string warning = string.Empty;

			if (ValidateServerRegistered(ctx) && ValidateCommandChannel(ctx))
			{
				if (!ValidateVCParameterInput(ctx.Guild, publicity, maxUsers, bitrate, region, out string error)) //ensure the provided parameters arent shite
				{
					messageType = MessageType.Error;
					goto Completed;
				}

				if (!ValidateMemberCreationPermissions(ctx.Guild, ctx.Member, null, publicity, region, out error)) //TODO implement channel name
				{
					messageType = MessageType.Error;
					goto Completed;
				}

				if (ctx.Member.VoiceState is null || ctx.Member.VoiceState.Channel is null || !ctx.Member.VoiceState.Channel.Equals(registeredGuildData[ctx.Guild].WaitingRoomVC)) //ensure the member is in the waiting room
				{
					error = $"``You cannot create a voice channel if you arent in the waiting room: {registeredGuildData[ctx.Guild].WaitingRoomVC.Name}``";
					messageType = MessageType.Error;
					goto Completed;
				}

				List<DiscordOverwriteBuilder> channelPermissionsBuilderList = GeneratePermissionOverwrites(ctx.Guild, ctx.Member, publicity, permittedMembers);
				DiscordChannel createdChannel = await CreateChannelAndMoveMemberAsync(ctx.Guild, ctx.Member, maxUsers, bitrate, region, channelPermissionsBuilderList);
				embedResponseFields.Add(new KeyValuePair<string, string>("Channel Publicity", publicity.ToString()));
				embedResponseFields.Add(new KeyValuePair<string, string>("Max Users", maxUsers.ToString()));
				embedResponseFields.Add(new KeyValuePair<string, string>("Bitrate", bitrate.ToString()));
				embedResponseFields.Add(new KeyValuePair<string, string>("Region", region.ToString()));
				embedResponseFields.Add(new KeyValuePair<string, string>("Permitted Members", $"{permittedMembers?.Length ?? 0} members permitted"));

				if (permittedMembers is null || permittedMembers.Length == 0)
				{
					warning = $"You have created a private voice channel but have not specified anyone permitted to join. Be sure to whitelist members you want to have access to it";
					messageType = MessageType.Warning;
					goto Completed;
				}

				if (permittedMembers.Length > 0)
				{
					await AttemptInformPermittedMembersDirectly(ctx.Member, createdChannel, permittedMembers);
					goto Completed;
				}

			Completed:
				if (error != string.Empty) embedResponseFields.Add(new KeyValuePair<string, string>("Error", error));
				if (warning != string.Empty) embedResponseFields.Add(new KeyValuePair<string, string>("Warning", warning));

				await ctx.Message.RespondAsync(CreateEmbedMessage(ctx, messageType, "Member Create Voice Channel", embedResponseFields));
			}
		}

		//TODO implement
		//TODO add overloads for every single param combo
		[Command("updatevc")]
		public async Task UpdateVC(CommandContext ctx, ChannelPublicity publicity = ChannelPublicity.Public, int? maxUsers = 0, int? bitrate = 64000, VoiceRegion region = VoiceRegion.Automatic)
		{
			await RespondAsync(ctx.Message, "NYI");
		}

		//TODO implement
		[Command("whitelist")]
		public async Task Whitelist(CommandContext ctx, params DiscordMember[] memberList)
		{
			await RespondAsync(ctx.Message, "NYI");
		}

		//TODO extrapolate this so the owner can specify an ID or Name (probably not) to delete instead of be in the vc (for convinence)
		[Command("deletevc")]
		public async Task DeleveVC(CommandContext ctx)
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

		#region Asynchronous Private Methods
		//TODO remove, change with more dynamic embed system
		async Task RespondAsync(DiscordMessage responseEntity, string message)
		{
			await responseEntity.RespondAsync(message);
		}

		//TODO thinking this should be moved to the GuildData class
		async Task<DiscordChannel> CreateChannelAndMoveMemberAsync(DiscordGuild guild, DiscordMember channelCreator, int? maxUsers, int? bitrate, VoiceRegion region/*TODO not yet supported by D#+ but will be soon (hopefully)*/, IEnumerable<DiscordOverwriteBuilder> permissions)
		{
			DiscordChannel createdChannel = null;
			try
			{
				createdChannel = await guild.CreateVoiceChannelAsync(channelCreator.Nickname ?? $"{channelCreator.DisplayName}'s VC", registeredGuildData[guild].ParentCategory, bitrate, maxUsers, permissions, $"Channel created via command by member {channelCreator.DisplayName}#{channelCreator.Discriminator}:{channelCreator.Id}");
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
		async Task AttemptInformPermittedMembersDirectly(DiscordMember channelCreator, DiscordChannel createdVoiceChannel, IEnumerable<DiscordMember> permittedAndAuthorizedMembers)
		{
			DiscordInvite newInvite = await createdVoiceChannel.CreateInviteAsync();
			foreach (DiscordMember member in permittedAndAuthorizedMembers)
			{
				DiscordDmChannel currentMemberDm;
				try { currentMemberDm = await member.CreateDmChannelAsync(); }
				catch (Exception) { continue; }

				if (currentMemberDm is null) continue;

				await currentMemberDm.SendMessageAsync($"You were whitelisted to join a VC by {channelCreator.Nickname ?? channelCreator.DisplayName} in {createdVoiceChannel.Guild.Name}. Click this link to immediately join the channel: {newInvite}");
			}
		}
		#endregion

		#region Synchronous Private Methods
		DiscordEmbed CreateEmbedMessage(CommandContext ctx, MessageType type, string title, List<KeyValuePair<string, string>> fields)
		{
			DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

			embed.WithTitle(title);
			embed.WithDescription($"{ctx.Prefix}{ctx.Command.Name} invoked by {ctx.Member.DisplayName}#{ctx.Member.Discriminator}");
			embed.WithThumbnail(ctx.Member.AvatarUrl);

			if (!(fields is null))
				foreach (var kvp in fields)
					embed.AddField(kvp.Key, kvp.Value);

			switch (type)
			{
				case MessageType.Error:
					embed.WithColor(DiscordColor.Red);
					break;
				case MessageType.Warning:
					embed.WithColor(DiscordColor.Orange);
					break;
				case MessageType.Success:
					embed.WithColor(DiscordColor.Green);
					break;
				case MessageType.Notification:
					embed.WithColor(DiscordColor.Blue);
					break;
			}

			embed.WithFooter(type.ToString());

			return embed.Build();
		}

		bool ValidateServerRegistered(CommandContext ctx)
		{
			return !(registeredGuildData[ctx.Guild] is null);
		}

		bool ValidateCommandChannel(CommandContext ctx)
		{
			return registeredGuildData[ctx.Guild].CommandListenChannel.Equals(ctx.Channel);
		}

		bool ValidateVCParameterInput(DiscordGuild guild, ChannelPublicity publicity, int? maxUsers, int? bitrate, VoiceRegion region, out string error)
		{
			error = string.Empty;

			if (registeredGuildData[guild] is null || registeredGuildData[guild].ParentCategory is null || registeredGuildData[guild].WaitingRoomVC is null || registeredGuildData[guild].CommandListenChannel is null)
			{
				error = $"``Be sure to have an admin set the parent category, waiting room vc and the command channel before using this command``";
				goto Completed;
			}
			if (publicity == ChannelPublicity.Unknown)
			{
				error = "``The publicity option you selected is not supported. Available options are: public|private|supporter|hidden``";
				goto Completed;
			}
			if (maxUsers < 0 || maxUsers > 99)
			{
				error = "``You cannot create a channel with a limit less than 0 or larger than 99 people``";
				goto Completed;
			}
			if (bitrate < 8000 || bitrate > 96000)
			{
				error = "``You cannot create a channel with a bitrate less than 8kbps or larger than 96kbps``";
				goto Completed;
			}
			if (region == VoiceRegion.Unknown)
			{
				error = "``The region option you selected is not supported. Available options are: auto|automatic|brazil|europe|hongkong|india|japan|russia|singapore|southafrica|sydney|uscentral|useast|ussouth|uswest``";
				goto Completed;
			}

		Completed:
			return error == string.Empty;
		}

		//TODO need a better way to store/specify permissions, and preferably one where no permissions is left out (because of my OCD)
		List<DiscordOverwriteBuilder> GeneratePermissionOverwrites(DiscordGuild guild, DiscordMember creator, ChannelPublicity publicity, params DiscordMember[] permittedMembers)
		{
			//TODO implement the ranking method that allows moderators to specify what roles have access to naming capabilities, publicity options, and bitrate options
			List<DiscordOverwriteBuilder> channelPermissionsBuilderList = new List<DiscordOverwriteBuilder>
			{
				new DiscordOverwriteBuilder().For(guild.EveryoneRole).Deny(Permissions.AccessChannels | Permissions.UseVoice), //Prevent everyone from viewing any channel
				new DiscordOverwriteBuilder().For(registeredGuildData[guild].MemberRole).Allow(publicity == ChannelPublicity.Hidden ? Permissions.None : Permissions.AccessChannels).Deny(publicity == ChannelPublicity.Private ? Permissions.UseVoice : Permissions.None).Deny(publicity == ChannelPublicity.Hidden ? Permissions.AccessChannels : Permissions.None), //Allow members to see private channels
				new DiscordOverwriteBuilder().For(registeredGuildData[guild].MutedRole).Allow(publicity == ChannelPublicity.Hidden ? Permissions.None : Permissions.AccessChannels).Deny(Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection | Permissions.Stream) //Dissallow muted members from accessing or viewing
			};

			//TODO this might be redundant...need to test if the muted role overwrite will disallow the muted role from joining even if this allows them...though i think i know the behavior that the role system will produce
			//TODO may need to accomodate other blacklist roles
			foreach (var member in permittedMembers)
			{
				if (member.Roles.Contains(registeredGuildData[guild].MutedRole)) continue; //TODO we want to inform the creator this member was not whitelisted in the vc because they have the muted role
				channelPermissionsBuilderList.Add(new DiscordOverwriteBuilder().For(member).Allow(Permissions.AccessChannels | Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection | Permissions.Stream));
			}

			return channelPermissionsBuilderList;
		}

		bool ValidateMemberCreationPermissions(DiscordGuild guild, DiscordMember member, string channelName, ChannelPublicity requestedPublicity, VoiceRegion requestedRegion, out string error)
		{
			return registeredGuildData[guild].CheckPermission(member, channelName, requestedPublicity, requestedRegion, out error);
		}
		#endregion
	}
}
