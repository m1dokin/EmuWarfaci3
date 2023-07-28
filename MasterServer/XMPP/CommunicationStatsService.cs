using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.XMPP
{
	// Token: 0x0200080C RID: 2060
	[Service]
	[Singleton]
	internal class CommunicationStatsService : ICommunicationStatsService
	{
		// Token: 0x06002A33 RID: 10803 RVA: 0x000B6150 File Offset: 0x000B4550
		public CommunicationStatsService(IOnlineClient onlineClient, IUserRepository userRepository)
		{
			this.m_onlineClient = onlineClient;
			this.m_userRepository = userRepository;
		}

		// Token: 0x170003E1 RID: 993
		// (get) Token: 0x06002A34 RID: 10804 RVA: 0x000B6166 File Offset: 0x000B4566
		public int TotalOnlineUsers
		{
			get
			{
				this.UpdateStats();
				return this.m_totalOnlineUsers;
			}
		}

		// Token: 0x170003E2 RID: 994
		// (get) Token: 0x06002A35 RID: 10805 RVA: 0x000B6174 File Offset: 0x000B4574
		public Dictionary<string, int> ServerOnlineUsers
		{
			get
			{
				this.UpdateStats();
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (SOnlineServer sonlineServer in this.m_onlineClient.OnlineServers)
				{
					dictionary[sonlineServer.Resource] = sonlineServer.Users;
				}
				dictionary[Resources.XmppResource] = this.m_userRepository.GetOnlineUsersCount();
				return dictionary;
			}
		}

		// Token: 0x170003E3 RID: 995
		// (get) Token: 0x06002A36 RID: 10806 RVA: 0x000B6208 File Offset: 0x000B4608
		private string StatsStoreJid
		{
			get
			{
				return "k01." + CryOnline.CryOnlineGetInstance().GetConfiguration(Resources.XmppOnlineDomain).GetHost();
			}
		}

		// Token: 0x06002A37 RID: 10807 RVA: 0x000B6228 File Offset: 0x000B4628
		private void UpdateStats()
		{
			if (DateTime.UtcNow > this.m_nextUpdateTime)
			{
				QueryManager.RequestSt("communication_stats", this.StatsStoreJid, new object[0]);
				this.m_nextUpdateTime = DateTime.UtcNow.AddSeconds(60.0);
			}
		}

		// Token: 0x06002A38 RID: 10808 RVA: 0x000B627C File Offset: 0x000B467C
		public void StatsUpdate(int totalOnline)
		{
			this.m_totalOnlineUsers = totalOnline;
		}

		// Token: 0x0400167D RID: 5757
		private const int UPDATE_STATS_TIMEOUT_SEC = 60;

		// Token: 0x0400167E RID: 5758
		private DateTime m_nextUpdateTime;

		// Token: 0x0400167F RID: 5759
		private int m_totalOnlineUsers;

		// Token: 0x04001680 RID: 5760
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001681 RID: 5761
		private readonly IUserRepository m_userRepository;
	}
}
