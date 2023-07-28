using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002FB RID: 763
	[Contract]
	internal interface IGameModesSystem
	{
		// Token: 0x060011A1 RID: 4513
		GameModeSetting GetGameModeSetting(MissionContextBase ctx);

		// Token: 0x060011A2 RID: 4514
		GameModeSetting GetGameModeSetting(string mode);

		// Token: 0x060011A3 RID: 4515
		GameModeRestriction GetGameModeRestriction(MissionContextBase ctx);

		// Token: 0x060011A4 RID: 4516
		GameModeRestriction GetGameModeRestriction(string mode);

		// Token: 0x060011A5 RID: 4517
		RoomRestrictionDesc GetRestrictionDesc(ERoomRestriction type);

		// Token: 0x060011A6 RID: 4518
		bool IsGlobalRestriction(string kind);

		// Token: 0x060011A7 RID: 4519
		bool IsRestrictionLocked(string kind);

		// Token: 0x060011A8 RID: 4520
		bool ValidateRestriction<T>(MissionContext mission, GameRoomType type, string kind, T changedValue, ref T oldValue);

		// Token: 0x060011A9 RID: 4521
		void GetDefaultValue<T>(MissionContext mission, GameRoomType type, ERoomRestriction kind, ref T value);

		// Token: 0x060011AA RID: 4522
		IEnumerable<string> GetSupportedModes();
	}
}
