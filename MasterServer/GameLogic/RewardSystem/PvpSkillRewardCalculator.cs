using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoom.RoomExtensions;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x0200041D RID: 1053
	[Service]
	internal class PvpSkillRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060016B1 RID: 5809 RVA: 0x0005F0DD File Offset: 0x0005D4DD
		public PvpSkillRewardCalculator(ISessionStorage sessionStorage, ISkillConfigProvider skillConfigProvider, ISkillService skillService)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_skillConfigProvider = skillConfigProvider;
			this.m_skillService = skillService;
		}

		// Token: 0x060016B2 RID: 5810 RVA: 0x0005F0FC File Offset: 0x0005D4FC
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			bool flag = inputData.winnerTeamId == (int)player.teamId;
			Skill skill = this.m_skillService.GetSkill(player.profileId, SkillType.Pvp);
			if ((flag && skill.CurveCoef - skill.Value < 5E-324) || (!flag && skill.Value < 5E-324) || this.IsDraw(inputData) || this.IsFreeForAllMode(player) || !this.AreBothTeamsNotEmpty(sessionId))
			{
				outputData.skillDiff = 0.0;
				return;
			}
			SkillConfig skillConfig = this.m_skillConfigProvider.GetSkillConfig(SkillType.Pvp);
			double num = this.CalculateSkillPointsReward(sessionId, inputData, skillConfig);
			num = Utils.Clamp<double>(num, skillConfig.MinPointsToAdd, skillConfig.MaxPointsToAdd);
			double num2 = (!flag) ? -1.0 : 1.0;
			outputData.skillDiff = num2 * num;
			outputData.skillType = SkillType.Pvp;
		}

		// Token: 0x060016B3 RID: 5811 RVA: 0x0005F1F9 File Offset: 0x0005D5F9
		private bool IsDraw(RewardInputData inputData)
		{
			return inputData.winnerTeamId == 0 || inputData.winnerTeamId == -1;
		}

		// Token: 0x060016B4 RID: 5812 RVA: 0x0005F212 File Offset: 0x0005D612
		private bool IsFreeForAllMode(RewardInputData.Team.Player player)
		{
			return player.teamId == 0;
		}

		// Token: 0x060016B5 RID: 5813 RVA: 0x0005F220 File Offset: 0x0005D620
		private bool AreBothTeamsNotEmpty(string sessionId)
		{
			PlayersInTeamPlayTime data = this.m_sessionStorage.GetData<PlayersInTeamPlayTime>(sessionId, ESessionData.PlayTime);
			return data.GetPlayers(1).Any<PlayerPlayTime>() && data.GetPlayers(2).Any<PlayerPlayTime>();
		}

		// Token: 0x060016B6 RID: 5814 RVA: 0x0005F25C File Offset: 0x0005D65C
		private double CalculateSkillPointsReward(string sessionId, RewardInputData inputData, SkillConfig config)
		{
			double num = this.CalculateTeamSkillDiff(sessionId, inputData);
			if (Math.Abs(num) < 5E-324)
			{
				return 0.0;
			}
			return config.ExponentialCoefficient * Math.Exp(-num);
		}

		// Token: 0x060016B7 RID: 5815 RVA: 0x0005F2A0 File Offset: 0x0005D6A0
		private double CalculateTeamSkillDiff(string sessionId, RewardInputData inputData)
		{
			int winnerTeamId = inputData.winnerTeamId;
			int teamId = (winnerTeamId != 1) ? 1 : 2;
			PlayersInTeamPlayTime data = this.m_sessionStorage.GetData<PlayersInTeamPlayTime>(sessionId, ESessionData.PlayTime);
			double num = this.CalculateTeamSkill(data, inputData, winnerTeamId);
			double num2 = this.CalculateTeamSkill(data, inputData, teamId);
			if (num + num2 < 5E-324)
			{
				return 0.0;
			}
			return (num - num2) / Math.Max(num, num2);
		}

		// Token: 0x060016B8 RID: 5816 RVA: 0x0005F314 File Offset: 0x0005D714
		private double CalculateTeamSkill(PlayersInTeamPlayTime teamsPlaytime, RewardInputData inputData, int teamId)
		{
			IDictionary<ulong, RewardInputData.Team.Player> reportedPlayTime = this.GetReportedPlayTime(inputData, teamId);
			IEnumerable<PlayerPlayTime> players = teamsPlaytime.GetPlayers(teamId);
			return this.CalculateTeamSkill(players, reportedPlayTime, inputData.sessionTime);
		}

		// Token: 0x060016B9 RID: 5817 RVA: 0x0005F340 File Offset: 0x0005D740
		private IDictionary<ulong, RewardInputData.Team.Player> GetReportedPlayTime(RewardInputData inputData, int teamId)
		{
			RewardInputData.Team team;
			Dictionary<ulong, RewardInputData.Team.Player> result;
			if (inputData.teams.TryGetValue((byte)teamId, out team))
			{
				List<RewardInputData.Team.Player> playerScores = team.playerScores;
				result = playerScores.ToDictionary((RewardInputData.Team.Player k) => k.profileId, (RewardInputData.Team.Player v) => v);
			}
			else
			{
				result = new Dictionary<ulong, RewardInputData.Team.Player>();
			}
			return result;
		}

		// Token: 0x060016BA RID: 5818 RVA: 0x0005F3B8 File Offset: 0x0005D7B8
		private double CalculateTeamSkill(IEnumerable<PlayerPlayTime> accumulatedPlayersPlayTime, IDictionary<ulong, RewardInputData.Team.Player> reportedPlayTime, float sessionTime)
		{
			double num = 0.0;
			foreach (PlayerPlayTime playerPlayTime in accumulatedPlayersPlayTime)
			{
				RewardInputData.Team.Player player;
				double num2;
				if (reportedPlayTime.TryGetValue(playerPlayTime.ProfileId, out player))
				{
					num2 = player.sessionTime.TotalSeconds;
					if (!player.inSessionFromStart)
					{
						num2 += playerPlayTime.GetPlayTime();
					}
				}
				else
				{
					num2 = playerPlayTime.GetPlayTime();
				}
				num2 = ((num2 <= (double)sessionTime) ? num2 : ((double)sessionTime));
				num += playerPlayTime.Skill * num2 / (double)sessionTime;
			}
			return num * num;
		}

		// Token: 0x04000AF6 RID: 2806
		private const SkillType CALCULATOR_SKILL_TYPE = SkillType.Pvp;

		// Token: 0x04000AF7 RID: 2807
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000AF8 RID: 2808
		private readonly ISkillConfigProvider m_skillConfigProvider;

		// Token: 0x04000AF9 RID: 2809
		private readonly ISkillService m_skillService;
	}
}
