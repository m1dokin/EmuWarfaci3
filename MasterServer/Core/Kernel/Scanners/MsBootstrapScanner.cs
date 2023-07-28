using System;
using MasterServer.Core.Application.Bootstrap;
using MasterServer.Matchmaking;

namespace MasterServer.Core.Kernel.Scanners
{
	// Token: 0x0200011D RID: 285
	internal class MsBootstrapScanner : BootstrapScanner
	{
		// Token: 0x06000483 RID: 1155 RVA: 0x00013900 File Offset: 0x00011D00
		public override void Configure(BootstrapFilter filter)
		{
			filter.EnableService<GlobalMatchmakingPerformer>(() => Resources.Channel != Resources.ChannelType.Service);
			filter.EnableService<EmptyMatchmakingPerformer>(() => Resources.Channel == Resources.ChannelType.Service);
		}
	}
}
