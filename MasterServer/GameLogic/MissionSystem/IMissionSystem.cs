using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000790 RID: 1936
	[Contract]
	internal interface IMissionSystem
	{
		// Token: 0x170003B6 RID: 950
		// (get) Token: 0x06002810 RID: 10256
		MissionGenerator MissionGenerator { get; }

		// Token: 0x170003B7 RID: 951
		// (get) Token: 0x06002811 RID: 10257
		MissionGraphRepository MissionGraphRepository { get; }

		// Token: 0x170003B8 RID: 952
		// (get) Token: 0x06002812 RID: 10258
		SubMissionConfigRepository SubMissionConfigRepository { get; }

		// Token: 0x170003B9 RID: 953
		// (get) Token: 0x06002813 RID: 10259
		ObjectivesRepository ObjectivesRepository { get; }

		// Token: 0x06002814 RID: 10260
		void AddMission(string key, string data);

		// Token: 0x06002815 RID: 10261
		MissionContext GetMission(string key);

		// Token: 0x06002816 RID: 10262
		List<MissionContext> MatchMission(string key);

		// Token: 0x06002817 RID: 10263
		List<MissionContextBase> GetMatchmakingMissions();

		// Token: 0x06002818 RID: 10264
		bool IsMissionExpired(string key);

		// Token: 0x06002819 RID: 10265
		bool IsUserMission(string key);

		// Token: 0x0600281A RID: 10266
		void ResetUserMissions();

		// Token: 0x0600281B RID: 10267
		void Dump();

		// Token: 0x0600281C RID: 10268
		void Dump(string key);
	}
}
