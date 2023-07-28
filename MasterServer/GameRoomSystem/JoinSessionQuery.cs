using System;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.ContractSystem;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.PerformanceSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.ServerInfo;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005EC RID: 1516
	[QueryAttributes(TagName = "session_join")]
	internal class JoinSessionQuery : BaseQuery
	{
		// Token: 0x06002042 RID: 8258 RVA: 0x0008230C File Offset: 0x0008070C
		public JoinSessionQuery(IBoosterService boosterService, ISessionStorage sessionStorage, IGameRoomManager gameRoomManager, IRankSystem rankSystem, IContractService contractService, IProfileItems profileItemsService, IServerInfo serverInfo, IRewardMultiplierService rewardMultiplierService, IItemStats itemsStats, IMissionPerformanceService performanceService)
		{
			this.m_boosterService = boosterService;
			this.m_sessionStorage = sessionStorage;
			this.m_gameRoomManager = gameRoomManager;
			this.m_rankSystem = rankSystem;
			this.m_contractService = contractService;
			this.m_profileItemsService = profileItemsService;
			this.m_serverInfo = serverInfo;
			this.m_rewardMultiplierService = rewardMultiplierService;
			this.m_itemsStats = itemsStats;
			this.m_performanceService = performanceService;
		}

		// Token: 0x06002043 RID: 8259 RVA: 0x0008236C File Offset: 0x0008076C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "JoinSessionQuery"))
			{
				UserInfo.User userInfo;
				if (!base.GetClientInfo(fromJid, out userInfo))
				{
					result = -3;
				}
				else
				{
					IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(userInfo.ProfileID);
					if (room == null)
					{
						Log.Warning(string.Format("Room not found for user: {0}", userInfo));
						result = -1;
					}
					else if (!this.m_rankSystem.CanJoinChannel(userInfo.Rank))
					{
						Log.Warning<UserInfo.User>("Player '{0}' will be kicked due to channel rank restriction.", userInfo);
						room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.GetExtension<KickExtension>().KickPlayer(userInfo.ProfileID, GameRoomPlayerRemoveReason.KickRankRestricted, true);
						});
						result = -1;
					}
					else
					{
						string sessionId = string.Empty;
						try
						{
							room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
							{
								RoomPlayer player = r.GetPlayer(userInfo.ProfileID);
								if (player == null)
								{
									throw new RoomGenericException(room.ID, string.Format("No such player {0}", userInfo));
								}
								if (player.RoomStatus != RoomPlayer.EStatus.Ready)
								{
									throw new RoomGenericException(room.ID, string.Format("Invalid player {0} status {1}", player, player.RoomStatus));
								}
								SessionState state = r.GetState<SessionState>(AccessMode.ReadOnly);
								if (state.Status != SessionStatus.Running)
								{
									throw new RoomGenericException(room.ID, string.Format("Invalid session status. Status: {0}", state.Status));
								}
								ServerEntity server = r.GetState<ServerState>(AccessMode.ReadOnly).Server;
								response.SetAttribute("room_id", r.ID.ToString());
								response.SetAttribute("server", server.ServerID);
								response.SetAttribute("hostname", server.Hostname);
								response.SetAttribute("port", server.Port.ToString());
								response.SetAttribute("local", (!this.m_serverInfo.IsLocalServer(server.ServerID)) ? "0" : "1");
								response.SetAttribute("session_id", server.SessionID);
								sessionId = r.SessionID;
							});
						}
						catch (RoomGenericException ex)
						{
							Log.Warning<string>("Join session failed: {0}", ex.Message);
							return -1;
						}
						room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.PlayerJoinedSession(userInfo.ProfileID);
						});
						result = ((!this.HandleJoinSession(sessionId, userInfo)) ? -1 : 0);
					}
				}
			}
			return result;
		}

		// Token: 0x06002044 RID: 8260 RVA: 0x00082510 File Offset: 0x00080910
		private bool HandleJoinSession(string sessionId, UserInfo.User user)
		{
			SessionBoosters.ProfileBoosters value = new SessionBoosters.ProfileBoosters
			{
				IsVip = this.m_boosterService.HasVipItem(user.ProfileID),
				Boosters = this.m_boosterService.GetBoosters(user.ProfileID)
			};
			SessionBoosters data = this.m_sessionStorage.GetData<SessionBoosters>(sessionId, ESessionData.Boosters);
			if (data == null)
			{
				Log.Warning(string.Format("Can't find booster info in sessionStorage, session: {0}, user: {1}", sessionId, user));
				return false;
			}
			data.Boosters[user.ProfileID] = value;
			ProfileContract profileContract = this.m_contractService.GetProfileContract(user.ProfileID);
			if (profileContract != null && profileContract.Status == ProfileContract.ContractStatus.eCS_InProgress)
			{
				SessionContracts data2 = this.m_sessionStorage.GetData<SessionContracts>(sessionId, ESessionData.Contracts);
				if (data2 == null)
				{
					Log.Warning(string.Format("Can't find contracts info in sessionStorage, session: {0}, user: {1}", sessionId, user));
					return false;
				}
				SProfileItem profileItem = this.m_profileItemsService.GetProfileItem(user.ProfileID, profileContract.ProfileItemId, EquipOptions.FilterByTags);
				if (profileItem != null)
				{
					data2.Contracts[user.ProfileID] = new SessionContracts.SessionInfo(profileItem, profileContract);
				}
				else if (this.m_itemsStats.IsItemAvailableForUser(profileContract.ContractName, user))
				{
					Log.Warning<string, ulong, ulong>("Contract {0} in progress without item {1} for ProfileId {2}", profileContract.ContractName, profileContract.ProfileItemId, user.ProfileID);
				}
			}
			this.m_rewardMultiplierService.GetResultMultiplier(user.ProfileID).ContinueWith(delegate(Task<SRewardMultiplier> task)
			{
				SessionRewardMultiplier.ProfileRewardMultiplier profileRewardMultiplier = new SessionRewardMultiplier.ProfileRewardMultiplier
				{
					Multiplier = task.Result
				};
				SessionRewardMultiplier data4 = this.m_sessionStorage.GetData<SessionRewardMultiplier>(sessionId, ESessionData.RewardMultiplier);
				if (data4 != null)
				{
					data4.Multiplier[user.ProfileID] = profileRewardMultiplier;
				}
				SessionContracts data5 = this.m_sessionStorage.GetData<SessionContracts>(sessionId, ESessionData.Contracts);
				if (data5 != null && data5.Contracts.ContainsKey(user.ProfileID))
				{
					data5.Contracts[user.ProfileID].Multiplier = profileRewardMultiplier.Multiplier;
				}
			});
			ProfilePerformanceInfo profilePerformance = this.m_performanceService.GetProfilePerformance(user.ProfileID);
			ProfilePerformanceSessionStorage data3 = this.m_sessionStorage.GetData<ProfilePerformanceSessionStorage>(sessionId, ESessionData.ProfilePerformanceInfo);
			if (data3 == null)
			{
				Log.Warning(string.Format("Can't find profile progression info in sessionStorage, session: {0}, user: {1}", sessionId, user));
				return false;
			}
			data3.ProfilePerformanceInfos[user.ProfileID] = profilePerformance;
			return true;
		}

		// Token: 0x04000FC6 RID: 4038
		private readonly IBoosterService m_boosterService;

		// Token: 0x04000FC7 RID: 4039
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000FC8 RID: 4040
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000FC9 RID: 4041
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000FCA RID: 4042
		private readonly IContractService m_contractService;

		// Token: 0x04000FCB RID: 4043
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x04000FCC RID: 4044
		private readonly IServerInfo m_serverInfo;

		// Token: 0x04000FCD RID: 4045
		private readonly IRewardMultiplierService m_rewardMultiplierService;

		// Token: 0x04000FCE RID: 4046
		private readonly IItemStats m_itemsStats;

		// Token: 0x04000FCF RID: 4047
		private readonly IMissionPerformanceService m_performanceService;
	}
}
