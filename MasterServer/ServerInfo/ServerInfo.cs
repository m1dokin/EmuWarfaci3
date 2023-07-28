using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DedicatedPoolServer.Model;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.DedicatedService;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;
using NLog;
using Util.Common;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C8 RID: 1736
	[Service]
	[Singleton]
	internal class ServerInfo : ServiceModule, IServerInfo
	{
		// Token: 0x0600246F RID: 9327 RVA: 0x00097B00 File Offset: 0x00095F00
		public ServerInfo(IQueryManager queryManager, IDALService dalService, IOnlineClient onlineClient, IServerRepository serverRepository, IProcessingQueueMetricsTracker processingQueueMetricsTracker, IDedicatedService dedicatedService)
		{
			this.m_queryManager = queryManager;
			this.m_dalService = dalService;
			this.m_onlineClient = onlineClient;
			this.m_serverRepository = serverRepository;
			this.m_processingQueueMetricsTracker = processingQueueMetricsTracker;
			this.m_dedicatedService = dedicatedService;
		}

		// Token: 0x1400009C RID: 156
		// (add) Token: 0x06002470 RID: 9328 RVA: 0x00097BB0 File Offset: 0x00095FB0
		// (remove) Token: 0x06002471 RID: 9329 RVA: 0x00097BE8 File Offset: 0x00095FE8
		public event EventHandler<ServerEntityEventArgs> ServerEntityEvent;

		// Token: 0x1700038F RID: 911
		// (get) Token: 0x06002472 RID: 9330 RVA: 0x00097C1E File Offset: 0x0009601E
		public bool IsGlobalLbsEnabled
		{
			get
			{
				return this.m_isGlobalLbsEnabled;
			}
		}

		// Token: 0x17000390 RID: 912
		// (get) Token: 0x06002473 RID: 9331 RVA: 0x00097C28 File Offset: 0x00096028
		public bool IsReconnectByNodeEnabled
		{
			get
			{
				return this.m_isReconnectByNodeEnabled;
			}
		}

		// Token: 0x17000391 RID: 913
		// (get) Token: 0x06002474 RID: 9332 RVA: 0x00097C32 File Offset: 0x00096032
		public int SearchByNodePerformanceRange
		{
			get
			{
				return this.m_searchByNodePerformanceRange;
			}
		}

		// Token: 0x06002475 RID: 9333 RVA: 0x00097C3C File Offset: 0x0009603C
		public override void Init()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("LoadBalancing");
			this.m_isGlobalLbsEnabled = (LoadBalancingCommon.ToLoadBalancingType(section.Get("type")) == LoadBalancingTypes.Global);
			string s;
			if (section.Get("lds_node_reconnect_enabled", out s))
			{
				this.m_isReconnectByNodeEnabled = (int.Parse(s) > 0);
			}
			string s2;
			if (section.Get("lds_node_search_range", out s2))
			{
				this.m_searchByNodePerformanceRange = int.Parse(s2);
			}
			if (!section.Get("global_lbs_route", out this.m_globalLbsRoute))
			{
				this.m_globalLbsRoute = LoadBalancingCommon.GetLoadBalancingRouteDefault(this.m_onlineClient.XmppHost);
			}
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_serverRepository.OnServerBind += this.OnServerPresence;
			this.m_reconnectingServers.ItemExpired += this.ReconnectingServerExpired;
			this.m_lastSyncTime = default(DateTime);
		}

		// Token: 0x06002476 RID: 9334 RVA: 0x00097D32 File Offset: 0x00096132
		public override void Start()
		{
			this.m_dbupdateQueue = new ServerInfo.DBUpdateProcessingQueue(500, this.m_processingQueueMetricsTracker);
		}

		// Token: 0x06002477 RID: 9335 RVA: 0x00097D4A File Offset: 0x0009614A
		public override void Stop()
		{
			base.Stop();
			if (this.m_dbupdateQueue != null)
			{
				this.m_dbupdateQueue.Stop();
				this.m_dbupdateQueue = null;
			}
			this.m_askedServers.Dispose();
			this.m_reconnectingServers.Dispose();
		}

		// Token: 0x06002478 RID: 9336 RVA: 0x00097D88 File Offset: 0x00096188
		public bool GetServer(string serverId, bool connectedOnly, out ServerEntity ent)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				if (!string.IsNullOrEmpty(serverId) && (!connectedOnly || !this.m_reconnectingServers.ContainsKey(serverId)) && !this.m_removedServers.ContainsKey(serverId))
				{
					result = this.m_cachedServers.TryGetValue(serverId, out ent);
				}
				else
				{
					ent = null;
					result = false;
				}
			}
			return result;
		}

		// Token: 0x06002479 RID: 9337 RVA: 0x00097E14 File Offset: 0x00096214
		public bool GetTestServer(out ServerEntity ent)
		{
			ent = null;
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_dedicatedTestClients.Count <= 0)
				{
					return false;
				}
				foreach (KeyValuePair<string, ServerEntity> keyValuePair in this.m_dedicatedTestClients)
				{
					if (this.m_dedicatedTestClients[keyValuePair.Key].Status == EGameServerStatus.Free)
					{
						ent = this.m_dedicatedTestClients[keyValuePair.Key];
						this.m_dedicatedTestClients.Remove(keyValuePair.Key);
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600247A RID: 9338 RVA: 0x00097F00 File Offset: 0x00096300
		public List<ServerEntity> GetBoundServers(bool connectedOnly)
		{
			object @lock = this.m_lock;
			List<ServerEntity> result;
			lock (@lock)
			{
				IEnumerable<ServerEntity> collection;
				if (connectedOnly)
				{
					collection = from p in this.m_cachedServers
					where !this.m_reconnectingServers.ContainsKey(p.Key)
					select p.Value;
				}
				else
				{
					collection = this.m_cachedServers.Values;
				}
				result = new List<ServerEntity>(collection);
			}
			return result;
		}

		// Token: 0x0600247B RID: 9339 RVA: 0x00097F94 File Offset: 0x00096394
		public Task<DedicatedInfo> RequestServerByServerId(DedicatedMode mode, string buildTypeInRoom, string serverId)
		{
			if (string.IsNullOrEmpty(serverId) || !this.IsLocalServer(serverId))
			{
				return TaskHelpers.Failed<DedicatedInfo>(new InvalidOperationException("Server ID should not be empty or non-local!"));
			}
			return (!this.m_isGlobalLbsEnabled) ? TaskHelpers.Completed<DedicatedInfo>(new DedicatedInfo
			{
				DedicatedId = serverId
			}) : this.InternalRequestServer(mode, buildTypeInRoom, string.Empty, serverId);
		}

		// Token: 0x0600247C RID: 9340 RVA: 0x00097FFC File Offset: 0x000963FC
		public Task<DedicatedInfo> RequestServer(DedicatedMode mode, string buildTypeInRoom, string regionId)
		{
			if (string.IsNullOrEmpty(regionId))
			{
				return TaskHelpers.Failed<DedicatedInfo>(new ArgumentNullException("regionId"));
			}
			if (this.m_isGlobalLbsEnabled)
			{
				return this.InternalRequestServer(mode, buildTypeInRoom, regionId, string.Empty);
			}
			this.SyncServerDatabaseList();
			ServerEntity serverEntity = null;
			object @lock = this.m_lock;
			lock (@lock)
			{
				List<ServerEntity> candidateServers = this.GetCandidateServers(mode, buildTypeInRoom);
				if (candidateServers.Count == 0 && mode == DedicatedMode.PurePVP)
				{
					candidateServers = this.GetCandidateServers(DedicatedMode.PVP_PVE, buildTypeInRoom);
				}
				if (candidateServers.Count == 0)
				{
					return TaskHelpers.Completed<DedicatedInfo>(new DedicatedInfo
					{
						DedicatedId = string.Empty
					});
				}
				if (this.m_searchByNodePerformanceRange > 0)
				{
					string node = this.m_onlineClient.Server;
					serverEntity = (from s in candidateServers
					where s.Node == node
					select s).MaxEx((ServerEntity s) => s.PerformanceIndex);
				}
				if (serverEntity == null)
				{
					serverEntity = candidateServers[this.m_random.Next(0, candidateServers.Count)];
				}
				this.m_askedServers.Add(serverEntity.ServerID, serverEntity);
			}
			this.m_queryManager.Request("bind_server_info", serverEntity.OnlineID, new object[]
			{
				serverEntity.ServerID
			});
			return TaskHelpers.Completed<DedicatedInfo>(new DedicatedInfo
			{
				DedicatedId = serverEntity.ServerID
			});
		}

		// Token: 0x0600247D RID: 9341 RVA: 0x0009819C File Offset: 0x0009659C
		private Task<DedicatedInfo> InternalRequestServer(DedicatedMode mode, string buildType, string regionId, string serverId = "")
		{
			DedicatedFilter filter = new DedicatedFilter
			{
				MasterId = Resources.ServerName,
				Mode = mode,
				BuildType = buildType,
				RegionId = regionId,
				DedicatedId = serverId
			};
			return this.m_dedicatedService.LockDedicatedAsync(filter).ContinueWith<DedicatedInfo>(delegate(Task<DedicatedInfo> t)
			{
				if (t.IsFaulted)
				{
					throw new AggregateException(new Exception[]
					{
						t.Exception
					});
				}
				object @lock = this.m_lock;
				lock (@lock)
				{
					if (t.Result != null)
					{
						this.m_removedServers.Remove(t.Result.DedicatedId);
					}
				}
				return t.Result;
			});
		}

		// Token: 0x0600247E RID: 9342 RVA: 0x000981F8 File Offset: 0x000965F8
		private List<ServerEntity> GetCandidateServers(DedicatedMode mode, string buildTypeInRoom)
		{
			Jid msJid = Jid.Parse(Resources.Jid);
			List<ServerEntity> source = (from srv in this.m_databaseServers.Values
			orderby srv.PerformanceIndex
			where !this.m_askedServers.ContainsKey(srv.ServerID) && srv.Mode == mode && Jid.Parse(srv.OnlineID).Host == msJid.Host && (string.IsNullOrEmpty(buildTypeInRoom) || srv.BuildType == buildTypeInRoom)
			select srv).ToList<ServerEntity>();
			ServerEntity serverEntity = source.FirstOrDefault<ServerEntity>();
			float equalRange = (serverEntity == null) ? 0f : Math.Max(serverEntity.PerformanceIndex + (float)this.m_searchByNodePerformanceRange, 0f);
			return source.TakeWhile((ServerEntity x) => x.PerformanceIndex <= equalRange).ToList<ServerEntity>();
		}

		// Token: 0x0600247F RID: 9343 RVA: 0x000982C4 File Offset: 0x000966C4
		private void SyncServerDatabaseList()
		{
			DateTime now = DateTime.Now;
			if ((now - this.m_lastSyncTime).TotalSeconds < 60.0)
			{
				return;
			}
			this.m_lastSyncTime = now;
			try
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_databaseServers.Clear();
					foreach (SServerEntity record in this.m_dalService.CommonSystem.GetFreeServers(Resources.XmppResource))
					{
						ServerEntity serverEntity = new ServerEntity(record);
						this.m_databaseServers.Add(serverEntity.ServerID, serverEntity);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06002480 RID: 9344 RVA: 0x000983D0 File Offset: 0x000967D0
		public bool ReleaseServer(string serverId, string sessionId)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				ServerEntity serverEntity;
				result = (this.m_cachedServers.TryGetValue(serverId, out serverEntity) && (string.IsNullOrEmpty(sessionId) || serverEntity.SessionID == sessionId) && this.ReleaseServer(serverEntity));
			}
			return result;
		}

		// Token: 0x06002481 RID: 9345 RVA: 0x00098448 File Offset: 0x00096848
		public bool ReleaseServer(string serverId, bool isForcedRelease = false)
		{
			object @lock = this.m_lock;
			bool result;
			lock (@lock)
			{
				ServerEntity server;
				if (!this.m_cachedServers.TryGetValue(serverId, out server))
				{
					if (isForcedRelease)
					{
						result = this.ReleaseServer(new ServerEntity(new SServerEntity
						{
							OnlineId = string.Empty,
							ServerId = serverId
						}));
					}
					else
					{
						result = false;
					}
				}
				else
				{
					result = this.ReleaseServer(server);
				}
			}
			return result;
		}

		// Token: 0x06002482 RID: 9346 RVA: 0x000984E0 File Offset: 0x000968E0
		public bool ReleaseServer(ServerEntity server)
		{
			if (this.m_isGlobalLbsEnabled)
			{
				this.m_dedicatedService.UnlockDedicatedAsync(server.ServerID, Resources.ServerName).ContinueWith(delegate(Task t)
				{
					ServerInfo.Logger.Error<AggregateException>(t.Exception);
				}, TaskContinuationOptions.OnlyOnFaulted);
				this.RemoveServer(server.ServerID);
			}
			else if (server.Status != EGameServerStatus.Free)
			{
				this.m_queryManager.Request("mission_unload", server.OnlineID, new object[0]);
			}
			return true;
		}

		// Token: 0x06002483 RID: 9347 RVA: 0x00098574 File Offset: 0x00096974
		private void RemoveServer(string dedicatedId)
		{
			object @lock = this.m_lock;
			ServerEntity serverEntity;
			lock (@lock)
			{
				this.m_removedServers.Replace(dedicatedId, true);
				Dictionary<string, ServerEntity> dictionary = (!this.IsTestDediclient(dedicatedId)) ? this.m_cachedServers : this.m_dedicatedTestClients;
				if (dictionary.TryGetValue(dedicatedId, out serverEntity))
				{
					dictionary.Remove(dedicatedId);
				}
			}
			if (serverEntity != null)
			{
				ServerEntityEventArgs args = new ServerEntityEventArgs
				{
					ServerId = serverEntity.ServerID,
					State = ServerEntityState.SERVER_UNBOUND
				};
				this.NotifyListeners(args);
			}
		}

		// Token: 0x06002484 RID: 9348 RVA: 0x00098624 File Offset: 0x00096A24
		public void OnServerBound(bool bindOk, string serverId)
		{
			ServerEntityEventArgs args = new ServerEntityEventArgs
			{
				ServerId = serverId,
				State = ((!bindOk) ? ServerEntityState.SERVER_BINDING_FAILED : ServerEntityState.SERVER_BOUND)
			};
			this.NotifyListeners(args);
		}

		// Token: 0x06002485 RID: 9349 RVA: 0x0009865C File Offset: 0x00096A5C
		public void OnServerInfo(ServerEntity srv)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_removedServers.ContainsKey(srv.ServerID))
				{
					return;
				}
				Dictionary<string, ServerEntity> dictionary = (!this.IsTestDediclient(srv.ServerID)) ? this.m_cachedServers : this.m_dedicatedTestClients;
				ServerInfo.Logger.Debug<ServerEntity>("[ServerInfo] Received updated server entity:\n{0}", srv);
				dictionary[srv.ServerID] = srv;
				if (!this.m_isGlobalLbsEnabled)
				{
					if (this.m_isReconnectByNodeEnabled && (srv.Node != this.m_onlineClient.Server || Jid.Parse(srv.OnlineID).Host != this.m_onlineClient.XmppHost))
					{
						this.m_reconnectingServers[srv.ServerID] = srv;
						return;
					}
					this.m_reconnectingServers.Remove(srv.ServerID);
				}
			}
			ServerEntityEventArgs args = new ServerEntityEventArgs
			{
				ServerId = srv.ServerID,
				State = ServerEntityState.SERVER_CHANGED,
				Entity = srv
			};
			this.NotifyListeners(args);
			if (!this.IsLocalServer(srv.ServerID) && !this.IsTestDediclient(srv.ServerID) && !this.m_isGlobalLbsEnabled)
			{
				this.m_dbupdateQueue.Add(srv);
			}
		}

		// Token: 0x06002486 RID: 9350 RVA: 0x000987E0 File Offset: 0x00096BE0
		private void ReconnectingServerExpired(string key, ServerEntity data)
		{
			this.RemoveServer(key);
		}

		// Token: 0x06002487 RID: 9351 RVA: 0x000987E9 File Offset: 0x00096BE9
		private void OnServerPresence(bool bound, string onlineId, string serverId)
		{
			if (bound)
			{
				return;
			}
			if (this.IsGlobalLbsEnabled || !this.m_reconnectingServers.ContainsKey(serverId))
			{
				this.RemoveServer(serverId);
			}
		}

		// Token: 0x06002488 RID: 9352 RVA: 0x00098818 File Offset: 0x00096C18
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "global_lbs_route", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_globalLbsRoute = e.sValue;
			}
			else if (string.Equals(e.Name, "lds_node_search_range", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_searchByNodePerformanceRange = int.Parse(e.sValue);
			}
			else if (string.Equals(e.Name, "lds_node_reconnect_enabled", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_isReconnectByNodeEnabled = (int.Parse(e.sValue) > 0);
			}
			else if (string.Equals(e.Name, "type", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_isGlobalLbsEnabled = (LoadBalancingCommon.ToLoadBalancingType(e.sValue) == LoadBalancingTypes.Global);
			}
		}

		// Token: 0x06002489 RID: 9353 RVA: 0x000988D8 File Offset: 0x00096CD8
		public void NotifyListeners(ServerEntityEventArgs args)
		{
			if (this.ServerEntityEvent != null)
			{
				foreach (Delegate @delegate in this.ServerEntityEvent.GetInvocationList())
				{
					try
					{
						((EventHandler<ServerEntityEventArgs>)@delegate)(this, args);
					}
					catch (Exception e)
					{
						Log.Error(e);
					}
				}
			}
		}

		// Token: 0x0600248A RID: 9354 RVA: 0x00098944 File Offset: 0x00096D44
		public bool IsLocalServer(string serverId)
		{
			return !string.IsNullOrEmpty(serverId) && serverId.EndsWith("-local");
		}

		// Token: 0x0600248B RID: 9355 RVA: 0x0009895F File Offset: 0x00096D5F
		public bool IsTestDediclient(string serverId)
		{
			return !string.IsNullOrEmpty(serverId) && serverId.Contains("_ds_test");
		}

		// Token: 0x0600248C RID: 9356 RVA: 0x0009897C File Offset: 0x00096D7C
		public void DumpServers()
		{
			StringBuilder sb = new StringBuilder();
			object @lock = this.m_lock;
			lock (@lock)
			{
				sb.AppendLine("Cached servers:");
				foreach (ServerEntity serverEntity in this.m_cachedServers.Values)
				{
					sb.AppendFormat("\t{0} {1} {2} ({3}): {4}\n", new object[]
					{
						serverEntity.ServerID,
						serverEntity.Mode,
						serverEntity.PerformanceIndex,
						serverEntity.OnlineID,
						serverEntity.Status
					});
				}
				sb.AppendLine();
				this.SyncServerDatabaseList();
				sb.AppendFormat("Database servers (last synced {0}):\n", this.m_lastSyncTime.ToString(CultureInfo.InvariantCulture));
				foreach (ServerEntity serverEntity2 in this.m_databaseServers.Values)
				{
					sb.AppendFormat("\t{0} {1} {2} ({3}): {4}\n", new object[]
					{
						serverEntity2.ServerID,
						serverEntity2.Mode,
						serverEntity2.PerformanceIndex,
						serverEntity2.OnlineID,
						serverEntity2.Status
					});
				}
				sb.AppendLine();
				sb.AppendLine("Asked servers:");
				this.m_askedServers.Enumerate(delegate(string key, ServerEntity se)
				{
					sb.AppendFormat("\t{0} {1} {2} ({3}): {4}\n", new object[]
					{
						se.ServerID,
						se.Mode,
						se.PerformanceIndex,
						se.OnlineID,
						se.Status
					});
					return true;
				});
			}
			Log.Info(sb.ToString());
		}

		// Token: 0x0600248D RID: 9357 RVA: 0x00098BB8 File Offset: 0x00096FB8
		public void DumpServer(string serverId)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				bool flag2 = false;
				foreach (ServerEntity serverEntity in this.m_cachedServers.Values)
				{
					if (serverEntity.ServerID.Contains(serverId))
					{
						serverEntity.Dump();
						flag2 = true;
					}
				}
				if (!flag2)
				{
					foreach (ServerEntity serverEntity2 in this.m_databaseServers.Values)
					{
						if (serverEntity2.ServerID.Contains(serverId))
						{
							serverEntity2.Dump();
							flag2 = true;
						}
					}
					if (!flag2)
					{
						ServerInfo.Logger.Info("Server {0} not found", serverId);
					}
				}
			}
		}

		// Token: 0x0600248E RID: 9358 RVA: 0x00098CE4 File Offset: 0x000970E4
		public void DebugStealServer(string serverId)
		{
			IDALService service = ServicesManager.GetService<IDALService>();
			List<ServerEntity> list = (from srv in service.CommonSystem.GetFreeServers(Resources.XmppResource)
			where srv.ServerId.Contains(serverId)
			select new ServerEntity(srv)).ToList<ServerEntity>();
			foreach (ServerEntity serverEntity in list)
			{
				ServerInfo.Logger.Info("Stealing free server {0} ...", serverEntity.ServerID);
				this.m_queryManager.Request("bind_server_info", serverEntity.OnlineID, new object[]
				{
					serverEntity.ServerID
				});
			}
		}

		// Token: 0x04001245 RID: 4677
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		// Token: 0x04001246 RID: 4678
		private readonly IQueryManager m_queryManager;

		// Token: 0x04001247 RID: 4679
		private readonly IDALService m_dalService;

		// Token: 0x04001248 RID: 4680
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001249 RID: 4681
		private readonly IServerRepository m_serverRepository;

		// Token: 0x0400124A RID: 4682
		private readonly IProcessingQueueMetricsTracker m_processingQueueMetricsTracker;

		// Token: 0x0400124B RID: 4683
		private readonly IDedicatedService m_dedicatedService;

		// Token: 0x0400124C RID: 4684
		private const int DEF_SYNC_TIMEOUT_SEC = 60;

		// Token: 0x0400124D RID: 4685
		private const int DEF_ASK_AGAIN_TIMEOUT_SEC = 60;

		// Token: 0x0400124E RID: 4686
		private const int DEF_DB_UPDATE_QUEUE_LIMIT = 500;

		// Token: 0x0400124F RID: 4687
		private const int DEF_RECONNECTION_TIMEOUT_SEC = 15;

		// Token: 0x04001250 RID: 4688
		private const int DEF_REMOVED_SERVER_TIMEOUT_SEC = 600;

		// Token: 0x04001251 RID: 4689
		private readonly object m_lock = new object();

		// Token: 0x04001252 RID: 4690
		private readonly Random m_random = new Random(DateTime.Now.Millisecond);

		// Token: 0x04001253 RID: 4691
		private readonly Dictionary<string, ServerEntity> m_cachedServers = new Dictionary<string, ServerEntity>();

		// Token: 0x04001254 RID: 4692
		private readonly Dictionary<string, ServerEntity> m_databaseServers = new Dictionary<string, ServerEntity>();

		// Token: 0x04001255 RID: 4693
		private readonly Dictionary<string, ServerEntity> m_dedicatedTestClients = new Dictionary<string, ServerEntity>();

		// Token: 0x04001256 RID: 4694
		private readonly CacheDictionary<string, ServerEntity> m_askedServers = new CacheDictionary<string, ServerEntity>(60);

		// Token: 0x04001257 RID: 4695
		private readonly CacheDictionary<string, ServerEntity> m_reconnectingServers = new CacheDictionary<string, ServerEntity>(15);

		// Token: 0x04001258 RID: 4696
		private readonly CacheDictionary<string, bool> m_removedServers = new CacheDictionary<string, bool>(600);

		// Token: 0x04001259 RID: 4697
		private ServerInfo.DBUpdateProcessingQueue m_dbupdateQueue;

		// Token: 0x0400125A RID: 4698
		private DateTime m_lastSyncTime;

		// Token: 0x0400125B RID: 4699
		private volatile bool m_isGlobalLbsEnabled;

		// Token: 0x0400125C RID: 4700
		private volatile bool m_isReconnectByNodeEnabled;

		// Token: 0x0400125D RID: 4701
		private volatile int m_searchByNodePerformanceRange;

		// Token: 0x0400125E RID: 4702
		private string m_globalLbsRoute;

		// Token: 0x020006C9 RID: 1737
		private class DBUpdateProcessingQueue : ProcessingQueue<ServerEntity>
		{
			// Token: 0x06002497 RID: 9367 RVA: 0x00098EAA File Offset: 0x000972AA
			public DBUpdateProcessingQueue(int limit, IProcessingQueueMetricsTracker processingQueueMetricsTracker) : base("DBUpdateProcessingQueue", processingQueueMetricsTracker, true)
			{
				base.QueueLimit = limit;
			}

			// Token: 0x06002498 RID: 9368 RVA: 0x00098EC0 File Offset: 0x000972C0
			public override void Process(ServerEntity item)
			{
				SServerEntity entity = new SServerEntity
				{
					ServerId = item.ServerID,
					OnlineId = item.OnlineID,
					Hostname = item.Hostname,
					Port = item.Port,
					Node = item.Node,
					Mode = ((item.Mode != DedicatedMode.PurePVP) ? "pvp_pve" : "pure_pvp"),
					Status = (int)item.Status,
					MissionKey = item.Mission,
					BuildType = item.BuildType,
					PerformanceIndex = item.PerformanceIndex
				};
				IDALService service = ServicesManager.GetService<IDALService>();
				service.CommonSystem.UpdateServer(Resources.XmppResource, entity);
			}
		}
	}
}
