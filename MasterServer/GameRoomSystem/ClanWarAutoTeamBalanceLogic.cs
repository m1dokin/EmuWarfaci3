using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000450 RID: 1104
	[Service]
	[Singleton]
	internal class ClanWarAutoTeamBalanceLogic : AutoTeamBalanceLogic
	{
		// Token: 0x06001768 RID: 5992 RVA: 0x00061563 File Offset: 0x0005F963
		public ClanWarAutoTeamBalanceLogic(IGameModesSystem gameModesSystem) : base(gameModesSystem)
		{
		}

		// Token: 0x06001769 RID: 5993 RVA: 0x0006156C File Offset: 0x0005F96C
		protected override void PutGroupsIntoTeams(IGameRoom room, Dictionary<int, TeamInfo> teamInfo, IEnumerable<RoomPlayer> players)
		{
			ClanWar state = room.GetState<ClanWar>(AccessMode.ReadOnly);
			TeamInfo firstTeam = teamInfo[1];
			TeamInfo secondTeam = teamInfo[2];
			bool useGroups = base.UseGroups(room);
			IEnumerable<RoomPlayer> players2 = from x in players
			where !firstTeam.HasPlayer(x) && !secondTeam.HasPlayer(x)
			select x;
			List<GroupInfo> source = base.CreateGroups(useGroups, players2).ToList<GroupInfo>();
			IOrderedEnumerable<GroupInfo> source2 = from x in source
			orderby x.Count descending, x.Skill descending
			select x;
			foreach (GroupInfo groupInfo2 in from groupInfo in source2
			where !groupInfo.HasTeam
			select groupInfo)
			{
				this.AssignGroupToTeam(state, firstTeam, secondTeam, groupInfo2);
			}
		}

		// Token: 0x0600176A RID: 5994 RVA: 0x00061694 File Offset: 0x0005FA94
		private void AssignGroupToTeam(ClanWar clanWar, TeamInfo firstTeam, TeamInfo secondTeam, GroupInfo groupInfo)
		{
			TeamInfo teamInfo = this.DetermineTeamId(groupInfo, clanWar, firstTeam, secondTeam);
			if (teamInfo != null && teamInfo.HasSpaceForGroup(groupInfo))
			{
				teamInfo.AddGroup(groupInfo);
			}
		}

		// Token: 0x0600176B RID: 5995 RVA: 0x000616C8 File Offset: 0x0005FAC8
		private TeamInfo DetermineTeamId(GroupInfo groupInfo, ClanWar clanWar, TeamInfo firstTeam, TeamInfo secondTeam)
		{
			RoomPlayer roomPlayer = groupInfo.Players.First<RoomPlayer>();
			if (roomPlayer.IsInClan(clanWar.Clan1))
			{
				return firstTeam;
			}
			if (roomPlayer.IsInClan(clanWar.Clan2))
			{
				return secondTeam;
			}
			if (string.IsNullOrEmpty(clanWar.Clan1))
			{
				return firstTeam;
			}
			if (string.IsNullOrEmpty(clanWar.Clan2))
			{
				return secondTeam;
			}
			return null;
		}
	}
}
