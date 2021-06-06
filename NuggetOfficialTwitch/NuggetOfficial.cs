using NuggetTwitchIntegration.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using TwitchLib;
using TwitchLib.Api;

namespace NuggetTwitchIntegration
{
	//TODO because we will be working with Task and stuff, i think we need to make this thread safe and stuff
	/// <summary>
	/// Singleton wrapper around <see cref="TwitchAPI"/>
	/// </summary>
	public class NuggetOfficial
	{
		/// <summary>
		/// Gets the already existing <see cref="NuggetOfficial"/> instance. If one does not exist, one will be created with no data. It is advised you use new <see cref="NuggetOfficial(string, string)"/> before trying to access these properties
		/// </summary>
		public static NuggetOfficial Instance
		{
			get
			{
				if (instance is null) instance = new NuggetOfficial();

				return instance;
			}
			private set => instance = value;
		}
		private static NuggetOfficial instance;

		/// <summary>
		/// Get the singleton instance of <see cref="TwitchAPI"/>. If one does not existm one will be created with no data. It is advised you use new <see cref="NuggetOfficial(string, string)"/> before trying to access these properties
		/// </summary>
		public TwitchAPI API
		{
			get
			{
				if (api is null) api = new TwitchAPI();

				return api;
			}
			private set => api = value;
		}
		private static TwitchAPI api;

		NuggetOfficial() => API = new TwitchAPI();
		/// <summary>
		/// Create a new <see cref="TwitchAPI"/> with the provided <paramref name="clientId"/> and <paramref name="accessToken"/>, if one is provided. Will additionally populate <see cref="Instance"/> with a reference 
		/// to the created instance
		/// </summary>
		/// <param name="clientId">Client ID to pass to the <see cref="TwitchAPI"/> instance</param>
		/// <param name="accessToken">Access Token to pass to the <see cref="TwitchAPI"/> instance</param>
		public NuggetOfficial(string clientId, string accessToken = null)
		{
			API = new TwitchAPI();
			API.Settings.ClientId = clientId;
			API.Settings.AccessToken = accessToken ?? string.Empty;

			Instance = this;
		}
	}
}
