using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000011 RID: 17
	internal static class DebugRoomHelpers
	{
		// Token: 0x06000048 RID: 72 RVA: 0x000051C4 File Offset: 0x000035C4
		public static ulong CreateRoom(IDebugGameRoomActivator roomActivator, IMissionSystem missionSystem, IProfileProgressionService profileProgression, IUserRepository userRepository, IRankSystem rankSystem, DebugCreateRoomsCmdParams param)
		{
			Random random = new Random((int)DateTime.Now.Ticks);
			int startId = random.Next();
			Func<MissionContextBase, bool> predicate;
			if (!string.IsNullOrEmpty(param.Mission))
			{
				predicate = ((MissionContextBase m) => m.name.Contains(param.Mission) || m.uid.Equals(param.Mission, StringComparison.OrdinalIgnoreCase));
			}
			else
			{
				predicate = ((MissionContextBase m) => m.gameMode.Equals(param.GameMode, StringComparison.OrdinalIgnoreCase));
			}
			MissionContextBase missionContextBase = missionSystem.GetMatchmakingMissions().FirstOrDefault(predicate);
			if (missionContextBase == null)
			{
				throw new InvalidOperationException(string.Format("Failed to get mission context. Requested {0}", (!string.IsNullOrEmpty(param.Mission)) ? ("name/id: " + param.Mission) : ("game type: " + param.GameMode)));
			}
			GameRoomType gameRoomType = param.RoomType;
			if (missionContextBase.gameMode == "pve" && !gameRoomType.IsPveMode())
			{
				gameRoomType = GameRoomType.PvE;
			}
			IGameRoom gameRoom = roomActivator.OpenRoomWithFakePlayers(gameRoomType, missionContextBase.uid, startId, param, profileProgression);
			if (gameRoom != null)
			{
				List<RoomPlayer> addedPlayers = new List<RoomPlayer>();
				gameRoom.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					addedPlayers = r.Players.ToList<RoomPlayer>();
				});
				foreach (RoomPlayer roomPlayer in addedPlayers)
				{
					UserInfo.User user = userRepository.MakeFake(roomPlayer.UserID, roomPlayer.ProfileID, roomPlayer.OnlineID, roomPlayer.Rank, rankSystem.GetExperience(roomPlayer.Rank), param.RegionId);
					using (UserLoginContext userLoginContext = new UserLoginContext(userRepository, user, ELoginType.Ordinary, DateTime.Now))
					{
						userLoginContext.Commit();
					}
				}
				return gameRoom.ID;
			}
			return 0UL;
		}
	}
}
