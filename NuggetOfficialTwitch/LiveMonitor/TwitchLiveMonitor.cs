using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Services;

namespace NuggetTwitchIntegration.LiveMonitor
{
	public class TwitchLiveMonitor
	{
		LiveStreamMonitorService monitor;

		public TwitchLiveMonitor(List<string> channelNamesToMonitor, Func<string, string, string, string, string, Task> OnGoLiveAction)
		{
			Task.Run(() => ConfigureLiveMonitorAsync(channelNamesToMonitor, OnGoLiveAction));
		}

		async Task ConfigureLiveMonitorAsync(List<string> channelNamesToMonitor, Func<string, string, string, string, string, Task> OnGoLiveAction)
		{
			monitor = new LiveStreamMonitorService(NuggetOfficial.Instance.API);
			monitor.SetChannelsByName(channelNamesToMonitor);

			monitor.OnStreamOnline += (sender, e) => 
			{
				Task.Run(async () => await OnGoLiveAction(e.Stream.UserName, e.Stream.Title, e.Stream.GameName, e.Stream.ThumbnailUrl, null));
			};

			monitor.Start();

			await Task.Delay(-1);
		}
	}
}
