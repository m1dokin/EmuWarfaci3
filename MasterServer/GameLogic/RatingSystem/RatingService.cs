using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL.RatingSystem;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x02000417 RID: 1047
	[Service]
	[Singleton]
	internal class RatingService : ServiceModule, IRatingService, IDebugRatingService, IRewardProcessor
	{
		// Token: 0x06001695 RID: 5781 RVA: 0x0005E606 File Offset: 0x0005CA06
		public RatingService(IDALService dalService, IConfigProvider<RatingConfig> ratingConfigProvider, ILogService logService, IUserProxyRepository userProxy)
		{
			this.m_dalService = dalService;
			this.m_ratingConfigProvider = ratingConfigProvider;
			this.m_logService = logService;
			this.m_userProxy = userProxy;
		}

		// Token: 0x1400004A RID: 74
		// (add) Token: 0x06001696 RID: 5782 RVA: 0x0005E62C File Offset: 0x0005CA2C
		// (remove) Token: 0x06001697 RID: 5783 RVA: 0x0005E664 File Offset: 0x0005CA64
		public event Action<ulong, ulong, Rating, Rating, string, ILogGroup> ProfileRatingChanged;

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x06001698 RID: 5784 RVA: 0x0005E69C File Offset: 0x0005CA9C
		public uint MaxRatingLevel
		{
			get
			{
				RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
				return ratingConfig.MaxRatingLevel;
			}
		}

		// Token: 0x06001699 RID: 5785 RVA: 0x0005E6BB File Offset: 0x0005CABB
		public RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			if (state != RewardProcessorState.Process)
			{
				return aggRewardData;
			}
			if (aggRewardData.ratingReward != 0)
			{
				this.AddRatingPoints(aggRewardData.profileId, aggRewardData.ratingReward, aggRewardData.ratingWinsToAdd, aggRewardData.sessionId, logGroup);
			}
			return aggRewardData;
		}

		// Token: 0x0600169A RID: 5786 RVA: 0x0005E6F8 File Offset: 0x0005CAF8
		public Rating GetRating(ulong profileId)
		{
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			RatingInfo ratingInfo = this.m_dalService.RatingSystem.GetRating(profileId);
			if (ratingInfo.RatingPoints > ratingConfig.MaxRatingPoints)
			{
				int ratingPoints = (int)(ratingConfig.MaxRatingPoints - ratingInfo.RatingPoints);
				ratingInfo = this.m_dalService.RatingSystem.AddRatingPoints(profileId, ratingPoints, 0, ratingInfo.SeasonId);
			}
			return this.GetRating(ratingInfo);
		}

		// Token: 0x0600169B RID: 5787 RVA: 0x0005E768 File Offset: 0x0005CB68
		public void SetRatingPoints(ulong profileId, uint ratingPoints)
		{
			Rating rating = this.GetRating(profileId);
			int ratingPointsToAdd = (int)(ratingPoints - rating.Points);
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				this.AddRatingPoints(profileId, ratingPointsToAdd, 0, string.Empty, logGroup);
			}
		}

		// Token: 0x0600169C RID: 5788 RVA: 0x0005E7C4 File Offset: 0x0005CBC4
		public void SetRatingWinStreak(ulong profileId, uint winStreak)
		{
			Rating rating = this.GetRating(profileId);
			int winsToAdd = (int)(winStreak - rating.WinStreak);
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				this.AddRatingPoints(profileId, 0, winsToAdd, string.Empty, logGroup);
			}
		}

		// Token: 0x0600169D RID: 5789 RVA: 0x0005E820 File Offset: 0x0005CC20
		public void ResetRating(ulong profileId, string seasonId)
		{
			Rating rating = this.GetRating(profileId);
			int ratingPoints = (int)(-(int)rating.Points);
			int wins = (int)(-(int)rating.WinStreak);
			this.m_dalService.RatingSystem.AddRatingPoints(profileId, ratingPoints, wins, seasonId);
		}

		// Token: 0x0600169E RID: 5790 RVA: 0x0005E85C File Offset: 0x0005CC5C
		public void AddRatingPoints(ulong profileId, int ratingPointsToAdd, int winsToAdd, string sessionId)
		{
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				this.AddRatingPoints(profileId, ratingPointsToAdd, winsToAdd, sessionId, logGroup);
			}
		}

		// Token: 0x0600169F RID: 5791 RVA: 0x0005E8A4 File Offset: 0x0005CCA4
		private void AddRatingPoints(ulong profileId, int ratingPointsToAdd, int winsToAdd, string sessionId, ILogGroup logGroup)
		{
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			Rating rating = this.GetRating(profileId);
			string seasonId = rating.SeasonId;
			int num = this.ClampRatingPoints((int)rating.Points, ratingPointsToAdd, ratingConfig.MaxRatingPoints);
			if (num == 0 && winsToAdd == 0)
			{
				return;
			}
			RatingInfo ratingInfo = this.m_dalService.RatingSystem.AddRatingPoints(profileId, num, winsToAdd, seasonId);
			Rating rating2 = this.GetRating(ratingInfo);
			ulong userID = this.m_userProxy.GetUserOrProxyByProfileId(profileId).UserID;
			this.ProfileRatingChanged.SafeInvoke(userID, profileId, rating, rating2, sessionId, logGroup);
		}

		// Token: 0x060016A0 RID: 5792 RVA: 0x0005E938 File Offset: 0x0005CD38
		private int ClampRatingPoints(int currentRatingPoints, int ratingPointsToAdd, uint maxPointsAvailable)
		{
			int min = -currentRatingPoints;
			int max = (int)(maxPointsAvailable - (uint)currentRatingPoints);
			return Utils.Clamp<int>(ratingPointsToAdd, min, max);
		}

		// Token: 0x060016A1 RID: 5793 RVA: 0x0005E958 File Offset: 0x0005CD58
		private Rating GetRating(RatingInfo ratingInfo)
		{
			RatingConfig ratingConfig = this.m_ratingConfigProvider.Get();
			RatingLevelConfig ratingLevelConfigByPoints = ratingConfig.GetRatingLevelConfigByPoints(ratingInfo.RatingPoints);
			return new Rating(ratingLevelConfigByPoints.Level, ratingInfo.RatingPoints, ratingInfo.WinStreak, ratingInfo.SeasonId);
		}

		// Token: 0x04000AEF RID: 2799
		private readonly IDALService m_dalService;

		// Token: 0x04000AF0 RID: 2800
		private readonly IConfigProvider<RatingConfig> m_ratingConfigProvider;

		// Token: 0x04000AF1 RID: 2801
		private readonly IUserProxyRepository m_userProxy;

		// Token: 0x04000AF2 RID: 2802
		private readonly ILogService m_logService;
	}
}
