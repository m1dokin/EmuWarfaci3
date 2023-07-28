using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.GameLogic.GameModes;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000452 RID: 1106
	[Service]
	[Singleton]
	internal class PvPAutoTeamBalanceLogic : AutoTeamBalanceLogic
	{
		// Token: 0x06001771 RID: 6001 RVA: 0x000617DC File Offset: 0x0005FBDC
		public PvPAutoTeamBalanceLogic(IGameModesSystem gameModesSystem) : base(gameModesSystem)
		{
		}

		// Token: 0x06001772 RID: 6002 RVA: 0x000617E8 File Offset: 0x0005FBE8
		protected override void PutGroupsIntoTeams(IGameRoom room, Dictionary<int, TeamInfo> teamInfo, IEnumerable<RoomPlayer> players)
		{
			TeamInfo firstTeam = teamInfo[1];
			TeamInfo secondTeam = teamInfo[2];
			bool flag = base.UseGroups(room);
			if (flag)
			{
				foreach (RoomPlayer player in players)
				{
					this.AssignToTeamWithSameGroup(player, firstTeam);
					this.AssignToTeamWithSameGroup(player, secondTeam);
				}
			}
			IEnumerable<RoomPlayer> players2 = from x in players
			where !firstTeam.HasPlayer(x) && !secondTeam.HasPlayer(x)
			select x;
			List<GroupInfo> source = base.CreateGroups(flag, players2).ToList<GroupInfo>();
			IOrderedEnumerable<GroupInfo> source2 = from x in source
			orderby x.Count descending, x.Skill descending
			select x;
			foreach (GroupInfo groupInfo2 in from groupInfo in source2
			where !groupInfo.HasTeam
			select groupInfo)
			{
				this.AssignGroupToTeam(firstTeam, secondTeam, groupInfo2);
			}
		}

		// Token: 0x06001773 RID: 6003 RVA: 0x00061964 File Offset: 0x0005FD64
		private void AssignToTeamWithSameGroup(RoomPlayer player, TeamInfo team)
		{
			if (player.HasGroup && team.HasGroup(player.GroupID) && team.HasFreeSlots())
			{
				team.AddPlayer(player);
			}
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x00061994 File Offset: 0x0005FD94
		private void AssignGroupToTeam(TeamInfo firstTeam, TeamInfo secondTeam, GroupInfo groupInfo)
		{
			TeamInfo teamInfo;
			if (firstTeam.PlayersCount == secondTeam.PlayersCount)
			{
				double num = firstTeam.TeamSkill + groupInfo.Skill;
				double num2 = secondTeam.TeamSkill + groupInfo.Skill;
				teamInfo = ((Math.Abs(firstTeam.TeamSkill - num2) < Math.Abs(secondTeam.TeamSkill - num)) ? secondTeam : firstTeam);
			}
			else
			{
				teamInfo = ((firstTeam.PlayersCount <= secondTeam.PlayersCount) ? firstTeam : secondTeam);
			}
			if (teamInfo.HasSpaceForGroup(groupInfo))
			{
				teamInfo.AddGroup(groupInfo);
			}
		}
	}
}
