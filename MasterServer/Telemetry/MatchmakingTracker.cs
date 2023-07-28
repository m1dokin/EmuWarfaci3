using System;
using MasterServer.Core;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Matchmaking;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Telemetry
{
	// Token: 0x020006EC RID: 1772
	internal class MatchmakingTracker : IDisposable
	{
		// Token: 0x0600253A RID: 9530 RVA: 0x0009BCFC File Offset: 0x0009A0FC
		public MatchmakingTracker(TelemetryService service)
		{
			this.m_service = service;
			this.MatchmakingSystem = ServicesManager.GetService<IMatchmakingSystem>();
			this.RankSystem = ServicesManager.GetService<IRankSystem>();
			this.MatchmakingSystem.OnUnQueueEntity += this.OnUnQueueEntity;
		}

		// Token: 0x0600253B RID: 9531 RVA: 0x0009BD38 File Offset: 0x0009A138
		public void Dispose()
		{
		}

		// Token: 0x0600253C RID: 9532 RVA: 0x0009BD3C File Offset: 0x0009A13C
		private void OnUnQueueEntity(MMEntityInfo entity, EUnQueueReason reason)
		{
			DateTime utcNow = DateTime.UtcNow;
			this.m_service.AddMeasure(1L, new object[]
			{
				"stat",
				"matchmaking_aborted",
				"channel",
				Resources.ChannelName,
				"bucket1",
				(int)(utcNow - entity.Settings.StartTimeUtc).TotalSeconds,
				"bucket2",
				this.GetSoftChannelName(),
				"bucket3",
				reason.ToString()
			});
		}

		// Token: 0x0600253D RID: 9533 RVA: 0x0009BDD8 File Offset: 0x0009A1D8
		private string GetSoftChannelName()
		{
			Resources.ChannelRankGroup channelRankGroup = this.RankSystem.ChannelRankGroup;
			if (channelRankGroup == Resources.ChannelRankGroup.Newbie)
			{
				return "newbie";
			}
			if (channelRankGroup != Resources.ChannelRankGroup.Skilled)
			{
				return null;
			}
			return "skilled";
		}

		// Token: 0x040012C9 RID: 4809
		private TelemetryService m_service;

		// Token: 0x040012CA RID: 4810
		private IMatchmakingSystem MatchmakingSystem;

		// Token: 0x040012CB RID: 4811
		private IRankSystem RankSystem;
	}
}
