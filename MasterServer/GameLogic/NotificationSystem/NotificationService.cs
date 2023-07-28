using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.DAL.Exceptions;
using MasterServer.Database;
using MasterServer.GameLogic.Achievements;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200057C RID: 1404
	[Service]
	[Singleton]
	internal class NotificationService : ServiceModule, INotificationService, IDebugNotificationService
	{
		// Token: 0x06001E2B RID: 7723 RVA: 0x0007A064 File Offset: 0x00078464
		public NotificationService(IDALService dalService, IAchievementSystem achievementSystem, ISessionInfoService sessionInfoService, IQueryManager queryManager, IMemcachedService memcachedService)
		{
			this.m_dalService = dalService;
			this.m_achievementSystem = achievementSystem;
			this.m_sessionInfoService = sessionInfoService;
			this.m_queryManager = queryManager;
			this.m_memcachedService = memcachedService;
			this.m_notificationSerializers = new Dictionary<ENotificationType, INotificationSerializer>();
			foreach (KeyValuePair<NotificationSerializerAttributes, Type> keyValuePair in ReflectionUtils.GetTypesByAttribute<NotificationSerializerAttributes>(Assembly.GetExecutingAssembly()))
			{
				NotificationSerializerAttributes key = keyValuePair.Key;
				INotificationSerializer value = (INotificationSerializer)Activator.CreateInstance(keyValuePair.Value);
				this.m_notificationSerializers.Add(key.NotificationType, value);
			}
			ConfigSection section = Resources.ModuleSettings.GetSection("Notifications");
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_invitationLimit = uint.Parse(section.Get("NotificationLimit"));
			if (Resources.DBUpdaterPermission)
			{
				int num = int.Parse(section.Get("NotificationsExpirationTimeout"));
				this.m_expirationTimer = new SafeTimer(new TimerCallback(this.OnExpirationTick), null, (long)num, (long)num);
			}
		}

		// Token: 0x14000070 RID: 112
		// (add) Token: 0x06001E2C RID: 7724 RVA: 0x0007A1A0 File Offset: 0x000785A0
		// (remove) Token: 0x06001E2D RID: 7725 RVA: 0x0007A1D8 File Offset: 0x000785D8
		public event NotificationConfirmed OnNotificationConfirmed;

		// Token: 0x14000071 RID: 113
		// (add) Token: 0x06001E2E RID: 7726 RVA: 0x0007A210 File Offset: 0x00078610
		// (remove) Token: 0x06001E2F RID: 7727 RVA: 0x0007A248 File Offset: 0x00078648
		public event NotificationExpired OnNotificationExpired;

		// Token: 0x06001E30 RID: 7728 RVA: 0x0007A280 File Offset: 0x00078680
		public override void Stop()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Notifications");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06001E31 RID: 7729 RVA: 0x0007A2B8 File Offset: 0x000786B8
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (Resources.DBUpdaterPermission && string.Equals(args.Name, "NotificationsExpirationTimeout", StringComparison.CurrentCultureIgnoreCase))
			{
				int iValue = args.iValue;
				this.m_expirationTimer = new SafeTimer(new TimerCallback(this.OnExpirationTick), null, (long)iValue, (long)iValue);
			}
		}

		// Token: 0x06001E32 RID: 7730 RVA: 0x0007A308 File Offset: 0x00078708
		private bool IsAchievementNotificationValid(SNotification notification, Dictionary<uint, AchievementUpdateChunk> achievements)
		{
			try
			{
				INotificationSerializer notificationSerializer = this.GetNotificationSerializer(notification.Type);
				if (!achievements.ContainsKey(((AchievementUpdateChunk)notificationSerializer.Deserialize(notification.Data)).achievementId))
				{
					Log.Warning<ulong, Dictionary<uint, AchievementUpdateChunk>>("Trying to send notification {0}, {1} with incorrect achievement", notification.ID, achievements);
					return false;
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
			return true;
		}

		// Token: 0x06001E33 RID: 7731 RVA: 0x0007A384 File Offset: 0x00078784
		public IEnumerable<SNotification> GetPendingByType(ulong profileId, ENotificationType type)
		{
			List<SNotification> list = new List<SNotification>();
			DateTime now = DateTime.UtcNow;
			IEnumerable<SNotification> enumerable = from x in this.m_dalService.NotificationSystem.GetPendingNotifications(profileId)
			where (x.Type & (uint)type) == x.Type && x.ExpirationTimeUTC > now
			select new SNotification(x);
			int num = 0;
			Lazy<Dictionary<uint, AchievementUpdateChunk>> lazy = new Lazy<Dictionary<uint, AchievementUpdateChunk>>(() => this.m_achievementSystem.GetCurrentProfileAchievements(profileId));
			foreach (SNotification snotification in enumerable)
			{
				if (!snotification.Type.IsAchievement() || this.IsAchievementNotificationValid(snotification, lazy.Value))
				{
					if (!snotification.Type.IsInvite() || (long)(++num) <= (long)((ulong)this.m_invitationLimit))
					{
						list.Add(snotification);
					}
				}
			}
			return list;
		}

		// Token: 0x06001E34 RID: 7732 RVA: 0x0007A4C0 File Offset: 0x000788C0
		private bool TryGetPendingNotification(ulong profileId, ulong notificationId, out SNotification notification)
		{
			notification = null;
			SNotification snotification = (from x in this.m_dalService.NotificationSystem.GetPendingNotifications(profileId)
			where x.ID == notificationId
			select new SNotification(x)).FirstOrDefault<SNotification>();
			if (snotification == null)
			{
				return false;
			}
			if (snotification.Type.IsAchievement())
			{
				Dictionary<uint, AchievementUpdateChunk> currentProfileAchievements = this.m_achievementSystem.GetCurrentProfileAchievements(profileId);
				if (!this.IsAchievementNotificationValid(snotification, currentProfileAchievements))
				{
					return false;
				}
			}
			if (snotification.IsExpired && !snotification.Type.IsInvite())
			{
				return false;
			}
			notification = snotification;
			return true;
		}

		// Token: 0x06001E35 RID: 7733 RVA: 0x0007A57C File Offset: 0x0007897C
		private ENotificationType GetInviteNotificationType()
		{
			return (ENotificationType)80U;
		}

		// Token: 0x06001E36 RID: 7734 RVA: 0x0007A580 File Offset: 0x00078980
		public IEnumerable<SNotification> PopPending(ulong profileId)
		{
			SNotification[] array = this.GetPendingByType(profileId, (ENotificationType)4294967295U).ToArray<SNotification>();
			if (array.Any((SNotification n) => n.ConfirmationType == EConfirmationType.None))
			{
				this.m_dalService.NotificationSystem.DeleteAllPendingByConfirmationType(profileId, 1U);
			}
			return array;
		}

		// Token: 0x06001E37 RID: 7735 RVA: 0x0007A5D8 File Offset: 0x000789D8
		public void DeletePendingByType(ulong profileId, ENotificationType type)
		{
			IEnumerable<SNotification> pendingByType = this.GetPendingByType(profileId, type);
			foreach (SNotification snotification in pendingByType)
			{
				this.m_dalService.NotificationSystem.DeletePendingNotification(profileId, snotification.ID);
			}
		}

		// Token: 0x06001E38 RID: 7736 RVA: 0x0007A648 File Offset: 0x00078A48
		public Task AddNotification<T>(ulong profileId, ENotificationType type, T data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation)
		{
			List<T> data2 = new List<T>(1)
			{
				data
			};
			return this.AddNotifications<T>(profileId, type, data2, expiration, delivery, confirmation);
		}

		// Token: 0x06001E39 RID: 7737 RVA: 0x0007A674 File Offset: 0x00078A74
		public void AddBroadcastNotifications<T>(List<string> recievers, ENotificationType type, IEnumerable<T> data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation)
		{
			IEnumerable<byte[]> data2 = data.Select(new Func<T, byte[]>(Utils.CreateByteArrayFromType<T>));
			this.SendBroadcastNotfication(recievers, type, data2, expiration, delivery, confirmation);
		}

		// Token: 0x06001E3A RID: 7738 RVA: 0x0007A6A4 File Offset: 0x00078AA4
		public Task AddNotifications<T>(ulong profileId, ENotificationType type, IEnumerable<T> data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation)
		{
			List<SNotification> notifications = (from d in data
			select NotificationFactory.CreateNotification<T>(type, d, expiration, confirmation)).ToList<SNotification>();
			return this.AddNotifications(profileId, notifications, delivery);
		}

		// Token: 0x06001E3B RID: 7739 RVA: 0x0007A6F0 File Offset: 0x00078AF0
		public Task AddNotifications(ulong profileId, IEnumerable<SNotification> notifications, EDeliveryType delivery)
		{
			if (delivery == EDeliveryType.SendOnCheckPoint)
			{
				foreach (SNotification notification in notifications)
				{
					this.SaveNotificationToDB(profileId, notification);
				}
			}
			else
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
				if (ServicesManager.ExecutionPhase >= ExecutionPhase.Started)
				{
					return this.m_sessionInfoService.GetProfileInfoAsync(profileInfo.Nickname).ContinueWith(delegate(Task<ProfileInfo> t)
					{
						if (!string.IsNullOrEmpty(t.Result.OnlineID))
						{
							foreach (SNotification snotification in notifications)
							{
								if (snotification.ConfirmationType == EConfirmationType.Confirmation)
								{
									this.SaveNotificationToDB(profileId, snotification);
								}
							}
							IEnumerable<SNotification> enumerable = notifications.Except(from x in notifications
							where x.ConfirmationType == EConfirmationType.Confirmation && x.ID == 0UL
							select x);
							this.m_queryManager.Request("sync_notifications", t.Result.OnlineID, new object[]
							{
								enumerable
							});
						}
						else if (delivery != EDeliveryType.SendNow)
						{
							this.AddNotifications(profileId, notifications, EDeliveryType.SendOnCheckPoint);
						}
						else
						{
							Log.Info<ulong>("Notification wasn't send to user {0}", profileId);
						}
					});
				}
				if (delivery != EDeliveryType.SendNow)
				{
					return this.AddNotifications(profileId, notifications, EDeliveryType.SendOnCheckPoint);
				}
				Log.Info<ulong>("Notification wasn't send to user {0}", profileId);
			}
			return TaskHelpers.Completed();
		}

		// Token: 0x06001E3C RID: 7740 RVA: 0x0007A7FC File Offset: 0x00078BFC
		private void SaveNotificationToDB(ulong profileId, SNotification notification)
		{
			try
			{
				SPendingNotification pendingNotification = this.CreatePendingNotification(profileId, notification);
				if (notification.Type.IsInvite())
				{
					IEnumerable<SNotification> pendingByType = this.GetPendingByType(profileId, this.GetInviteNotificationType());
					if ((long)pendingByType.Count<SNotification>() >= (long)((ulong)this.m_invitationLimit))
					{
						return;
					}
				}
				notification.ID = this.m_dalService.NotificationSystem.AddPendingNotification(pendingNotification);
			}
			catch (DALBinaryDataLengthViolationException e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06001E3D RID: 7741 RVA: 0x0007A880 File Offset: 0x00078C80
		public void Confirm(ulong profileId, ulong notificationId, XmlNode confirmationNode)
		{
			bool flag = true;
			if (this.OnNotificationConfirmed != null)
			{
				SNotification notif;
				if (this.TryGetPendingNotification(profileId, notificationId, out notif))
				{
					this.OnNotificationConfirmed(notif, confirmationNode);
				}
				else
				{
					flag = false;
				}
			}
			if (flag)
			{
				this.DeletePending(profileId, notificationId);
			}
		}

		// Token: 0x06001E3E RID: 7742 RVA: 0x0007A8CC File Offset: 0x00078CCC
		public INotificationSerializer GetNotificationSerializer(ENotificationType notificationType)
		{
			INotificationSerializer result;
			if (!this.m_notificationSerializers.TryGetValue(notificationType, out result))
			{
				throw new NotificationServiceException(string.Format("Unsupported notification type: {0}", notificationType));
			}
			return result;
		}

		// Token: 0x06001E3F RID: 7743 RVA: 0x0007A904 File Offset: 0x00078D04
		private void SendBroadcastNotfication(List<string> recievers, ENotificationType type, IEnumerable<byte[]> data, TimeSpan expiration, EDeliveryType delivery, EConfirmationType confirmation)
		{
			if (delivery != EDeliveryType.SendNow || confirmation == EConfirmationType.Confirmation)
			{
				throw new NotificationServiceException("No broadcasting for SendOnCheckPoint/SendNowOrLater or Confirm notifications");
			}
			if (recievers.Count > 0)
			{
				IEnumerable<SNotification> enumerable = from d in data
				select new SNotification
				{
					Type = type,
					ConfirmationType = confirmation,
					ExpirationTimeUTC = DateTime.UtcNow + expiration,
					Data = d
				};
				this.m_queryManager.BroadcastRequest("notification_broadcast", "k01.", recievers, new object[]
				{
					enumerable
				});
			}
		}

		// Token: 0x06001E40 RID: 7744 RVA: 0x0007A98C File Offset: 0x00078D8C
		private SPendingNotification CreatePendingNotification(ulong profileId, SNotification notification)
		{
			return new SPendingNotification
			{
				ProfileId = profileId,
				ConfirmationType = (uint)notification.ConfirmationType,
				Data = notification.Data,
				Message = notification.Message,
				ExpirationTimeUTC = notification.ExpirationTimeUTC,
				Type = (uint)notification.Type
			};
		}

		// Token: 0x06001E41 RID: 7745 RVA: 0x0007A9EB File Offset: 0x00078DEB
		private void DeletePending(ulong profileId, ulong notificationId)
		{
			this.m_dalService.NotificationSystem.DeletePendingNotification(profileId, notificationId);
		}

		// Token: 0x06001E42 RID: 7746 RVA: 0x0007AA00 File Offset: 0x00078E00
		private void OnExpirationTick(object dummy)
		{
			object updateLock = this.m_updateLock;
			lock (updateLock)
			{
				if (this.m_jobInProgress)
				{
					Log.Warning("Clear expired notification job already in progress, please increase timeout interval");
					return;
				}
				this.m_jobInProgress = true;
			}
			try
			{
				this.ScheduledMaintanance();
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			finally
			{
				object updateLock2 = this.m_updateLock;
				lock (updateLock2)
				{
					this.m_jobInProgress = false;
				}
			}
		}

		// Token: 0x06001E43 RID: 7747 RVA: 0x0007AAC4 File Offset: 0x00078EC4
		private void ScheduledMaintanance()
		{
			IEnumerable<SPendingNotification> enumerable = this.m_dalService.NotificationSystem.ClearExpiredNotifications();
			if (this.m_memcachedService != null && this.m_memcachedService.Connected)
			{
				List<ulong> list = new List<ulong>();
				foreach (SPendingNotification spendingNotification in enumerable)
				{
					if (!list.Contains(spendingNotification.ProfileId))
					{
						list.Add(spendingNotification.ProfileId);
						this.m_memcachedService.Remove(cache_domains.profile[spendingNotification.ProfileId].notifications);
					}
				}
			}
			if (this.OnNotificationExpired != null)
			{
				foreach (SPendingNotification notification in enumerable)
				{
					this.OnNotificationExpired(notification);
				}
			}
		}

		// Token: 0x06001E44 RID: 7748 RVA: 0x0007ABE8 File Offset: 0x00078FE8
		public void DeleteNotificationsForProfile(ulong profileId)
		{
			this.m_dalService.NotificationSystem.DeleteNotificationsForProfile(profileId);
		}

		// Token: 0x04000EB1 RID: 3761
		private const ulong DefaultNotificationID = 0UL;

		// Token: 0x04000EB2 RID: 3762
		public static readonly TimeSpan DefaultNotificationTTL = TimeSpan.FromDays(1.0);

		// Token: 0x04000EB3 RID: 3763
		private readonly object m_updateLock = new object();

		// Token: 0x04000EB4 RID: 3764
		private bool m_jobInProgress;

		// Token: 0x04000EB5 RID: 3765
		private SafeTimer m_expirationTimer;

		// Token: 0x04000EB6 RID: 3766
		private uint m_invitationLimit;

		// Token: 0x04000EB7 RID: 3767
		private readonly IDALService m_dalService;

		// Token: 0x04000EB8 RID: 3768
		private readonly IAchievementSystem m_achievementSystem;

		// Token: 0x04000EB9 RID: 3769
		private readonly ISessionInfoService m_sessionInfoService;

		// Token: 0x04000EBA RID: 3770
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000EBB RID: 3771
		private readonly IMemcachedService m_memcachedService;

		// Token: 0x04000EBC RID: 3772
		private readonly Dictionary<ENotificationType, INotificationSerializer> m_notificationSerializers;
	}
}
