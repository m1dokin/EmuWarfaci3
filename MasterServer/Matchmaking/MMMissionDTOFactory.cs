using System;
using HK2Net;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000514 RID: 1300
	[Service]
	[Singleton]
	internal class MMMissionDTOFactory : IMMMissionDTOFactory
	{
		// Token: 0x06001C32 RID: 7218 RVA: 0x00071A11 File Offset: 0x0006FE11
		public MMMissionDTOFactory(IGameModesSystem gameModesSystem, IMissionSystem missionSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x06001C33 RID: 7219 RVA: 0x00071A28 File Offset: 0x0006FE28
		public MMMissionDTO Create(MissionContextBase mission, GameRoomType roomType)
		{
			GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(mission);
			bool flag;
			gameModeSetting.GetSetting(ERoomSetting.NO_TEAMS_MODE, out flag);
			int maxPlayers = 16;
			MissionContext mission2 = this.m_missionSystem.GetMission(mission.uid);
			this.m_gameModesSystem.GetDefaultValue<int>(mission2, roomType, ERoomRestriction.MAX_PLAYERS, ref maxPlayers);
			GameModeSetting gameModeSetting2 = this.m_gameModesSystem.GetGameModeSetting(mission2);
			int minPlayersToJoin;
			gameModeSetting2.GetSetting(roomType, ERoomSetting.MIN_PLAYERS_FOR_ROOM_JOINING, out minPlayersToJoin);
			int minPlayersToCreate;
			gameModeSetting2.GetSetting(roomType, ERoomSetting.MIN_PLAYERS_FOR_ROOM_CREATION, out minPlayersToCreate);
			string text;
			gameModeSetting2.GetSetting(roomType, ERoomSetting.CLASS_PATTERN, out text);
			return new MMMissionDTO(mission.uid, !flag, minPlayersToJoin, minPlayersToCreate, maxPlayers, (text ?? string.Empty).ToCharArray());
		}

		// Token: 0x04000D7B RID: 3451
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x04000D7C RID: 3452
		private readonly IMissionSystem m_missionSystem;
	}
}
