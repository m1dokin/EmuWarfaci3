using System;
using System.Collections.Generic;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004B3 RID: 1203
	internal interface IMapRotatingMode
	{
		// Token: 0x060019A3 RID: 6563
		void OnSetMissionEnded(MissionContext mission);

		// Token: 0x1700027F RID: 639
		// (get) Token: 0x060019A4 RID: 6564
		bool IsVoteAvailable { get; }

		// Token: 0x17000280 RID: 640
		// (get) Token: 0x060019A5 RID: 6565
		IEnumerable<string> GetRotatingMaps { get; }

		// Token: 0x060019A6 RID: 6566
		bool TryCountVote(ulong profileId, string mission_uid);

		// Token: 0x060019A7 RID: 6567
		IDictionary<string, int> DumpVotingState();

		// Token: 0x060019A8 RID: 6568
		string ChooseWinner();
	}
}
