using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameRoom.RoomExtensions;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020000F1 RID: 241
	[Service]
	internal class RatingRewardCalculator : IRewardCalculatorElement
	{
		// Token: 0x060003ED RID: 1005 RVA: 0x00011069 File Offset: 0x0000F469
		public RatingRewardCalculator(IRatingService ratingService, IConfigProvider<RatingConfig> ratingConfigProvider, IConfigProvider<RatingWinStreakConfig> ratingWinStreakConfigProvider, ISessionStorage sessionStorage)
		{
			this.m_ratingService = ratingService;
			this.m_ratingConfigProvider = ratingConfigProvider;
			this.m_ratingWinStreakConfigProvider = ratingWinStreakConfigProvider;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x060003EE RID: 1006 RVA: 0x00011090 File Offset: 0x0000F490
		public void Calculate(MissionContext context, RewardInputData inputData, RewardInputData.Team.Player player, string sessionId, ref uint reward, ref RewardOutputData outputData)
		{
			if (!inputData.leaversPunished)
			{
				this.PunishLeavers(inputData, sessionId);
				inputData.leaversPunished = true;
			}
			if (this.IsDraw(inputData))
			{
				outputData.ratingReward = 0;
				return;
			}
			Rating rating = this.m_ratingService.GetRating(player.profileId);
			int num = (inputData.winnerTeamId != (int)player.teamId) ? -1 : 1;
			uint num2 = this.CalculateWinStreakBonus(rating.Level, rating.WinStreak, num);
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			long num3 = (long)((ulong)(ratingConfig.Step + num2) * (ulong)((long)num));
			long num4 = (long)((ulong)rating.Points + (ulong)num3);
			num4 = Utils.Clamp<long>(num4, 0L, (long)((ulong)ratingConfig.MaxRatingPoints));
			RatingLevelConfig ratingLevelConfigByPoints = ratingConfig.GetRatingLevelConfigByPoints((uint)num4);
			uint num5 = this.CalculateAdjustment(rating.Level, ratingLevelConfigByPoints);
			outputData.ratingReward = Convert.ToInt32((long)((ulong)num5 + (ulong)num3));
			outputData.winStreakBonus = num2;
			outputData.ratingWinsToAdd = (int)((num <= 0) ? (-rating.WinStreak) : 1U);
		}

		// Token: 0x060003EF RID: 1007 RVA: 0x00011194 File Offset: 0x0000F594
		private uint CalculateAdjustment(uint currentRatingLevel, RatingLevelConfig newRatingLevelConfig)
		{
			if (currentRatingLevel < newRatingLevelConfig.Level)
			{
				return newRatingLevelConfig.Adjustment;
			}
			return 0U;
		}

		// Token: 0x060003F0 RID: 1008 RVA: 0x000111AC File Offset: 0x0000F5AC
		private uint CalculateWinStreakBonus(uint ratingLevel, uint currentWinStreak, int winnerMultiplier)
		{
			RatingWinStreakConfig ratingWinStreakConfig = this.m_ratingWinStreakConfigProvider.Get();
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			uint num = ratingConfig.MaxRatingLevel - ratingLevel + 1U;
			bool enabled = ratingWinStreakConfig.Enabled;
			bool flag = winnerMultiplier > 0;
			bool flag2 = num > ratingWinStreakConfig.ApplyBelowRating;
			bool flag3 = currentWinStreak + 1U >= ratingWinStreakConfig.StartFromStreak;
			if (enabled && flag && flag2 && flag3)
			{
				return ratingWinStreakConfig.BonusAmount;
			}
			return 0U;
		}

		// Token: 0x060003F1 RID: 1009 RVA: 0x00011227 File Offset: 0x0000F627
		private bool IsDraw(RewardInputData inputData)
		{
			return inputData.winnerTeamId == 0;
		}

		// Token: 0x060003F2 RID: 1010 RVA: 0x00011234 File Offset: 0x0000F634
		private void PunishLeavers(RewardInputData inputData, string sessionId)
		{
			try
			{
				RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
				List<ulong> allProfilesPlayedInSession = this.GetAllProfilesPlayedInSession(sessionId);
				List<ulong> allProfilesCompletedSession = this.GetAllProfilesCompletedSession(inputData);
				foreach (ulong num in allProfilesPlayedInSession)
				{
					if (!allProfilesCompletedSession.Contains(num))
					{
						Rating rating = this.m_ratingService.GetRating(num);
						this.m_ratingService.AddRatingPoints(num, (int)(-(int)ratingConfig.LeavePenalty), (int)(-(int)rating.WinStreak), sessionId);
					}
				}
			}
			catch (Exception innerException)
			{
				Log.Error(new ApplicationException("Exception occur during punishing rating game leavers", innerException));
			}
		}

		// Token: 0x060003F3 RID: 1011 RVA: 0x00011300 File Offset: 0x0000F700
		private List<ulong> GetAllProfilesPlayedInSession(string sessionId)
		{
			PlayersInTeamPlayTime data = this.m_sessionStorage.GetData<PlayersInTeamPlayTime>(sessionId, ESessionData.PlayTime);
			IEnumerable<ulong> first = from x in data.GetPlayers(1)
			select x.ProfileId;
			IEnumerable<ulong> second = from x in data.GetPlayers(2)
			select x.ProfileId;
			IEnumerable<ulong> source = first.Concat(second).Distinct<ulong>();
			return source.ToList<ulong>();
		}

		// Token: 0x060003F4 RID: 1012 RVA: 0x00011384 File Offset: 0x0000F784
		private List<ulong> GetAllProfilesCompletedSession(RewardInputData inputData)
		{
			IEnumerable<RewardInputData.Team.Player> source = inputData.teams.SelectMany((KeyValuePair<byte, RewardInputData.Team> x) => x.Value.playerScores);
			IEnumerable<ulong> source2 = from x in source
			select x.profileId;
			return source2.ToList<ulong>();
		}

		// Token: 0x040001A3 RID: 419
		private readonly IRatingService m_ratingService;

		// Token: 0x040001A4 RID: 420
		private readonly IConfigProvider<RatingConfig> m_ratingConfigProvider;

		// Token: 0x040001A5 RID: 421
		private readonly IConfigProvider<RatingWinStreakConfig> m_ratingWinStreakConfigProvider;

		// Token: 0x040001A6 RID: 422
		private readonly ISessionStorage m_sessionStorage;
	}
}
