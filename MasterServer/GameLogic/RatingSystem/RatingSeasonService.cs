using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.DAL.RatingSystem;
using MasterServer.Database;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason;
using Util.Common;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000E0 RID: 224
	[Service]
	[Singleton]
	internal class RatingSeasonService : IRatingSeasonService, IDebugRatingSeasonService
	{
		// Token: 0x060003A5 RID: 933 RVA: 0x00010018 File Offset: 0x0000E418
		public RatingSeasonService(IDALService dalService, IRatingService ratingService, IConfigProvider<RatingSeasonConfig> ratingSeasonConfigProvider)
		{
			this.m_dalService = dalService;
			this.m_ratingService = ratingService;
			this.m_ratingSeasonConfigProvider = ratingSeasonConfigProvider;
		}

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x060003A6 RID: 934 RVA: 0x00010038 File Offset: 0x0000E438
		// (remove) Token: 0x060003A7 RID: 935 RVA: 0x00010070 File Offset: 0x0000E470
		public event Action<ulong, Rating> ProfileRatingReseted;

		// Token: 0x060003A8 RID: 936 RVA: 0x000100A8 File Offset: 0x0000E4A8
		public RatingSeason GetRatingSeason()
		{
			RatingSeasonConfig ratingSeasonConfig = this.m_ratingSeasonConfigProvider.Get();
			if (ratingSeasonConfig.Enabled)
			{
				RatingSeasonInfo ratingSeasonInfo = this.m_dalService.RatingSystem.GetRatingSeasonInfo();
				return new RatingSeason(ratingSeasonInfo);
			}
			return RatingSeason.DisabledSeason;
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x000100EC File Offset: 0x0000E4EC
		public void SetupSeason(RatingSeasonRuleConfig ratingSeasonConfig, bool seasonActive)
		{
			if (Resources.DBUpdaterPermission)
			{
				RatingSeasonInfo ratingSeasonInfo = this.m_dalService.RatingSystem.GetRatingSeasonInfo();
				string seasonId = ratingSeasonInfo.SeasonId;
				if (this.IsNewSeasonConfigured(ratingSeasonConfig, ratingSeasonInfo))
				{
					string arg = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm");
					seasonId = string.Format("{0}_{1}", ratingSeasonConfig.SeasonIdTemplate, arg);
				}
				this.UpdateSeason(seasonId, ratingSeasonConfig.AnnouncementEndDate, ratingSeasonConfig.GamesEndDate, seasonActive);
			}
		}

		// Token: 0x060003AA RID: 938 RVA: 0x00010165 File Offset: 0x0000E565
		public void SetupSeason(string seasonId, DateTime announcementEndDate, DateTime gamesEndDate, bool seasonActive)
		{
			this.UpdateSeason(seasonId, announcementEndDate, gamesEndDate, seasonActive);
		}

		// Token: 0x060003AB RID: 939 RVA: 0x00010174 File Offset: 0x0000E574
		public Rating GetPlayerRating(ulong profileId)
		{
			Rating rating = this.m_ratingService.GetRating(profileId);
			RatingSeason ratingSeason = this.GetRatingSeason();
			string seasonId = ratingSeason.SeasonId;
			return (!(rating.SeasonId != seasonId)) ? rating : new Rating(0U, 0U, 0U, seasonId);
		}

		// Token: 0x060003AC RID: 940 RVA: 0x000101BC File Offset: 0x0000E5BC
		public bool SetPlayerRatingPoints(ulong profileId, uint ratingPointsToSet)
		{
			Rating rating = this.m_ratingService.GetRating(profileId);
			RatingSeason ratingSeason = this.GetRatingSeason();
			if (rating.SeasonId != ratingSeason.SeasonId)
			{
				return false;
			}
			this.m_ratingService.SetRatingPoints(profileId, ratingPointsToSet);
			return true;
		}

		// Token: 0x060003AD RID: 941 RVA: 0x00010204 File Offset: 0x0000E604
		public void UpdateSeasonForPlayer(ulong profileId)
		{
			Rating rating = this.m_ratingService.GetRating(profileId);
			RatingSeason ratingSeason = this.GetRatingSeason();
			string seasonId = ratingSeason.SeasonId;
			if (!ratingSeason.IsActive || rating.SeasonId == seasonId)
			{
				return;
			}
			this.m_ratingService.ResetRating(profileId, seasonId);
			rating = this.m_ratingService.GetRating(profileId);
			this.ProfileRatingReseted.SafeInvoke(profileId, rating);
		}

		// Token: 0x060003AE RID: 942 RVA: 0x00010270 File Offset: 0x0000E670
		public IEnumerable<ulong> GetTopRatingPlayers(uint playersCount)
		{
			RatingSeason ratingSeason = this.GetRatingSeason();
			string seasonId = ratingSeason.SeasonId;
			IEnumerable<RatingInfo> topRatingPlayers = this.m_dalService.RatingSystem.GetTopRatingPlayers(seasonId, playersCount);
			return (from p in topRatingPlayers
			select p.ProfileId).ToList<ulong>();
		}

		// Token: 0x060003AF RID: 943 RVA: 0x000102C8 File Offset: 0x0000E6C8
		private void UpdateSeason(string seasonId, DateTime announcementEndDate, DateTime gamesEndDate, bool seasonActive)
		{
			string announcementEndDate2 = announcementEndDate.ToString("yyyy-MM-ddTHH:mm");
			string gamesEndDate2 = gamesEndDate.ToString("yyyy-MM-ddTHH:mm");
			this.m_dalService.RatingSystem.UpdateSeason(seasonId, announcementEndDate2, gamesEndDate2, seasonActive);
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x00010304 File Offset: 0x0000E704
		private bool IsNewSeasonConfigured(RatingSeasonRuleConfig seasonConfig, RatingSeasonInfo currentSeasonStatus)
		{
			bool flag = RatingSeasonRangeHelper.IsActive(currentSeasonStatus.AnnouncementEndDate, currentSeasonStatus.GamesEndDate);
			bool flag2 = RatingSeasonRangeHelper.IsFinished(currentSeasonStatus.GamesEndDate);
			bool flag3 = flag || flag2;
			bool flag4 = RatingSeasonRangeHelper.IsActive(seasonConfig.AnnouncementEndDate, seasonConfig.GamesEndDate);
			bool flag5 = RatingSeasonRangeHelper.IsAnnouncement(seasonConfig.AnnouncementEndDate);
			bool flag6 = flag4 || flag5;
			bool flag7 = flag3 && flag5;
			bool flag8 = flag2 && flag6;
			return flag7 || flag8;
		}

		// Token: 0x04000181 RID: 385
		private readonly IDALService m_dalService;

		// Token: 0x04000182 RID: 386
		private readonly IRatingService m_ratingService;

		// Token: 0x04000183 RID: 387
		private readonly IConfigProvider<RatingSeasonConfig> m_ratingSeasonConfigProvider;
	}
}
