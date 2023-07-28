using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DedicatedPoolServer.Model;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.ServerInfo;
using MasterServer.Telemetry;
using MasterServer.Telemetry.Metrics;
using Util.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000767 RID: 1895
	[Service]
	[Singleton]
	internal class GameRoomServer : ServiceModule, IGameRoomServer
	{
		// Token: 0x0600274E RID: 10062 RVA: 0x000A614C File Offset: 0x000A454C
		public GameRoomServer(IGameRoomManager gameRoomManager, IServerInfo serverInfo, IOnlineClient onlineClientService, ITelemetryService telemetryService, IServerInfoTracker infoTracker)
		{
			this.m_roomManager = gameRoomManager;
			this.m_srvInfo = serverInfo;
			this.m_onlineClientService = onlineClientService;
			this.m_telemetryService = telemetryService;
			this.m_infoTracker = infoTracker;
		}

		// Token: 0x170003AE RID: 942
		// (get) Token: 0x0600274F RID: 10063 RVA: 0x000A6184 File Offset: 0x000A4584
		// (set) Token: 0x06002750 RID: 10064 RVA: 0x000A618C File Offset: 0x000A458C
		public bool CleanupStaleServers { get; set; }

		// Token: 0x170003AF RID: 943
		// (get) Token: 0x06002751 RID: 10065 RVA: 0x000A6195 File Offset: 0x000A4595
		// (set) Token: 0x06002752 RID: 10066 RVA: 0x000A619D File Offset: 0x000A459D
		private bool EnableLocalServer { get; set; }

		// Token: 0x06002753 RID: 10067 RVA: 0x000A61A8 File Offset: 0x000A45A8
		public override void Init()
		{
			this.CleanupStaleServers = true;
			this.EnableLocalServer = false;
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.Get("RequestQueueSize", out this.m_requestMaxQueueSize);
			section.Get("RequestTTL", out this.m_requestTTL);
			this.m_requestConsumeSize = 0;
			this.m_requestThreadSleep = 10;
			if (section.HasValue("EnableLocalServer"))
			{
				this.EnableLocalServer = (section.Get("EnableLocalServer") == "1");
			}
			if (section.HasValue("RequestConsumeSize"))
			{
				section.Get("RequestConsumeSize", out this.m_requestConsumeSize);
			}
			if (section.HasValue("RequestThreadSleep"))
			{
				section.Get("RequestThreadSleep", out this.m_requestThreadSleep);
			}
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_reqIdx = 0U;
			this.m_requests = new Queue<GameRoomServer.RequestContext>(this.m_requestMaxQueueSize);
			this.m_requestsBuffer = new Queue<GameRoomServer.RequestContext>(this.m_requestMaxQueueSize);
			this.m_requestsThread = new Thread(new ParameterizedThreadStart(this.RequestsHandler))
			{
				Name = "Request thread"
			};
			this.m_requestsThread.Start();
			this.m_srvInfo.ServerEntityEvent += this.OnServerEntityUpdated;
		}

		// Token: 0x06002754 RID: 10068 RVA: 0x000A62F4 File Offset: 0x000A46F4
		public override void Stop()
		{
			try
			{
				this.m_requestsThread.Abort();
			}
			catch
			{
			}
		}

		// Token: 0x06002755 RID: 10069 RVA: 0x000A6328 File Offset: 0x000A4728
		public bool RequestServer(ulong roomId, string session_id, string serverId)
		{
			return this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_ASK_SERVER)
			{
				reqSrv = 
				{
					roomId = roomId,
					session_id = session_id,
					serverId = serverId
				}
			});
		}

		// Token: 0x06002756 RID: 10070 RVA: 0x000A6368 File Offset: 0x000A4768
		public void ReleaseServer(ulong roomId, string serverId)
		{
			this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_RELEASE_SERVER)
			{
				reqSrv = 
				{
					roomId = roomId,
					serverId = serverId
				}
			});
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x000A639C File Offset: 0x000A479C
		public void FailedServer(string serverId)
		{
			this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_FAILED_SERVER)
			{
				reqSrv = 
				{
					serverId = serverId
				}
			});
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x000A63C4 File Offset: 0x000A47C4
		public void OnMissionLoad(string server_id, MissionLoadResult result, string session_id)
		{
			this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_MISSION_LOAD_RESULT)
			{
				reqSrv = 
				{
					loadResult = result,
					session_id = session_id,
					serverId = server_id
				}
			});
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x000A6404 File Offset: 0x000A4804
		private void OnServerEntityUpdated(object sender, ServerEntityEventArgs e)
		{
			if (e.State == ServerEntityState.SERVER_CHANGED)
			{
				this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_CHANGED_SERVER)
				{
					reqSrv = 
					{
						serverId = e.ServerId,
						serverEntity = (ServerEntity)e.Entity.Clone()
					}
				});
			}
			else if (e.State == ServerEntityState.SERVER_UNBOUND)
			{
				this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_DELETED_SERVER)
				{
					reqSrv = 
					{
						serverId = e.ServerId
					}
				});
			}
			else if (e.State == ServerEntityState.SERVER_BINDING_FAILED)
			{
				this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_FAILED_SERVER)
				{
					reqSrv = 
					{
						serverId = e.ServerId
					}
				});
			}
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x000A64BC File Offset: 0x000A48BC
		private bool SendRequest(GameRoomServer.RequestContext re)
		{
			object requestsBuffer = this.m_requestsBuffer;
			lock (requestsBuffer)
			{
				re.idx = (this.m_reqIdx += 1U);
				if (this.m_requestsBuffer.Count > this.m_requestMaxQueueSize)
				{
					Log.Warning<int, string>("Request queue size {0} is too big, ignoring incoming new request {1}", this.m_requestsBuffer.Count, re.DumpToString());
					re.Dump();
					return false;
				}
				string text = string.Empty;
				switch (re.type)
				{
				case GameRoomServer.RequestType.RT_ASK_SERVER:
				case GameRoomServer.RequestType.RT_REQUESTED_SERVER:
				case GameRoomServer.RequestType.RT_RELEASE_SERVER:
				case GameRoomServer.RequestType.RT_MISSION_LOAD_RESULT:
					text = re.DumpToStringEx();
					goto IL_C0;
				case GameRoomServer.RequestType.RT_CHANGED_SERVER:
					goto IL_C0;
				}
				text = re.DumpToString();
				IL_C0:
				if (!string.IsNullOrEmpty(text))
				{
					Log.Info<string, int>("Request {0} added to list. Queue size {1}", text, this.m_requestsBuffer.Count);
				}
				re.Dump();
				this.m_requestsBuffer.Enqueue(re);
			}
			return true;
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x000A65E0 File Offset: 0x000A49E0
		private void RequestsHandler(object state)
		{
			CultureHelpers.SetNeutralThreadCulture();
			for (;;)
			{
				try
				{
					Thread.Sleep(this.m_requestThreadSleep);
					this.OnRequestHandler();
				}
				catch (ThreadAbortException e)
				{
					Log.Error(e);
					break;
				}
				catch (Exception e2)
				{
					Log.Error(e2);
				}
			}
		}

		// Token: 0x0600275C RID: 10076 RVA: 0x000A6644 File Offset: 0x000A4A44
		private void OnRequestHandler()
		{
			if (this.CleanupStaleServers && ++this.m_currentPeriod % 600 == 0)
			{
				this.DoCleanupStaleServers();
			}
			object requestsBuffer = this.m_requestsBuffer;
			int num;
			lock (requestsBuffer)
			{
				num = this.m_requests.Count;
				while (this.m_requestsBuffer.Count != 0)
				{
					if (this.m_requestConsumeSize != 0 && num > this.m_requestConsumeSize)
					{
						break;
					}
					GameRoomServer.RequestContext item = this.m_requestsBuffer.Dequeue();
					this.m_requests.Enqueue(item);
					num++;
				}
			}
			if (this.m_requests.Count == 0)
			{
				return;
			}
			this.m_infoTracker.ReportQueueSize(num);
			GameRoomServer.RequestContext requestContext = this.m_requests.Dequeue();
			if (requestContext.state == GameRoomServer.RequestState.RS_FINISHED)
			{
				Log.Info<string, int>("Request {0} finished. Queue size {1}", requestContext.DumpToString(), this.m_requests.Count);
				return;
			}
			if (requestContext.state == GameRoomServer.RequestState.RS_FAILED)
			{
				if (requestContext.type == GameRoomServer.RequestType.RT_ASK_SERVER)
				{
					this.m_infoTracker.ReportAskServerFailed();
				}
				Log.Info<string, int>("Request {0} failed. Queue size {1}", requestContext.DumpToString(), this.m_requests.Count);
				requestContext.Dump();
				requestContext.type = GameRoomServer.RequestType.RT_FAILED_SERVER;
			}
			else if ((DateTime.Now - requestContext.createTime).TotalSeconds > (double)this.m_requestTTL)
			{
				Log.Warning<string>("Request {0} too long cannot be handled. Removed", requestContext.DumpToString());
				requestContext.Dump();
				requestContext.type = GameRoomServer.RequestType.RT_FAILED_SERVER;
			}
			try
			{
				switch (requestContext.type)
				{
				case GameRoomServer.RequestType.RT_ASK_SERVER:
					this.HandleAskServer(requestContext);
					break;
				case GameRoomServer.RequestType.RT_REQUESTED_SERVER:
					this.HandleRequestedServer(requestContext);
					break;
				case GameRoomServer.RequestType.RT_CHANGED_SERVER:
					this.HandleChangedServer(requestContext);
					break;
				case GameRoomServer.RequestType.RT_RELEASE_SERVER:
					this.HandleReleaseServer(requestContext);
					break;
				case GameRoomServer.RequestType.RT_DELETED_SERVER:
					this.HandleDeletedServer(requestContext);
					break;
				case GameRoomServer.RequestType.RT_FAILED_SERVER:
				case GameRoomServer.RequestType.RT_FAILED_LOCK_REQUEST:
					this.HandleFailedRequest(requestContext);
					break;
				case GameRoomServer.RequestType.RT_MISSION_LOAD_RESULT:
					this.HandleMissionLoadResultRequest(requestContext);
					break;
				default:
					Log.Error<string>("Request {0} discarded: unsupported request type", requestContext.DumpToString());
					break;
				}
			}
			catch (RoomClosedException)
			{
				requestContext.state = GameRoomServer.RequestState.RS_FINISHED;
			}
			catch (Exception e)
			{
				Log.Error(e);
				requestContext.state = GameRoomServer.RequestState.RS_FAILED;
			}
			if (requestContext.state == GameRoomServer.RequestState.RS_FINISHED)
			{
				if (requestContext.type != GameRoomServer.RequestType.RT_CHANGED_SERVER)
				{
					Log.Info<string, int>("Request {0} finished. Queue size {1}", requestContext.DumpToString(), this.m_requests.Count);
				}
			}
			else
			{
				this.m_requests.Enqueue(requestContext);
			}
		}

		// Token: 0x0600275D RID: 10077 RVA: 0x000A692C File Offset: 0x000A4D2C
		private void DoCleanupStaleServers()
		{
			List<ServerEntity> list = new List<ServerEntity>();
			foreach (GameRoomServer.GameRoomServerEntity gameRoomServerEntity in this.m_servers.Values)
			{
				ServerEntity serverEntity;
				if (this.m_srvInfo.GetServer(gameRoomServerEntity.serverId, false, out serverEntity))
				{
					if (!this.m_srvInfo.IsLocalServer(serverEntity.ServerID))
					{
						if (gameRoomServerEntity.roomId != 0UL && this.m_roomManager.GetRoom(gameRoomServerEntity.roomId) == null)
						{
							Log.Warning<string, ulong>("Server {0} is bound to non-existing game room {1}, reseting", gameRoomServerEntity.serverId, gameRoomServerEntity.roomId);
							list.Add(serverEntity);
						}
						if (gameRoomServerEntity.roomId == 0UL && serverEntity.Status != EGameServerStatus.Free)
						{
							Log.Warning<string, EGameServerStatus>("Server {0} is not bound and not in free state ({1}), reseting", gameRoomServerEntity.serverId, serverEntity.Status);
							list.Add(serverEntity);
						}
					}
				}
			}
			foreach (ServerEntity serverEntity2 in list)
			{
				this.m_srvInfo.ReleaseServer(serverEntity2);
				this.m_servers.Remove(serverEntity2.ServerID);
			}
		}

		// Token: 0x0600275E RID: 10078 RVA: 0x000A6AA0 File Offset: 0x000A4EA0
		private void HandleMissionLoadResultRequest(GameRoomServer.RequestContext ctx)
		{
			foreach (GameRoomServer.RequestContext requestContext in from req in this.m_requests
			where req.reqSrv.serverId == ctx.reqSrv.serverId && req.type == GameRoomServer.RequestType.RT_ASK_SERVER
			select req)
			{
				MissionLoadResult loadResult = ctx.reqSrv.loadResult;
				if (loadResult != MissionLoadResult.MLR_OK)
				{
					if (loadResult != MissionLoadResult.MLR_FAILED)
					{
						if (loadResult == MissionLoadResult.MLR_NOT_OWER)
						{
							this.m_servers.Remove(ctx.reqSrv.serverId);
							requestContext.state = GameRoomServer.RequestState.RS_IN_PROGRESS;
						}
					}
					else
					{
						Log.Warning<string>("Server {0} is alive but failed to load a mission, please check its content", ctx.reqSrv.serverId);
						this.m_servers.Remove(ctx.reqSrv.serverId);
						requestContext.state = GameRoomServer.RequestState.RS_IN_PROGRESS;
					}
				}
				else
				{
					requestContext.state = ((!(ctx.reqSrv.session_id == requestContext.reqSrv.session_id)) ? GameRoomServer.RequestState.RS_FAILED : GameRoomServer.RequestState.RS_MISSION_LOADING);
				}
			}
			ctx.state = GameRoomServer.RequestState.RS_FINISHED;
		}

		// Token: 0x0600275F RID: 10079 RVA: 0x000A6BE8 File Offset: 0x000A4FE8
		private void HandleFailedRequest(GameRoomServer.RequestContext ctx)
		{
			if (ctx.reqSrv.roomId == 0UL && !string.IsNullOrEmpty(ctx.reqSrv.serverId) && this.m_servers.ContainsKey(ctx.reqSrv.serverId))
			{
				ctx.reqSrv.roomId = this.m_servers[ctx.reqSrv.serverId].roomId;
			}
			GameRoomServer.RequestContext requestContext = this.m_requests.FirstOrDefault((GameRoomServer.RequestContext req) => req.type == GameRoomServer.RequestType.RT_ASK_SERVER && (req.reqSrv.serverId == ctx.reqSrv.serverId || req.reqSrv.roomId == ctx.reqSrv.roomId));
			if (requestContext != null)
			{
				if (requestContext.state == GameRoomServer.RequestState.RS_ASKED_FOR_SERVER)
				{
					requestContext.state = GameRoomServer.RequestState.RS_ASK_FOR_SERVER_FAILED;
				}
				if (ctx.type == GameRoomServer.RequestType.RT_FAILED_LOCK_REQUEST && this.m_srvInfo.IsLocalServer(requestContext.reqSrv.serverId))
				{
					ctx.state = GameRoomServer.RequestState.RS_FINISHED;
					return;
				}
			}
			GameRoomServer.GameRoomServerEntity gameRoomServerEntity;
			if (!string.IsNullOrEmpty(ctx.reqSrv.serverId) && this.m_servers.TryGetValue(ctx.reqSrv.serverId, out gameRoomServerEntity) && this.m_srvInfo.ReleaseServer(ctx.reqSrv.serverId, ctx.reqSrv.session_id))
			{
				this.m_servers.Remove(ctx.reqSrv.serverId);
				Log.Info<string>("Request {0}. Releasing server due to the failed request", ctx.DumpToStringEx());
			}
			if (ctx.reqSrv.roomId != 0UL)
			{
				IGameRoom room = this.m_roomManager.GetRoom(ctx.reqSrv.roomId);
				if (room != null)
				{
					room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.GetExtension<ServerExtension>().BindServerFailed();
					});
				}
			}
			this.m_telemetryService.AddMeasure(1L, new object[]
			{
				"stat",
				"room_srv_allocation_fail",
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				DateTime.Now.ToString("yyyy-MM-dd")
			});
			ctx.state = GameRoomServer.RequestState.RS_FINISHED;
		}

		// Token: 0x06002760 RID: 10080 RVA: 0x000A6E54 File Offset: 0x000A5254
		private void HandleDeletedServer(GameRoomServer.RequestContext ctx)
		{
			GameRoomServer.RequestServerEntity reqSrv = ctx.reqSrv;
			if (!string.IsNullOrEmpty(reqSrv.serverId) && this.m_servers.ContainsKey(reqSrv.serverId))
			{
				ulong roomId = this.m_servers[reqSrv.serverId].roomId;
				this.m_servers.Remove(reqSrv.serverId);
				Log.Info<string>("Request {0}. Server has been deleted", ctx.DumpToStringEx());
				if (roomId != 0UL)
				{
					IGameRoom room = this.m_roomManager.GetRoom(roomId);
					if (room != null)
					{
						room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.GetExtension<ServerExtension>().UnbindServer(true);
						});
					}
				}
			}
			ctx.state = GameRoomServer.RequestState.RS_FINISHED;
		}

		// Token: 0x06002761 RID: 10081 RVA: 0x000A6F10 File Offset: 0x000A5310
		private void HandleReleaseServer(GameRoomServer.RequestContext ctx)
		{
			GameRoomServer.RequestServerEntity reqSrv = ctx.reqSrv;
			GameRoomServer.GameRoomServerEntity gameRoomServerEntity;
			if (!string.IsNullOrEmpty(reqSrv.serverId) && this.m_servers.TryGetValue(reqSrv.serverId, out gameRoomServerEntity))
			{
				if (reqSrv.roomId != gameRoomServerEntity.roomId && gameRoomServerEntity.roomId != 0UL)
				{
					Log.Warning<string, ulong>("Request {0}. Room tries to release invalid server (server 'believes' it is owned by room {1})", ctx.DumpToStringEx(), gameRoomServerEntity.roomId);
					ctx.state = GameRoomServer.RequestState.RS_FINISHED;
					return;
				}
				if (this.m_srvInfo.ReleaseServer(reqSrv.serverId, false))
				{
					gameRoomServerEntity.InvalidateEntity();
					Log.Info<string>("Request {0}. Server has been released", ctx.DumpToStringEx());
					IGameRoom room = this.m_roomManager.GetRoom(reqSrv.roomId);
					if (room != null)
					{
						room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.GetExtension<ServerExtension>().UnbindServer(false);
						});
					}
				}
			}
			ctx.state = GameRoomServer.RequestState.RS_FINISHED;
		}

		// Token: 0x06002762 RID: 10082 RVA: 0x000A6FF8 File Offset: 0x000A53F8
		private void HandleRequestedServer(GameRoomServer.RequestContext ctx)
		{
			GameRoomServer.RequestContext requestContext = this.m_requests.FirstOrDefault((GameRoomServer.RequestContext r) => r.reqSrv.roomId == ctx.reqSrv.roomId);
			if (requestContext != null)
			{
				if (requestContext.state == GameRoomServer.RequestState.RS_ASKED_FOR_SERVER)
				{
					requestContext.reqSrv.serverId = ctx.reqSrv.serverId;
				}
			}
			else
			{
				this.m_srvInfo.ReleaseServer(ctx.reqSrv.serverId, ctx.reqSrv.session_id);
			}
			ctx.state = GameRoomServer.RequestState.RS_FINISHED;
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x000A7094 File Offset: 0x000A5494
		private void HandleAskServer(GameRoomServer.RequestContext ctx)
		{
			if (ctx.state == GameRoomServer.RequestState.RS_ASKED_FOR_MISSION || ctx.state == GameRoomServer.RequestState.RS_MISSION_LOADING)
			{
				return;
			}
			GameRoomServer.RequestServerEntity req = ctx.reqSrv;
			IGameRoom room = this.m_roomManager.GetRoom(req.roomId);
			if (room == null)
			{
				ctx.state = GameRoomServer.RequestState.RS_FAILED;
				return;
			}
			string buildTypeInRoom = string.Empty;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				buildTypeInRoom = r.GetUsersBuildType();
			});
			if (this.m_srvInfo.IsLocalServer(req.serverId))
			{
				if (!this.EnableLocalServer)
				{
					return;
				}
				ServerEntity serverEntity;
				if (this.m_srvInfo.GetServer(req.serverId, true, out serverEntity))
				{
					if (room.IsPveMode() && serverEntity.Mode == DedicatedMode.PurePVP)
					{
						Log.Warning<string, string>("Request {0}. PVE game room asked for local server {1} run in pure PVP mode", ctx.DumpToStringEx(), serverEntity.OnlineID);
						ctx.state = GameRoomServer.RequestState.RS_FAILED;
					}
					else
					{
						this.RequestMissionLoad(ctx, serverEntity, room);
					}
				}
				else if (ctx.state != GameRoomServer.RequestState.RS_ASKED_FOR_SERVER)
				{
					DedicatedMode mode = (!room.IsPveMode()) ? DedicatedMode.PurePVP : DedicatedMode.PVP_PVE;
					this.m_srvInfo.RequestServerByServerId(mode, buildTypeInRoom, req.serverId).ContinueWith(delegate(Task<DedicatedInfo> parent)
					{
						this.HandleLockServerResponse(parent, req.roomId, req.serverId, req.session_id);
					});
					ctx.state = GameRoomServer.RequestState.RS_ASKED_FOR_SERVER;
				}
			}
			else if (ctx.state == GameRoomServer.RequestState.RS_ASK_FOR_SERVER_FAILED)
			{
				ctx.state = GameRoomServer.RequestState.RS_FAILED;
			}
			else
			{
				this.FindFreeDedicatedServer(ctx, room, buildTypeInRoom);
			}
		}

		// Token: 0x06002764 RID: 10084 RVA: 0x000A7224 File Offset: 0x000A5624
		private static float RankServer(ServerEntity ent, bool isPvpMode, string node, int dedicatedPerformanceRange)
		{
			int num = (!(ent.Node == node)) ? 0 : dedicatedPerformanceRange;
			return (!isPvpMode || ent.Mode != DedicatedMode.PurePVP) ? (10000f + ent.PerformanceIndex - (float)num) : (ent.PerformanceIndex - (float)num);
		}

		// Token: 0x06002765 RID: 10085 RVA: 0x000A7278 File Offset: 0x000A5678
		private bool TryGetFreeDedicatedServer(IGameRoom gameRoom, string buildTypeInRoom, ulong reqRoomId, string reqSessionId, out ServerEntity suitableCandidate)
		{
			suitableCandidate = null;
			if (!this.m_srvInfo.IsGlobalLbsEnabled)
			{
				string node = this.m_onlineClientService.Server;
				List<ServerEntity> source = (from srv in this.m_srvInfo.GetBoundServers(true)
				where !this.m_srvInfo.IsLocalServer(srv.ServerID) && (srv.Mode == DedicatedMode.PVP_PVE || gameRoom.IsPvpMode()) && this.m_servers.ContainsKey(srv.ServerID) && this.m_servers[srv.ServerID].roomId == 0UL && srv.BuildType == buildTypeInRoom && srv.Status == EGameServerStatus.Free
				orderby GameRoomServer.RankServer(srv, gameRoom.IsPvpMode(), node, this.m_srvInfo.SearchByNodePerformanceRange)
				select srv).ToList<ServerEntity>();
				if (source.Any<ServerEntity>())
				{
					suitableCandidate = source.FirstOrDefault<ServerEntity>();
					return true;
				}
			}
			DedicatedMode mode = (!gameRoom.IsPveMode()) ? DedicatedMode.PurePVP : DedicatedMode.PVP_PVE;
			string regionId = null;
			gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom room)
			{
				regionId = room.RegionId;
			});
			this.m_srvInfo.RequestServer(mode, buildTypeInRoom, regionId).ContinueWith(delegate(Task<DedicatedInfo> parent)
			{
				this.HandleLockServerResponse(parent, reqRoomId, null, reqSessionId);
			});
			return false;
		}

		// Token: 0x06002766 RID: 10086 RVA: 0x000A7390 File Offset: 0x000A5790
		private void FindFreeDedicatedServer(GameRoomServer.RequestContext ctx, IGameRoom gameRoom, string buildTypeInRoom)
		{
			ServerEntity serverEntity = null;
			if (ctx.state == GameRoomServer.RequestState.RS_ASKED_FOR_SERVER)
			{
				this.m_srvInfo.GetServer(ctx.reqSrv.serverId, true, out serverEntity);
			}
			else if (ctx.state == GameRoomServer.RequestState.RS_IN_PROGRESS)
			{
				this.TryGetFreeDedicatedServer(gameRoom, buildTypeInRoom, ctx.reqSrv.roomId, ctx.reqSrv.session_id, out serverEntity);
				ctx.state = GameRoomServer.RequestState.RS_ASKED_FOR_SERVER;
			}
			if (serverEntity == null)
			{
				return;
			}
			this.RequestMissionLoad(ctx, serverEntity, gameRoom);
		}

		// Token: 0x06002767 RID: 10087 RVA: 0x000A7410 File Offset: 0x000A5810
		private void HandleLockServerResponse(Task<DedicatedInfo> parentTask, ulong roomId, string serverId, string sessionId)
		{
			if (parentTask.IsFaulted)
			{
				Log.Error(parentTask.Exception);
				this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_FAILED_LOCK_REQUEST)
				{
					reqSrv = 
					{
						roomId = roomId
					}
				});
				return;
			}
			string text = serverId ?? string.Empty;
			string text2 = (parentTask.Result == null) ? null : parentTask.Result.DedicatedId;
			if (!string.IsNullOrEmpty(text2) && (string.IsNullOrEmpty(text) || string.Equals(text2, text, StringComparison.OrdinalIgnoreCase)))
			{
				this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_REQUESTED_SERVER)
				{
					reqSrv = 
					{
						roomId = roomId,
						serverId = text2,
						session_id = sessionId
					}
				});
			}
			else
			{
				this.SendRequest(new GameRoomServer.RequestContext(GameRoomServer.RequestType.RT_FAILED_LOCK_REQUEST)
				{
					reqSrv = 
					{
						roomId = roomId
					}
				});
			}
		}

		// Token: 0x06002768 RID: 10088 RVA: 0x000A74F4 File Offset: 0x000A58F4
		private void RequestMissionLoad(GameRoomServer.RequestContext context, ServerEntity sEntity, IGameRoom gameRoom)
		{
			GameRoomServer.RequestServerEntity reqSrv = context.reqSrv;
			GameRoomServer.GameRoomServerEntity gameRoomServerEntity;
			if (!this.m_servers.TryGetValue(sEntity.ServerID, out gameRoomServerEntity))
			{
				return;
			}
			QueryManager.RequestSt("mission_load", sEntity.OnlineID, new object[]
			{
				gameRoom,
				reqSrv.session_id
			});
			reqSrv.serverId = sEntity.ServerID;
			context.state = GameRoomServer.RequestState.RS_ASKED_FOR_MISSION;
			gameRoomServerEntity.roomId = reqSrv.roomId;
			Log.Info<string>("Request {0}. Server is bound to room", context.DumpToStringEx());
		}

		// Token: 0x06002769 RID: 10089 RVA: 0x000A7574 File Offset: 0x000A5974
		private void CheckForWaitingRequests(GameRoomServer.RequestContext ctx)
		{
			foreach (GameRoomServer.RequestContext requestContext in this.m_requests)
			{
				if (requestContext.state == GameRoomServer.RequestState.RS_MISSION_LOADING && !(requestContext.reqSrv.serverId != ctx.reqSrv.serverId))
				{
					if (ctx.reqSrv.serverEntity.Status == EGameServerStatus.Ready || ctx.reqSrv.serverEntity.Status == EGameServerStatus.Waiting || ctx.reqSrv.serverEntity.Status == EGameServerStatus.Playing)
					{
						if (requestContext.reqSrv.session_id == ctx.reqSrv.serverEntity.SessionID)
						{
							if (this.SetRoomServer(requestContext.reqSrv.roomId, ctx.reqSrv.serverEntity))
							{
								requestContext.state = GameRoomServer.RequestState.RS_FINISHED;
								Log.Info<string, string>("Request {0}. Set server to the room (request {1})", ctx.DumpToString(), requestContext.DumpToStringEx());
							}
							else
							{
								requestContext.state = GameRoomServer.RequestState.RS_FAILED;
								Log.Info<string, string>("Request {0}. Setting server to the room failed (request: {1})", ctx.DumpToString(), requestContext.DumpToStringEx());
							}
						}
						else if (!string.IsNullOrEmpty(ctx.reqSrv.serverEntity.SessionID))
						{
							requestContext.state = GameRoomServer.RequestState.RS_FAILED;
						}
					}
					break;
				}
			}
		}

		// Token: 0x0600276A RID: 10090 RVA: 0x000A76F8 File Offset: 0x000A5AF8
		private void HandleChangedServer(GameRoomServer.RequestContext ctx)
		{
			if (!this.m_servers.ContainsKey(ctx.reqSrv.serverId))
			{
				this.m_servers.Add(ctx.reqSrv.serverId, new GameRoomServer.GameRoomServerEntity(ctx.reqSrv.serverId));
				Log.Info<string, string>("Request {0}. Server {1} added to the server list", ctx.DumpToString(), ctx.reqSrv.serverId);
			}
			this.CheckForWaitingRequests(ctx);
			GameRoomServer.GameRoomServerEntity gameRoomServerEntity = this.m_servers[ctx.reqSrv.serverId];
			if (gameRoomServerEntity.roomId != 0UL)
			{
				IGameRoom gr = this.m_roomManager.GetRoom(gameRoomServerEntity.roomId);
				if (gr != null)
				{
					gr.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.GetExtension<ServerExtension>().Update();
						Log.Info<string, string, ulong>("Request {0}. Server {1} state updated for room {2}", ctx.DumpToString(), ctx.reqSrv.serverId, gr.ID);
					});
				}
			}
			ctx.state = GameRoomServer.RequestState.RS_FINISHED;
		}

		// Token: 0x0600276B RID: 10091 RVA: 0x000A7810 File Offset: 0x000A5C10
		private void OnConfigChanged(ConfigEventArgs e)
		{
			if (string.Equals(e.Name, "RequestQueueSize", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_requestMaxQueueSize = e.iValue;
			}
			else if (string.Equals(e.Name, "RequestTTL", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_requestTTL = e.iValue;
			}
			else if (string.Equals(e.Name, "RequestConsumeSize", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_requestConsumeSize = e.iValue;
			}
			else if (string.Equals(e.Name, "RequestThreadSleep", StringComparison.CurrentCultureIgnoreCase))
			{
				this.m_requestThreadSleep = e.iValue;
			}
			else if (string.Equals(e.Name, "EnableLocalServer", StringComparison.CurrentCultureIgnoreCase))
			{
				this.EnableLocalServer = (e.sValue == "1");
			}
		}

		// Token: 0x0600276C RID: 10092 RVA: 0x000A78E8 File Offset: 0x000A5CE8
		private bool SetRoomServer(ulong room_id, ServerEntity server)
		{
			IGameRoom room = this.m_roomManager.GetRoom(room_id);
			if (room == null)
			{
				return false;
			}
			bool bound = true;
			try
			{
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					bound = r.GetExtension<ServerExtension>().BindServer(server);
				});
				GameRoomServer.GameRoomServerEntity gameRoomServerEntity;
				if (this.m_servers.TryGetValue(server.ServerID, out gameRoomServerEntity))
				{
					gameRoomServerEntity.roomId = room_id;
				}
			}
			catch (Exception)
			{
				room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					Log.Warning<string, string, string>("Faild to bind server {0} to room {1} (session: {2})", server.ServerID, r.RoomName, r.SessionID);
				});
				throw;
			}
			return bound;
		}

		// Token: 0x04001446 RID: 5190
		private Thread m_requestsThread;

		// Token: 0x04001447 RID: 5191
		private readonly Dictionary<string, GameRoomServer.GameRoomServerEntity> m_servers = new Dictionary<string, GameRoomServer.GameRoomServerEntity>();

		// Token: 0x04001448 RID: 5192
		private Queue<GameRoomServer.RequestContext> m_requests;

		// Token: 0x04001449 RID: 5193
		private Queue<GameRoomServer.RequestContext> m_requestsBuffer;

		// Token: 0x0400144A RID: 5194
		private uint m_reqIdx;

		// Token: 0x0400144B RID: 5195
		private int m_requestMaxQueueSize;

		// Token: 0x0400144C RID: 5196
		private int m_requestTTL;

		// Token: 0x0400144D RID: 5197
		private int m_requestConsumeSize;

		// Token: 0x0400144E RID: 5198
		private int m_requestThreadSleep;

		// Token: 0x0400144F RID: 5199
		private int m_currentPeriod;

		// Token: 0x04001450 RID: 5200
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x04001451 RID: 5201
		private readonly IServerInfo m_srvInfo;

		// Token: 0x04001452 RID: 5202
		private readonly IOnlineClient m_onlineClientService;

		// Token: 0x04001453 RID: 5203
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04001454 RID: 5204
		private IServerInfoTracker m_infoTracker;

		// Token: 0x04001455 RID: 5205
		private const int REQUEST_THREAD_SLEEP_MSEC = 10;

		// Token: 0x04001456 RID: 5206
		private const int SERVER_CLEANUP_PERIOD = 600;

		// Token: 0x04001457 RID: 5207
		private const int REQUEST_CONSUME_SIZE = 0;

		// Token: 0x02000768 RID: 1896
		private enum RequestType
		{
			// Token: 0x0400145E RID: 5214
			RT_ASK_SERVER,
			// Token: 0x0400145F RID: 5215
			RT_REQUESTED_SERVER,
			// Token: 0x04001460 RID: 5216
			RT_CHANGED_SERVER,
			// Token: 0x04001461 RID: 5217
			RT_RELEASE_SERVER,
			// Token: 0x04001462 RID: 5218
			RT_DELETED_SERVER,
			// Token: 0x04001463 RID: 5219
			RT_FAILED_SERVER,
			// Token: 0x04001464 RID: 5220
			RT_FAILED_LOCK_REQUEST,
			// Token: 0x04001465 RID: 5221
			RT_MISSION_LOAD_RESULT
		}

		// Token: 0x02000769 RID: 1897
		private enum RequestState
		{
			// Token: 0x04001467 RID: 5223
			RS_IN_PROGRESS,
			// Token: 0x04001468 RID: 5224
			RS_FINISHED,
			// Token: 0x04001469 RID: 5225
			RS_FAILED,
			// Token: 0x0400146A RID: 5226
			RS_ASKED_FOR_MISSION,
			// Token: 0x0400146B RID: 5227
			RS_MISSION_LOADING,
			// Token: 0x0400146C RID: 5228
			RS_ASKED_FOR_SERVER,
			// Token: 0x0400146D RID: 5229
			RS_ASK_FOR_SERVER_FAILED
		}

		// Token: 0x0200076A RID: 1898
		private class RequestServerEntity
		{
			// Token: 0x06002770 RID: 10096 RVA: 0x000A79B1 File Offset: 0x000A5DB1
			public RequestServerEntity()
			{
				this.roomId = 0UL;
				this.session_id = string.Empty;
				this.serverId = string.Empty;
				this.loadResult = MissionLoadResult.MLR_OK;
				this.serverEntity = null;
			}

			// Token: 0x06002771 RID: 10097 RVA: 0x000A79E5 File Offset: 0x000A5DE5
			public string DumpToString()
			{
				return string.Format("room: {0}, srv: {1}, session: {2}", this.roomId, this.serverId, this.session_id);
			}

			// Token: 0x06002772 RID: 10098 RVA: 0x000A7A08 File Offset: 0x000A5E08
			public void Dump()
			{
				Log.Verbose("RequestServerEntity: roomId   = {0}", new object[]
				{
					this.roomId
				});
				Log.Verbose("RequestServerEntity: serverId = {0}", new object[]
				{
					this.serverId
				});
			}

			// Token: 0x0400146E RID: 5230
			public ulong roomId;

			// Token: 0x0400146F RID: 5231
			public string session_id;

			// Token: 0x04001470 RID: 5232
			public string serverId;

			// Token: 0x04001471 RID: 5233
			public MissionLoadResult loadResult;

			// Token: 0x04001472 RID: 5234
			public ServerEntity serverEntity;
		}

		// Token: 0x0200076B RID: 1899
		private class RequestContext
		{
			// Token: 0x06002773 RID: 10099 RVA: 0x000A7A41 File Offset: 0x000A5E41
			public RequestContext(GameRoomServer.RequestType rt)
			{
				this.idx = 0U;
				this.type = rt;
				this.createTime = DateTime.Now;
				this.state = GameRoomServer.RequestState.RS_IN_PROGRESS;
				this.reqSrv = new GameRoomServer.RequestServerEntity();
			}

			// Token: 0x06002774 RID: 10100 RVA: 0x000A7A74 File Offset: 0x000A5E74
			public string DumpToString()
			{
				return string.Format("{0}, id: {1}", this.type, this.idx);
			}

			// Token: 0x06002775 RID: 10101 RVA: 0x000A7A96 File Offset: 0x000A5E96
			public string DumpToStringEx()
			{
				return string.Format("{0}, id: {1}, reqSrv: [{2}]", this.type, this.idx, this.reqSrv.DumpToString());
			}

			// Token: 0x06002776 RID: 10102 RVA: 0x000A7AC4 File Offset: 0x000A5EC4
			public void Dump()
			{
				Log.Verbose("RequestContext: type   = {0}", new object[]
				{
					Enum.GetName(typeof(GameRoomServer.RequestType), this.type)
				});
				Log.Verbose("RequestContext: id     = {0}", new object[]
				{
					this.idx
				});
				this.reqSrv.Dump();
			}

			// Token: 0x04001473 RID: 5235
			public GameRoomServer.RequestType type;

			// Token: 0x04001474 RID: 5236
			public uint idx;

			// Token: 0x04001475 RID: 5237
			public DateTime createTime;

			// Token: 0x04001476 RID: 5238
			public GameRoomServer.RequestState state;

			// Token: 0x04001477 RID: 5239
			public GameRoomServer.RequestServerEntity reqSrv;
		}

		// Token: 0x0200076C RID: 1900
		private class GameRoomServerEntity
		{
			// Token: 0x06002777 RID: 10103 RVA: 0x000A7B27 File Offset: 0x000A5F27
			public GameRoomServerEntity(string sid)
			{
				this.serverId = sid;
				this.InvalidateEntity();
			}

			// Token: 0x06002778 RID: 10104 RVA: 0x000A7B3C File Offset: 0x000A5F3C
			public void InvalidateEntity()
			{
				this.roomId = 0UL;
			}

			// Token: 0x04001478 RID: 5240
			public string serverId;

			// Token: 0x04001479 RID: 5241
			public ulong roomId;
		}
	}
}
