using System;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoom.Commands.Debug;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004B8 RID: 1208
	[Service]
	[Singleton]
	internal class GameRoomActivator : ServiceModule, IGameRoomActivator, IDebugGameRoomActivator
	{
		// Token: 0x060019BB RID: 6587 RVA: 0x00068CB0 File Offset: 0x000670B0
		public GameRoomActivator(IGameRoomManagerRegistry roomManagerReg, IRoomExtensionFactory factory, IRankSystem rankSystem, IUserRepository userRepository, IMissionSystem missionSystem, IClientVersionsManagementService clientVersionsManagementService, IMissionAccessLimitationService limitationService, IRoomPlayerFactory roomPlayerFactory, ISkillService skillService)
		{
			this.m_rankSystem = rankSystem;
			this.m_userRepository = userRepository;
			this.m_factory = factory;
			this.m_roomManagerReg = roomManagerReg;
			this.m_missionSystem = missionSystem;
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_limitationService = limitationService;
			this.m_roomPlayerFactory = roomPlayerFactory;
			this.m_skillService = skillService;
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x00068D08 File Offset: 0x00067108
		public IGameRoom OpenRoom(GameRoomType type, Action<IGameRoom> setup)
		{
			return this.OpenRoom(type, RoomReference.EmptyReference, setup);
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x00068D18 File Offset: 0x00067118
		public IGameRoom OpenRoomByRoomRef(RoomReference roomRef, CreateRoomParam roomParam)
		{
			if (this.m_roomManagerReg.ContainsRoom(roomRef))
			{
				throw new GameRoomManagerException(string.Format("Room with same room ref is already created {0}", roomRef));
			}
			MissionContext mission = this.m_missionSystem.GetMission(roomParam.Mission);
			if (mission == null)
			{
				throw new GameRoomManagerException(string.Format("Can't find mission with key {0}", roomParam.Mission));
			}
			if (!Resources.ChannelTypes.IsPvP(Resources.Channel))
			{
				throw new GameRoomManagerException(string.Format("Room type '{0}' is invalid for current channel for room opening.", GameRoomType.PvP_AutoStart));
			}
			if (mission.IsPveMode())
			{
				throw new GameRoomManagerException(string.Format("Not PvP mission {0} isn't allowed for room {1}", roomParam.Mission, roomRef.Reference));
			}
			return this.OpenRoom(GameRoomType.PvP_AutoStart, roomRef, delegate(IGameRoom r)
			{
				MissionExtension extension = r.GetExtension<MissionExtension>();
				extension.SetMission(roomParam.Mission);
				CoreState state = r.GetState<CoreState>(AccessMode.ReadWrite);
				state.AllowManualJoin = roomParam.AllowJoin;
				AutoStart state2 = r.GetState<AutoStart>(AccessMode.ReadWrite);
				state2.CanManualStart = roomParam.ManualStart;
				CustomParams state3 = r.GetState<CustomParams>(AccessMode.ReadWrite);
				state3.HighLatencyAutoKick = true;
				state3.LockedSpectatorCamera = roomParam.LockedSpectatorCamera;
				state3.PreRoundTime = roomParam.PreRoundTime;
				state3.RoundLimit = roomParam.RoundLimit;
				state3.FriendlyFire = true;
				state3.DeadCanChat = false;
				state3.EnemyOutlines = false;
				state3.Autobalance = false;
				state3.OvertimeMode = roomParam.OvertimeMode;
			});
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x00068DF4 File Offset: 0x000671F4
		private IGameRoom OpenRoom(GameRoomType type, RoomReference roomReference, Action<IGameRoom> setup)
		{
			bool discard = false;
			RoomExtensionsData roomExtensions = this.m_factory.GetRoomExtensions(type);
			ulong num = (ulong)((long)Interlocked.Increment(ref this.m_lastId));
			int hashCode = Resources.ServerName.GetHashCode();
			int hashCode2 = DateTime.UtcNow.GetHashCode();
			ulong num2 = (ulong)(0.5 * (double)(hashCode + hashCode2) * (double)(hashCode + hashCode2 + 1) + (double)hashCode2);
			ulong id = (num2 << 32) + num;
			GameRoom room = new GameRoom(this.m_roomManagerReg, this.m_rankSystem, this.m_userRepository, this.m_clientVersionsManagementService, this.m_limitationService, this.m_roomPlayerFactory, this.m_skillService, roomExtensions, id, roomReference, type);
			room.RoomClosed += this.OnRoomClosed;
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				try
				{
					setup(r);
				}
				catch (DiscardRoomException)
				{
					discard = true;
				}
			}, delegate(IGameRoom committed)
			{
				if (!discard && !this.m_roomManagerReg.RegisterRoom(room))
				{
					discard = true;
				}
			});
			if (discard)
			{
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					r.Close();
				});
				return null;
			}
			return room;
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x00068F2F File Offset: 0x0006732F
		private void OnRoomClosed(IGameRoom room)
		{
			this.m_roomManagerReg.UnregisterRoom(room);
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x00068F40 File Offset: 0x00067340
		public IGameRoom OpenRoomWithFakePlayers(GameRoomType roomType, string missionId, int startId, DebugRoomPlayersParams debugPlayerParams, IProfileProgressionService profileProgressionService)
		{
			Action<IGameRoom> setup = delegate(IGameRoom r)
			{
				r.RoomName = "room_" + Guid.NewGuid();
				r.Private = false;
				MissionExtension extension = r.GetExtension<MissionExtension>();
				if (extension.SetMission(missionId) != GameRoomRetCode.OK)
				{
					Log.Warning<string>("Failed to create room with mission '{0}', discarding", missionId);
					throw new DiscardRoomException();
				}
				SkillType skillTypeByRoomType = SkillTypeHelper.GetSkillTypeByRoomType(r.Type);
				r.AddFakePlayers(debugPlayerParams, startId, profileProgressionService, skillTypeByRoomType);
			};
			return this.OpenRoom(roomType, setup);
		}

		// Token: 0x060019C1 RID: 6593 RVA: 0x00068F86 File Offset: 0x00067386
		public void CloseRooms(params IGameRoom[] rooms)
		{
			rooms.SafeForEach(delegate(IGameRoom room)
			{
				try
				{
					room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.RemoveFakePlayers((from p in r.Players
						select p.ProfileID).ToList<ulong>());
						r.Close();
					});
					room = null;
				}
				catch (RoomClosedException)
				{
				}
				finally
				{
					if (room != null)
					{
						this.m_roomManagerReg.UnregisterRoom(room);
					}
				}
			});
		}

		// Token: 0x04000C4E RID: 3150
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000C4F RID: 3151
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000C50 RID: 3152
		private readonly IRoomExtensionFactory m_factory;

		// Token: 0x04000C51 RID: 3153
		private readonly IGameRoomManagerRegistry m_roomManagerReg;

		// Token: 0x04000C52 RID: 3154
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000C53 RID: 3155
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x04000C54 RID: 3156
		private readonly IMissionAccessLimitationService m_limitationService;

		// Token: 0x04000C55 RID: 3157
		private readonly IRoomPlayerFactory m_roomPlayerFactory;

		// Token: 0x04000C56 RID: 3158
		private readonly ISkillService m_skillService;

		// Token: 0x04000C57 RID: 3159
		private int m_lastId;
	}
}
