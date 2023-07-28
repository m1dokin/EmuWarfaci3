using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000454 RID: 1108
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_add_players", Help = "Adds players to the room described by its id.")]
	internal class DebugAddPlayersCmd : ConsoleCommand<DebugAddPlayersCmdParams>
	{
		// Token: 0x0600177A RID: 6010 RVA: 0x00061AD7 File Offset: 0x0005FED7
		public DebugAddPlayersCmd(IGameRoomManager roomManager, IProfileProgressionService profileProgression, IUserRepository userRepository, IRankSystem rankSystem)
		{
			this.m_roomManager = roomManager;
			this.m_profileProgression = profileProgression;
			this.m_userRepository = userRepository;
			this.m_rankSystem = rankSystem;
		}

		// Token: 0x0600177B RID: 6011 RVA: 0x00061AFC File Offset: 0x0005FEFC
		protected override void Execute(DebugAddPlayersCmdParams param)
		{
			List<RoomPlayer> addedPlayers = new List<RoomPlayer>();
			Random random = new Random((int)DateTime.Now.Ticks);
			int startId = random.Next();
			IGameRoom room = this.m_roomManager.GetRoom(param.RoomId);
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				List<ulong> curentRoomProfileIds = (from x in r.Players
				select x.ProfileID).ToList<ulong>();
				SkillType skillTypeByRoomType = SkillTypeHelper.GetSkillTypeByRoomType(r.Type);
				r.AddFakePlayers(param, startId, this.m_profileProgression, skillTypeByRoomType);
				addedPlayers.AddRange(from x in r.Players
				where !curentRoomProfileIds.Contains(x.ProfileID)
				select x);
			});
			foreach (RoomPlayer roomPlayer in addedPlayers)
			{
				UserInfo.User user = this.m_userRepository.MakeFake(roomPlayer.UserID, roomPlayer.ProfileID, roomPlayer.OnlineID, roomPlayer.Rank, this.m_rankSystem.GetExperience(roomPlayer.Rank), param.RegionId);
				using (UserLoginContext userLoginContext = new UserLoginContext(this.m_userRepository, user, ELoginType.Ordinary, DateTime.Now))
				{
					userLoginContext.Commit();
				}
			}
		}

		// Token: 0x04000B4A RID: 2890
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x04000B4B RID: 2891
		private readonly IProfileProgressionService m_profileProgression;

		// Token: 0x04000B4C RID: 2892
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000B4D RID: 2893
		private readonly IRankSystem m_rankSystem;
	}
}
