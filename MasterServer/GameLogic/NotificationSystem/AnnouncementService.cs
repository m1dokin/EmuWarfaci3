using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.Users;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000587 RID: 1415
	[Service]
	[Singleton]
	internal class AnnouncementService : ServiceModule, IAnnouncementService
	{
		// Token: 0x06001E62 RID: 7778 RVA: 0x0007B367 File Offset: 0x00079767
		public AnnouncementService(IDALService dalService, INotificationService notificationService, IOnlineClient onlineClient, IQueryManager queryManager, IUserRepository userRepository)
		{
			this.m_dalService = dalService;
			this.m_notificationService = notificationService;
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
			this.m_userRepository = userRepository;
		}

		// Token: 0x06001E63 RID: 7779 RVA: 0x0007B39F File Offset: 0x0007979F
		public override void Start()
		{
			base.Start();
			this.ScheduledUpdate();
		}

		// Token: 0x06001E64 RID: 7780 RVA: 0x0007B3AD File Offset: 0x000797AD
		public override void Stop()
		{
			if (this.m_timer != null)
			{
				this.m_timer.Dispose();
				this.m_timer = null;
			}
			base.Stop();
		}

		// Token: 0x06001E65 RID: 7781 RVA: 0x0007B3D2 File Offset: 0x000797D2
		public IEnumerable<Announcement> GetAnnouncements()
		{
			return this.m_dalService.AnnouncementSystem.GetAnnouncements();
		}

		// Token: 0x06001E66 RID: 7782 RVA: 0x0007B3E4 File Offset: 0x000797E4
		public IEnumerable<Announcement> GetAnnouncementsToSend()
		{
			List<Announcement> list = new List<Announcement>();
			foreach (Announcement announcement in this.m_dalService.AnnouncementSystem.GetActiveAnnouncements())
			{
				if (announcement.ReadyToSend() && announcement.IsServerSupported(Resources.ServerName) && announcement.IsChannelSupported(Resources.Channel.ToString()))
				{
					list.Add(announcement);
				}
			}
			return list;
		}

		// Token: 0x06001E67 RID: 7783 RVA: 0x0007B488 File Offset: 0x00079888
		public void Add(Announcement announcement)
		{
			announcement.ID = this.m_dalService.AnnouncementSystem.Add(announcement);
			if (announcement.ID == 0UL)
			{
				throw new ApplicationException(string.Format("Announcement wasn't added: {0}", announcement));
			}
			this.SendUpdateQuery();
		}

		// Token: 0x06001E68 RID: 7784 RVA: 0x0007B4C8 File Offset: 0x000798C8
		public bool Remove(ulong id)
		{
			bool result = false;
			Announcement announcement;
			if (this.GetAnnouncement(id, out announcement))
			{
				result = true;
				bool flag = announcement.ReadyToSend();
				this.m_dalService.AnnouncementSystem.Remove(id);
				this.SendUpdateQuery((!flag) ? 0UL : id);
				if (flag)
				{
					this.SendDeleteQuery(id);
				}
			}
			return result;
		}

		// Token: 0x06001E69 RID: 7785 RVA: 0x0007B521 File Offset: 0x00079921
		public bool GetAnnouncement(ulong id, out Announcement announcement)
		{
			announcement = this.m_dalService.AnnouncementSystem.GetAnnouncementById(id);
			return announcement != null;
		}

		// Token: 0x06001E6A RID: 7786 RVA: 0x0007B540 File Offset: 0x00079940
		public void ModifyAnnouncement(Announcement announcement)
		{
			Announcement announcement2;
			if (this.GetAnnouncement(announcement.ID, out announcement2))
			{
				string channel = Resources.Channel.ToString();
				string serverName = Resources.ServerName;
				bool flag = (announcement2.ReadyToSend() && !announcement.ReadyToSend()) || (announcement2.IsServerSupported(serverName) && !announcement.IsServerSupported(serverName)) || (announcement2.IsChannelSupported(channel) && !announcement.IsChannelSupported(channel)) || announcement2.Target != announcement.Target;
				this.m_dalService.AnnouncementSystem.Modify(announcement);
				if (flag)
				{
					this.SendDeleteQuery(announcement.ID);
				}
				this.SendUpdateQuery((!flag) ? 0UL : announcement.ID);
			}
		}

		// Token: 0x06001E6B RID: 7787 RVA: 0x0007B616 File Offset: 0x00079A16
		public void UpdateCache(ulong deleteId)
		{
			if (deleteId > 0UL)
			{
				this.SendDeleteQuery(deleteId);
			}
			this.ScheduledUpdate();
		}

		// Token: 0x06001E6C RID: 7788 RVA: 0x0007B62D File Offset: 0x00079A2D
		private void SendUpdateQuery()
		{
			this.SendUpdateQuery(0UL);
		}

		// Token: 0x06001E6D RID: 7789 RVA: 0x0007B638 File Offset: 0x00079A38
		private void SendUpdateQuery(ulong deleted_announcement)
		{
			this.m_queryManager.Request("master_server_bcast", this.m_onlineClient.TargetRoute, new object[]
			{
				"announcement",
				deleted_announcement.ToString(),
				"no_self_send"
			});
			this.ScheduledUpdate();
		}

		// Token: 0x06001E6E RID: 7790 RVA: 0x0007B68C File Offset: 0x00079A8C
		private void SendDeleteQuery(ulong deleted_announcement)
		{
			List<string> recievers = (from u in this.m_userRepository.GetUsersWithoutTouch()
			select u.OnlineID).ToList<string>();
			QueryManager.BroadcastRequestSt("announcement_deleted", "k01.", recievers, new object[]
			{
				deleted_announcement
			});
		}

		// Token: 0x06001E6F RID: 7791 RVA: 0x0007B6EC File Offset: 0x00079AEC
		private void SetTimer()
		{
			DateTime utcNow = DateTime.UtcNow;
			DateTime dateTime = DateTime.MaxValue;
			foreach (Announcement announcement in this.m_dalService.AnnouncementSystem.GetActiveAnnouncements())
			{
				if (announcement.StartTimeUTC >= utcNow && dateTime > announcement.StartTimeUTC)
				{
					dateTime = announcement.StartTimeUTC;
				}
			}
			if (dateTime < DateTime.MaxValue)
			{
				long dueTimeMs = (long)(dateTime - utcNow).TotalMilliseconds;
				if (this.m_timer != null)
				{
					this.m_timer.Change(dueTimeMs, 0L);
				}
				else
				{
					this.m_timer = new SafeTimer(new TimerCallback(this.OnUpdateTick), null, dueTimeMs, 0L);
				}
			}
		}

		// Token: 0x06001E70 RID: 7792 RVA: 0x0007B7DC File Offset: 0x00079BDC
		private void ScheduledUpdate()
		{
			List<Announcement> list = new List<Announcement>();
			foreach (Announcement announcement in this.GetAnnouncementsToSend())
			{
				if (announcement.Target == 0UL)
				{
					list.Add(announcement);
				}
				else if (this.m_userRepository.IsOnline(announcement.Target))
				{
					this.m_notificationService.AddNotification<Announcement>(announcement.Target, ENotificationType.Announcement, announcement, TimeSpan.Zero, EDeliveryType.SendNow, EConfirmationType.None);
				}
			}
			if (list.Any<Announcement>())
			{
				List<string> list2 = (from u in this.m_userRepository.GetUsersWithoutTouch()
				select u.OnlineID).ToList<string>();
				if (list2.Any<string>())
				{
					this.m_notificationService.AddBroadcastNotifications<Announcement>(list2, ENotificationType.Announcement, list, TimeSpan.Zero, EDeliveryType.SendNow, EConfirmationType.None);
				}
				else
				{
					Log.Info("[AnnouncementService.ScheduledUpdate] No broadcast announcements sent, no online users");
				}
			}
			this.SetTimer();
		}

		// Token: 0x06001E71 RID: 7793 RVA: 0x0007B8FC File Offset: 0x00079CFC
		private void OnUpdateTick(object dummy)
		{
			object updateLock = this.m_updateLock;
			lock (updateLock)
			{
				if (this.m_jobInProgress)
				{
					Log.Warning("Update announcements job already in progress, please increase timeout interval");
					return;
				}
				this.m_jobInProgress = true;
			}
			try
			{
				this.ScheduledUpdate();
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

		// Token: 0x04000ECA RID: 3786
		private object m_updateLock = new object();

		// Token: 0x04000ECB RID: 3787
		private bool m_jobInProgress;

		// Token: 0x04000ECC RID: 3788
		private SafeTimer m_timer;

		// Token: 0x04000ECD RID: 3789
		private readonly IDALService m_dalService;

		// Token: 0x04000ECE RID: 3790
		private readonly INotificationService m_notificationService;

		// Token: 0x04000ECF RID: 3791
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04000ED0 RID: 3792
		private readonly IQueryManager m_queryManager;

		// Token: 0x04000ED1 RID: 3793
		private readonly IUserRepository m_userRepository;
	}
}
