using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.GameRoom.RoomExtensions.Vote.Exceptions;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E7 RID: 1255
	[Service]
	[Singleton]
	internal class VoteConfigContainer : ServiceModule, IVoteConfigContainer
	{
		// Token: 0x06001B00 RID: 6912 RVA: 0x0006E90C File Offset: 0x0006CD0C
		public VoteConfigContainer(IConfigProvider<VotesConfig> votesConfigProvider)
		{
			this.m_votesConfigProvider = votesConfigProvider;
		}

		// Token: 0x06001B01 RID: 6913 RVA: 0x0006E91B File Offset: 0x0006CD1B
		public override void Start()
		{
			this.m_configs = this.m_votesConfigProvider.Get();
			this.m_votesConfigProvider.Changed += this.OnConfigChanged;
		}

		// Token: 0x06001B02 RID: 6914 RVA: 0x0006E945 File Offset: 0x0006CD45
		public override void Stop()
		{
			this.m_votesConfigProvider.Changed -= this.OnConfigChanged;
		}

		// Token: 0x06001B03 RID: 6915 RVA: 0x0006E960 File Offset: 0x0006CD60
		public VoteConfig GetConfig(VoteType voteType)
		{
			VoteConfig result;
			if (!this.m_configs.TryGetValue(voteType, out result))
			{
				throw new InvalidVoteTypeException(voteType.ToString());
			}
			return result;
		}

		// Token: 0x06001B04 RID: 6916 RVA: 0x0006E994 File Offset: 0x0006CD94
		private void OnConfigChanged(VotesConfig votesConfig)
		{
			this.m_configs = votesConfig;
		}

		// Token: 0x04000CE5 RID: 3301
		private readonly IConfigProvider<VotesConfig> m_votesConfigProvider;

		// Token: 0x04000CE6 RID: 3302
		private Dictionary<VoteType, VoteConfig> m_configs;
	}
}
