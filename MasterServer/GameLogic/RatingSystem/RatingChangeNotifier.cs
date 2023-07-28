using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000EF RID: 239
	[Service]
	[Singleton]
	internal class RatingChangeNotifier : ServiceModule, IRatingChangeNotifier
	{
		// Token: 0x060003E4 RID: 996 RVA: 0x00010F29 File Offset: 0x0000F329
		public RatingChangeNotifier(IRatingService ratingService)
		{
			this.m_ratingService = ratingService;
		}

		// Token: 0x060003E5 RID: 997 RVA: 0x00010F38 File Offset: 0x0000F338
		public override void Init()
		{
			this.m_ratingService.ProfileRatingChanged += this.NotifyRatingChanged;
		}

		// Token: 0x060003E6 RID: 998 RVA: 0x00010F51 File Offset: 0x0000F351
		public override void Stop()
		{
			this.m_ratingService.ProfileRatingChanged -= this.NotifyRatingChanged;
		}

		// Token: 0x060003E7 RID: 999 RVA: 0x00010F6C File Offset: 0x0000F36C
		private void NotifyRatingChanged(ulong userId, ulong profileId, Rating oldRating, Rating newRating, string sessionId, ILogGroup logGroup)
		{
			if (logGroup != null)
			{
				logGroup.RatingChangedLog(userId, profileId, newRating.SeasonId, sessionId, oldRating.Points, oldRating.Level, newRating.Points, newRating.Level, newRating.WinStreak);
			}
		}

		// Token: 0x040001A0 RID: 416
		private readonly IRatingService m_ratingService;
	}
}
