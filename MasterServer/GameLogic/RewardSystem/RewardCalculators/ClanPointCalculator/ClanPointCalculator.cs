using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem.RewardCalculators.ClanPointCalculator
{
	// Token: 0x020000E1 RID: 225
	[Service]
	internal class ClanPointCalculator : IRewardCalculatorElement
	{
		// Token: 0x060003B2 RID: 946 RVA: 0x0001039B File Offset: 0x0000E79B
		public ClanPointCalculator(IClanPointCalculatorConfigProvider clanPointCalculatorConfiguration, IClanService clanService)
		{
			this.m_clanPointCalculatorConfigProvider = clanPointCalculatorConfiguration;
			this.m_clanService = clanService;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x000103B1 File Offset: 0x0000E7B1
		public static uint CalculateClanPoints(uint gainedExp, float roomTypeMultiplier, float groupSizeMultiplier)
		{
			return (uint)(gainedExp * roomTypeMultiplier * groupSizeMultiplier);
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000103BC File Offset: 0x0000E7BC
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			int groupSize = this.GetGroupSize(player, inputData);
			float roomTypeMultiplier = this.m_clanPointCalculatorConfigProvider.GetRoomTypeMultiplier(inputData.roomType);
			float groupSizeMultiplier = this.m_clanPointCalculatorConfigProvider.GetGroupSizeMultiplier(inputData.roomType, groupSize);
			outputData.gainedClanPoints = ClanPointCalculator.CalculateClanPoints(outputData.gainedExp, roomTypeMultiplier, groupSizeMultiplier);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("After session {0} player with profileId: {1} gained clanPoints: {2}", sessionId, player.profileId, outputData.gainedClanPoints);
			stringBuilder.AppendLine();
			stringBuilder.AppendFormat("Group size was: {0}, GameModeMultiplier: {1}, GroupSizeMultiplier: {2}", groupSize, roomTypeMultiplier, groupSizeMultiplier);
			Log.Verbose(Log.Group.SessionResults, stringBuilder.ToString(), new object[0]);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x00010474 File Offset: 0x0000E874
		private int GetGroupSize(RewardInputData.Team.Player player, RewardInputData data)
		{
			ClanInfo playerClanInfo = this.m_clanService.GetClanInfoByPid(player.profileId);
			RewardInputData.Team team;
			if (playerClanInfo == null || !data.teams.TryGetValue(player.teamId, out team))
			{
				return 1;
			}
			List<RewardInputData.Team.Player> source = (from p in team.playerScores
			where p.groupId == player.groupId
			select p).ToList<RewardInputData.Team.Player>();
			IEnumerable<RewardInputData.Team.Player> source2 = source.Where(delegate(RewardInputData.Team.Player c)
			{
				ClanInfo clanInfoByPid = this.m_clanService.GetClanInfoByPid(c.profileId);
				return clanInfoByPid != null && clanInfoByPid.ClanID == playerClanInfo.ClanID;
			});
			return (!source2.Any<RewardInputData.Team.Player>()) ? 1 : source2.Count<RewardInputData.Team.Player>();
		}

		// Token: 0x04000186 RID: 390
		private const int DefaultGroupSize = 1;

		// Token: 0x04000187 RID: 391
		private readonly IClanPointCalculatorConfigProvider m_clanPointCalculatorConfigProvider;

		// Token: 0x04000188 RID: 392
		private readonly IClanService m_clanService;
	}
}
