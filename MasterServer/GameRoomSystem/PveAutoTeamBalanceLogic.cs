using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameLogic.GameModes;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000451 RID: 1105
	[Service]
	[Singleton]
	internal class PveAutoTeamBalanceLogic : AutoTeamBalanceLogic
	{
		// Token: 0x0600176F RID: 5999 RVA: 0x00061776 File Offset: 0x0005FB76
		public PveAutoTeamBalanceLogic(IGameModesSystem gameModesSystem) : base(gameModesSystem)
		{
		}

		// Token: 0x06001770 RID: 6000 RVA: 0x00061780 File Offset: 0x0005FB80
		protected override void PutGroupsIntoTeams(IGameRoom room, Dictionary<int, TeamInfo> teamInfo, IEnumerable<RoomPlayer> players)
		{
			TeamInfo teamInfo2 = teamInfo[1];
			foreach (RoomPlayer roomPlayer in players)
			{
				teamInfo2.AddPlayer(roomPlayer);
			}
		}
	}
}
