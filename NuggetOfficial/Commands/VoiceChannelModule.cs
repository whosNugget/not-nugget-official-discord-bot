using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NuggetOfficial.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuggetOfficial.Commands
{
	public class VoiceChannelModule : BaseCommandModule
	{
		//TODO this should reference a database (or databases) for efficient and lightweight data storage for all this information. This is IMPERATIVE if this bot will
		//ever be sharded and publically released (which is a possibility, though I think it will always be a mini passion project that will only ever be used in my server
		//and maybe some friend's servers if they ask. If this bot is going to be in multiple servers, however, it is also required to use an external method of storing data,
		//as the bot currently stores information for a single registerred guild, and it has no capability of holding information on multiple servers at the moment.
		//When the time to convert this bot to a database instance, this class will need a massive refactor. It will be a fun development challenge, though :)

		readonly DiscordGuild registeredGuild = null;
		readonly DiscordRole memberRole = null;
		readonly DiscordRole mutedRole = null;

		DiscordChannel parentCategory = null;
		DiscordChannel waitingRoomVC = null;

		Dictionary<DiscordMember, DiscordChannel> createdChannels = new Dictionary<DiscordMember, DiscordChannel>();

		public VoiceChannelModule(DiscordGuild registeredGuild)
		{
			if (registeredGuild.Id != 0)
			{
				this.registeredGuild = registeredGuild;

				if (ConfigurationManager.AppSettings.AllKeys.Contains("parentCategoryID")) parentCategory = this.registeredGuild.GetChannel(ulong.Parse(ConfigurationManager.AppSettings.Get("parentCategoryID")));
				if (ConfigurationManager.AppSettings.AllKeys.Contains("waitingRoomVCID")) waitingRoomVC = this.registeredGuild.GetChannel(ulong.Parse(ConfigurationManager.AppSettings.Get("waitingRoomVCID")));
				if (ConfigurationManager.AppSettings.AllKeys.Contains("memberRole")) memberRole = this.registeredGuild.GetRole(ulong.Parse(ConfigurationManager.AppSettings.Get("memberRole")));
				if (ConfigurationManager.AppSettings.AllKeys.Contains("mutedRole")) mutedRole = this.registeredGuild.GetRole(ulong.Parse(ConfigurationManager.AppSettings.Get("mutedRole")));

				registeredGuild.GetChannel(832686904066834443).SendMessageAsync($"Registered this server's parent category to {parentCategory.Name}, waiting room to {waitingRoomVC.Name}, member role to {memberRole.Mention}, and muted role to {mutedRole.Mention}");
			}
		}

		[Command("registerguild")]
		public async Task RegisterGuild(CommandContext ctx, DiscordChannel parentCategory, DiscordChannel waitingRoomVC, DiscordRole memberRole, DiscordRole mutedRole)
		{
			if (this.parentCategory is null || this.waitingRoomVC is null)
			{
				this.parentCategory = parentCategory;
				this.waitingRoomVC = waitingRoomVC;

				ConfigurationManager.AppSettings.Set("parentCategoryID", parentCategory.Id.ToString());
				ConfigurationManager.AppSettings.Set("waitingRoomVCID", waitingRoomVC.Id.ToString());
				ConfigurationManager.AppSettings.Set("memberRole", memberRole.Id.ToString());
				ConfigurationManager.AppSettings.Set("mutedRole", mutedRole.Id.ToString());

				await RespondAsync(ctx.Message, $"``Registered this server's parent category to {parentCategory.Name}, waiting room to {waitingRoomVC}, member role to {memberRole.Mention}, and muted role to {mutedRole.Mention}``");
			}
			else
			{
				await RespondAsync(ctx.Message, "``This server is already registered``");
			}
		}

		//TODO write every other method parameter combination. If not all of them, at least the all default one (probably wont do)
		[Command("createvc")]
		public async Task CreateVC(CommandContext ctx, ChannelPublicity publicity = ChannelPublicity.Public, int? maxUsers = 0, int? bitrate = 64000, VoiceRegion region = VoiceRegion.Automatic, params DiscordMember[] permittedMembers)
		{
			if (!(ctx.Member.VoiceState is null) && !(ctx.Member.VoiceState.Channel is null) && ctx.Member.VoiceState.Channel.Equals(waitingRoomVC))
			{
				//in waiting room vc
				if (ValidateVCParameterInput(out string error, publicity, maxUsers, bitrate, region, permittedMembers))
				{
					//input valid
					if (!createdChannels.ContainsKey(ctx.Member) || (createdChannels.ContainsKey(ctx.Member) && createdChannels[ctx.Member] is null))
					{
						//member does not own a VC
						List<DiscordOverwriteBuilder> channelPermissionsBuilderList = GeneratePermissionOverwrites(ctx.Member, publicity, permittedMembers);
						await CreateChannelAndMoveMemberAsync(ctx.Member, maxUsers, bitrate, region, channelPermissionsBuilderList);

						if ((publicity == ChannelPublicity.Private || publicity == ChannelPublicity.Hidden) && permittedMembers?.Length == 0)
						{
							//member created a private VC with no allowed members
							await RespondAsync(ctx.Message, "``You've created a private channel but you haven't allowed members to join. Be sure to update the vc with a list of allowed members, else only you will be allowed to join``");
						}
					}
					else
					{
						//member already owns a VC
						await RespondAsync(ctx.Message, "``You need to delete your previously created channel before creating a new one``");
					}
				}
				else
				{
					//input invalid
					await RespondAsync(ctx.Message, error);
				}
			}
			else
			{
				//creator not in the waiting room
				await RespondAsync(ctx.Message, $"``You cannot create a voice channel if you arent in the waiting room: {waitingRoomVC.Name}``");
			}

			//dev message
			await RespondAsync(ctx.Message, $"``publicity: {publicity}, maxUsers: {maxUsers}, bitrate: {bitrate}, region: {region}``");
		}

		[Command("updatevc")]
		public async Task UpdateVC(CommandContext ctx, ChannelPublicity publicity = ChannelPublicity.Public, int? maxUsers = 0, int? bitrate = 64000, VoiceRegion region = VoiceRegion.Automatic)
		{

		}
		[Command("updatevc")]
		public async Task UpdateVC(CommandContext ctx, bool permit, params DiscordMember[] memberList)
		{ 

		}

		[Command("deletevc")]
		public async Task DeleveVC(CommandContext ctx)
		{
			if (createdChannels.ContainsKey(ctx.Member) && !(createdChannels[ctx.Member] is null))
			{
				//member has a channel
				try
				{
					DiscordChannel toDelete = createdChannels[ctx.Member];
					await toDelete.DeleteAsync($"{ctx.Member.DisplayName}#{ctx.Member.Discriminator}:{ctx.Member.Id} requested to delete their created channel: {toDelete.Name}:{toDelete.Id}");
					createdChannels[ctx.Member] = null;
					await RespondAsync(ctx.Message, $"``Deleted channel {toDelete.Name}``");
				}
				catch(Exception e)
				{
					await RespondAsync(ctx.Message, $"``{e}``");
				}
			}
			else
			{
				//member doesnt have a channel
				await RespondAsync(ctx.Message, "``You need to create a VC to delete one``");
			}
		}

		#region Asynchronous Private Methods
		async Task RespondAsync(DiscordMessage responseEntity, string message)
		{
			await responseEntity.RespondAsync(message);
		}

		async Task<DiscordChannel> CreateChannelAndMoveMemberAsync(DiscordMember channelCreator, int? maxUsers, int? bitrate, VoiceRegion region/*TODO not yet supported by D#+ but will be soon (hopefully)*/, IEnumerable<DiscordOverwriteBuilder> permissions)
		{
			DiscordChannel createdChannel = null;
			try
			{
				createdChannel = await registeredGuild.CreateVoiceChannelAsync(channelCreator.Nickname ?? $"{channelCreator.DisplayName}'s VC", parentCategory, bitrate, maxUsers, permissions, $"Channel created via command by member {channelCreator.DisplayName}#{channelCreator.Discriminator}:{channelCreator.Id}");

				if (createdChannels.ContainsKey(channelCreator)) createdChannels[channelCreator] = createdChannel;
				else createdChannels.Add(channelCreator, createdChannel);
			}
			catch (Exception)
			{
				//TODO log, respond letting the creator know the issue that occurred
				return null;
			}

			await channelCreator.ModifyAsync(m => m.VoiceChannel = createdChannel);

			return createdChannel;
		}
		#endregion
		#region Synchronous Private Methods
		bool ValidateVCParameterInput(out string error, ChannelPublicity publicity, int? maxUsers, int? bitrate, VoiceRegion region, params DiscordMember[] permittedMembers)
		{
			error = string.Empty;

			//TODO need to research how to limit this if clutter, if possible
			if (parentCategory is null || waitingRoomVC is null)
			{
				error = $"``Be sure to have an admin set the parent category and the waiting room vc before using this command``";
			}
			if (publicity == ChannelPublicity.Unknown)
			{
				error = "``The publicity option you selected is not supported. Available options are: public|private|supporter|hidden``";
			}
			if (maxUsers < 0 || maxUsers > 99)
			{
				error = "``You cannot create a channel with a limit less than 0 or larger than 99 people``";
			}
			if (bitrate < 8000 || bitrate > 96000)
			{
				error = "``You cannot create a channel with a bitrate less than 8kbps or larger than 96kbps``";
			}
			if (region == VoiceRegion.Unknown)
			{
				error = "``The region option you selected is not supported. Available options are: auto|automatic|brazil|europe|hongkong|india|japan|russia|singapore|southafrica|sydney|uscentral|useast|ussouth|uswest``";
			}

			return error.Equals(string.Empty);
		}

		List<DiscordOverwriteBuilder> GeneratePermissionOverwrites(DiscordMember channelCreator, ChannelPublicity publicity, params DiscordMember[] permittedMembers)
		{
			//TODO implement the ranking method that allows moderators to specify what roles have access to naming capabilities, publicity options, and bitrate options
			List<DiscordOverwriteBuilder> channelPermissionsBuilderList = new List<DiscordOverwriteBuilder>
			{
				new DiscordOverwriteBuilder().For(registeredGuild.EveryoneRole).Deny(Permissions.AccessChannels | Permissions.UseVoice), //Prevent everyone from viewing any channel
				new DiscordOverwriteBuilder().For(memberRole).Allow(publicity == ChannelPublicity.Hidden ? Permissions.None : Permissions.AccessChannels).Deny(publicity == ChannelPublicity.Private ? Permissions.UseVoice : Permissions.None).Deny(publicity == ChannelPublicity.Hidden ? Permissions.AccessChannels : Permissions.None), //Allow members to see private channels
				new DiscordOverwriteBuilder().For(mutedRole).Allow(publicity == ChannelPublicity.Hidden ? Permissions.None : Permissions.AccessChannels).Deny(Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection | Permissions.Stream) //Dissallow muted members from accessing or viewing
			};

			foreach (var member in permittedMembers)
			{
				//TODO this might be redundant...need to test if the muted role overwrite will disallow the muted role from joining even if this allows them...though i think i know the behavior that the role system will produce
				//TODO may need to accomodate other blacklist roles
				if (member.Roles.Contains(mutedRole)) continue; //TODO we want to inform the creator this memvber was not whitelisted in the vc because they have the muted role
				channelPermissionsBuilderList.Add(new DiscordOverwriteBuilder().For(member).Allow(Permissions.AccessChannels | Permissions.UseVoice | Permissions.Speak | Permissions.UseVoiceDetection | Permissions.Stream));
			}

			return channelPermissionsBuilderList;
		}
		#endregion
	}
}
