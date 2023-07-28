using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DedicatedPoolServer.Model;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.Users;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006CC RID: 1740
	[OrphanService]
	[Singleton]
	internal class ServerLoadBalancing : ServiceModule
	{
		// Token: 0x0600249B RID: 9371 RVA: 0x000990EE File Offset: 0x000974EE
		public ServerLoadBalancing(IUserRepository userRepository, IServerInfo serverInfo, IOnlineClient onlineClient, IDALService dal)
		{
			this.m_userRepository = userRepository;
			this.m_serverInfo = serverInfo;
			this.m_onlineClient = onlineClient;
			this.m_dal = dal;
		}

		// Token: 0x0600249C RID: 9372 RVA: 0x00099114 File Offset: 0x00097514
		public override void Start()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("LoadBalancing");
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_config.Enabled = (LoadBalancingCommon.ToLoadBalancingType(section.Get("type")) == LoadBalancingTypes.Local);
			Log.Info<string>("Load balancing module is {0}", (!this.m_config.Enabled) ? "disabled" : "enabled");
			if (!this.m_config.Enabled)
			{
				return;
			}
			this.InitLocalLoadBalancing(section);
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x000991A4 File Offset: 0x000975A4
		private void BalancingJob(object state)
		{
			ServerLoadBalancing.Context config;
			lock (state)
			{
				if (this.m_jobInProgress)
				{
					Log.Warning("Balancing job already in progress, please increase timeout interval");
					return;
				}
				config = this.m_config;
				this.m_jobInProgress = true;
			}
			try
			{
				this.DoBalancing(config);
			}
			finally
			{
				lock (state)
				{
					this.m_jobInProgress = false;
				}
			}
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x00099250 File Offset: 0x00097650
		private void DoBalancing(ServerLoadBalancing.Context data)
		{
			if (!data.Enabled)
			{
				return;
			}
			if (this.m_userRepository.GetOnlineUsersCount() == 0)
			{
				return;
			}
			List<ServerEntity> boundServers = this.m_serverInfo.GetBoundServers(false);
			IEnumerable<ServerEntity> source = from ent in boundServers
			where ent.Mode == DedicatedMode.PurePVP
			select ent;
			IEnumerable<ServerEntity> source2 = from ent in boundServers
			where ent.Mode == DedicatedMode.PVP_PVE
			select ent;
			int num = 0;
			int num2 = 0;
			if (Resources.Channel == Resources.ChannelType.PVE)
			{
				num2 = data.MinPvE - source2.Count((ServerEntity ent) => ent.Status == EGameServerStatus.Free);
			}
			else
			{
				num = data.MinPvP - source.Count((ServerEntity ent) => ent.Status == EGameServerStatus.Free);
			}
			if (num > 0 || num2 > 0)
			{
				IEnumerable<ServerEntity> source3 = this.ReadServersList();
				if (num > 0)
				{
					this.Balance(data.MaxPerHost, data.MinPvP, num, (from ent in source3
					where ent.Mode == DedicatedMode.PurePVP
					select ent).ToList<ServerEntity>(), boundServers);
				}
				if (num2 > 0)
				{
					this.Balance(data.MaxPerHost, data.MinPvE, num2, (from ent in source3
					where ent.Mode == DedicatedMode.PVP_PVE
					select ent).ToList<ServerEntity>(), boundServers);
				}
			}
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x000993E4 File Offset: 0x000977E4
		private void Balance(int maxPerHosts, int minFree, int toBind, IList<ServerEntity> candidates, IList<ServerEntity> cached)
		{
			if (candidates.Any<ServerEntity>())
			{
				return;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (ServerEntity serverEntity in candidates)
			{
				if (!dictionary.ContainsKey(serverEntity.MasterServerId))
				{
					dictionary[serverEntity.MasterServerId] = 0;
				}
				Dictionary<string, int> dictionary2;
				string masterServerId;
				(dictionary2 = dictionary)[masterServerId = serverEntity.MasterServerId] = dictionary2[masterServerId] + 1;
			}
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
			using (IEnumerator<ServerEntity> enumerator2 = candidates.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					ServerEntity ent = enumerator2.Current;
					if (!dictionary3.ContainsKey(ent.Hostname))
					{
						dictionary4[ent.Hostname] = 0;
						dictionary3[ent.Hostname] = cached.Count((ServerEntity s) => s.MasterServerId == Resources.XmppResource && s.Hostname == ent.Hostname);
					}
				}
			}
			IEnumerable<SOnlineServer> onlineServers = this.m_onlineClient.OnlineServers;
			IOrderedEnumerable<ServerEntity> orderedEnumerable = from ent in candidates
			orderby ent.PerformanceIndex
			select ent;
			using (IEnumerator<ServerEntity> enumerator3 = orderedEnumerable.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					ServerEntity ent = enumerator3.Current;
					if (toBind == 0)
					{
						break;
					}
					int num = (int)(from e in dictionary3
					orderby e.Value
					select e.Value).Average();
					int num2 = (num <= 0) ? maxPerHosts : (num - dictionary3[ent.Hostname]);
					if (num2 >= 0)
					{
						if (num2 == 0)
						{
							num2 = maxPerHosts;
						}
						IEnumerable<ServerEntity> collection = from e in orderedEnumerable
						where e.Hostname == ent.Hostname
						select e;
						IEnumerable<ServerEntity> enumerable = Utils.Shuffle<ServerEntity>(new List<ServerEntity>(collection));
						using (IEnumerator<ServerEntity> enumerator4 = enumerable.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								ServerEntity ce = enumerator4.Current;
								if (num2 == 0 || toBind == 0)
								{
									break;
								}
								if (dictionary4[ce.Hostname] >= maxPerHosts)
								{
									break;
								}
								if (dictionary[ce.MasterServerId] > minFree || (onlineServers.Count((SOnlineServer ms) => ms.Users == 0 && ms.Resource == ce.MasterServerId) == 1 && dictionary[ce.MasterServerId] > 0))
								{
									num2--;
									toBind--;
									Dictionary<string, int> dictionary2;
									string masterServerId2;
									(dictionary2 = dictionary)[masterServerId2 = ce.MasterServerId] = dictionary2[masterServerId2] - 1;
									string hostname;
									(dictionary2 = dictionary4)[hostname = ce.Hostname] = dictionary2[hostname] + 1;
									string hostname2;
									(dictionary2 = dictionary3)[hostname2 = ce.Hostname] = dictionary2[hostname2] + 1;
									Log.Info<string>("Stealing dedicated server {0}", ce.OnlineID);
									QueryManager.RequestSt("bind_server_info", ce.OnlineID, new object[]
									{
										ce.ServerID
									});
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060024A0 RID: 9376 RVA: 0x00099814 File Offset: 0x00097C14
		private IEnumerable<ServerEntity> ReadServersList()
		{
			return (from se in this.m_dal.CommonSystem.GetFreeServers(Resources.XmppResource)
			select new ServerEntity(se)).ToList<ServerEntity>();
		}

		// Token: 0x060024A1 RID: 9377 RVA: 0x00099854 File Offset: 0x00097C54
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "type", StringComparison.CurrentCultureIgnoreCase))
			{
				this.HandleLoadBalancingTypeChanged(LoadBalancingCommon.ToLoadBalancingType(e.sValue));
			}
			else if (string.Equals(e.Name, "min_pvp", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_config.MinPvP = e.iValue;
			}
			else if (string.Equals(e.Name, "min_pve", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_config.MinPvE = e.iValue;
			}
			else if (string.Equals(e.Name, "max_per_host", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_config.MaxPerHost = e.iValue;
			}
			else if (string.Equals(e.Name, "timeout_sec", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_balancing.Change((long)(e.iValue * 10 * this.m_hashId), (long)(e.iValue * 1000));
			}
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x00099950 File Offset: 0x00097D50
		private void HandleLoadBalancingTypeChanged(LoadBalancingTypes newType)
		{
			bool flag = newType == LoadBalancingTypes.Local;
			if (flag == this.m_config.Enabled)
			{
				return;
			}
			if (!this.m_config.Enabled)
			{
				this.InitLocalLoadBalancing(Resources.ModuleSettings.GetSection("LoadBalancing"));
			}
			else
			{
				this.StopLoadBalancingTimer();
			}
			this.m_config.Enabled = flag;
			Log.Info<string>("Load balancing module is {0}", (!this.m_config.Enabled) ? "disabled" : "enabled");
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x000999DC File Offset: 0x00097DDC
		private void InitLocalLoadBalancing(ConfigSection sec)
		{
			this.m_config.MaxPerHost = int.Parse(sec.Get("max_per_host"));
			this.m_config.MinPvP = int.Parse(sec.Get("min_pvp"));
			this.m_config.MinPvE = int.Parse(sec.Get("min_pve"));
			this.m_config.Timeout = int.Parse(sec.Get("timeout_sec"));
			this.m_hashId = 100 + Resources.ServerName.GetHashCode() % 100;
			this.m_balancing = new SafeTimer(new TimerCallback(this.BalancingJob), this.m_config, (long)(this.m_config.Timeout * 10 * this.m_hashId), (long)(this.m_config.Timeout * 1000));
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x00099AB5 File Offset: 0x00097EB5
		private void StopLoadBalancingTimer()
		{
			if (this.m_balancing == null)
			{
				return;
			}
			this.m_balancing.Dispose();
			this.m_balancing = null;
		}

		// Token: 0x04001272 RID: 4722
		private SafeTimer m_balancing;

		// Token: 0x04001273 RID: 4723
		private ServerLoadBalancing.Context m_config;

		// Token: 0x04001274 RID: 4724
		private bool m_jobInProgress;

		// Token: 0x04001275 RID: 4725
		private int m_hashId;

		// Token: 0x04001276 RID: 4726
		private readonly IUserRepository m_userRepository;

		// Token: 0x04001277 RID: 4727
		private readonly IServerInfo m_serverInfo;

		// Token: 0x04001278 RID: 4728
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001279 RID: 4729
		private readonly IDALService m_dal;

		// Token: 0x020006CD RID: 1741
		private struct Context
		{
			// Token: 0x04001284 RID: 4740
			public bool Enabled;

			// Token: 0x04001285 RID: 4741
			public int MaxPerHost;

			// Token: 0x04001286 RID: 4742
			public int MinPvP;

			// Token: 0x04001287 RID: 4743
			public int MinPvE;

			// Token: 0x04001288 RID: 4744
			public int Timeout;
		}
	}
}
