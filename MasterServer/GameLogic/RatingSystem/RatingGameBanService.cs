using System;
using System.Collections.Generic;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.DAL.RatingSystem;
using MasterServer.Database;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PunishmentSystem;

namespace MasterServer.GameLogic.RatingSystem
{
	// Token: 0x020000C3 RID: 195
	[Service]
	[Singleton]
	internal class RatingGameBanService : ServiceModule, IRatingGameBanService
	{
		// Token: 0x06000321 RID: 801 RVA: 0x0000EAB9 File Offset: 0x0000CEB9
		public RatingGameBanService(IDALService dalService, INotificationService notificationService, ILogService logService, IConfigProvider<RatingGameBanConfig> ratingGameBanConfigProvider)
		{
			this.m_dalService = dalService;
			this.m_notificationService = notificationService;
			this.m_logService = logService;
			this.m_ratingGameBanConfigProvider = ratingGameBanConfigProvider;
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0000EADE File Offset: 0x0000CEDE
		private TimeSpan DefaultBanTimeout
		{
			get
			{
				return this.m_config.BanTimeout;
			}
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000EAEB File Offset: 0x0000CEEB
		public override void Start()
		{
			base.Start();
			this.m_config = this.m_ratingGameBanConfigProvider.Get();
			this.m_ratingGameBanConfigProvider.Changed += this.RatingGameBanConfigProviderOnChanged;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000EB1B File Offset: 0x0000CF1B
		public override void Stop()
		{
			this.m_ratingGameBanConfigProvider.Changed -= this.RatingGameBanConfigProviderOnChanged;
			base.Stop();
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000EB3A File Offset: 0x0000CF3A
		public void BanRatingGameForPlayers(IEnumerable<ulong> profileIds)
		{
			this.BanRatingGameForPlayers(profileIds, this.DefaultBanTimeout, string.Empty);
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000EB50 File Offset: 0x0000CF50
		public void BanRatingGameForPlayers(IEnumerable<ulong> profileIds, TimeSpan banTimeout, string msg = "")
		{
			if (!this.m_config.BanEnabled)
			{
				return;
			}
			foreach (ulong num in profileIds)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(num);
				if (profileInfo.Id != num)
				{
					Log.Warning<ulong>("Profile with {0} id doesn't exist. Trying to ban unexisted player!", num);
				}
				else
				{
					TimeSpan timeSpan = (!(banTimeout == TimeSpan.Zero)) ? banTimeout : this.DefaultBanTimeout;
					RatingGamePlayerBanInfo playerBanInfo = this.GetPlayerBanInfo(num);
					if (playerBanInfo.UnbanTime < DateTime.UtcNow + timeSpan)
					{
						this.m_dalService.RatingRoomBanSystem.BanRatingGameForPlayer(num, timeSpan);
						this.NotifyOfRatingGameBan(num, timeSpan, msg);
						this.m_logService.Event.CharacterBanLog(profileInfo.UserID, num, DateTime.UtcNow.AddSeconds(timeSpan.TotalSeconds), BanReportSource.RatingGameBan);
					}
				}
			}
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000EC68 File Offset: 0x0000D068
		public void UnbanRatingGameForPlayers(IEnumerable<ulong> profileIds)
		{
			if (!this.m_config.BanEnabled)
			{
				return;
			}
			foreach (ulong num in profileIds)
			{
				if (this.m_dalService.ProfileSystem.GetProfileInfo(num).Id != num)
				{
					Log.Warning<ulong>("Profile with {0} id doesn't exist. Trying to unban unexisted player!", num);
				}
				else
				{
					this.m_dalService.RatingRoomBanSystem.UnbanRatingGameForPlayer(num);
					this.NotifyOfRatingGameBan(num, default(TimeSpan), string.Empty);
				}
			}
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0000ED1C File Offset: 0x0000D11C
		public bool IsPlayerBanned(ulong profileId)
		{
			if (!this.m_config.BanEnabled)
			{
				return false;
			}
			RatingGamePlayerBanInfo playerBanInfo = this.GetPlayerBanInfo(profileId);
			return DateTime.UtcNow < playerBanInfo.UnbanTime;
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0000ED53 File Offset: 0x0000D153
		public RatingGamePlayerBanInfo GetPlayerBanInfo(ulong profileId)
		{
			if (!this.m_config.BanEnabled)
			{
				return new RatingGamePlayerBanInfo();
			}
			return this.m_dalService.RatingRoomBanSystem.GetPlayerBanInfo(profileId) ?? new RatingGamePlayerBanInfo();
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0000ED88 File Offset: 0x0000D188
		private void NotifyOfRatingGameBan(ulong profileId, TimeSpan banTimeout, string msg = "")
		{
			RatingGameBanPlayerNotification data = new RatingGameBanPlayerNotification(banTimeout, msg);
			this.m_notificationService.AddNotification<RatingGameBanPlayerNotification>(profileId, ENotificationType.RatingGameBan, data, this.m_config.BanTimeout, EDeliveryType.SendNow, EConfirmationType.None);
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000EDBD File Offset: 0x0000D1BD
		private void RatingGameBanConfigProviderOnChanged(RatingGameBanConfig ratingGameBanConfig)
		{
			Interlocked.Exchange<RatingGameBanConfig>(ref this.m_config, ratingGameBanConfig);
		}

		// Token: 0x04000155 RID: 341
		private readonly IDALService m_dalService;

		// Token: 0x04000156 RID: 342
		private readonly INotificationService m_notificationService;

		// Token: 0x04000157 RID: 343
		private readonly ILogService m_logService;

		// Token: 0x04000158 RID: 344
		private readonly IConfigProvider<RatingGameBanConfig> m_ratingGameBanConfigProvider;

		// Token: 0x04000159 RID: 345
		private RatingGameBanConfig m_config;
	}
}
