using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000206 RID: 518
	internal class NotificationSystemClient : DALCacheProxy<IDALService>, INotificationSystemClient
	{
		// Token: 0x06000AF3 RID: 2803 RVA: 0x0002903A File Offset: 0x0002743A
		internal void Reset(INotificationSystem system)
		{
			this.m_notificationSystem = system;
		}

		// Token: 0x06000AF4 RID: 2804 RVA: 0x00029044 File Offset: 0x00027444
		public IEnumerable<SPendingNotification> GetPendingNotifications(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SPendingNotification> options = new DALCacheProxy<IDALService>.Options<SPendingNotification>
			{
				cache_domain = cache_domains.profile[profileId].notifications,
				get_data_stream = (() => this.m_notificationSystem.GetPendingNotifications(profileId))
			};
			return base.GetDataStream<SPendingNotification>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AF5 RID: 2805 RVA: 0x000290AC File Offset: 0x000274AC
		public ulong AddPendingNotification(SPendingNotification pendingNotification)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.profile[pendingNotification.ProfileId].notifications,
				set_func = (() => this.m_notificationSystem.AddPendingNotification(pendingNotification))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AF6 RID: 2806 RVA: 0x00029118 File Offset: 0x00027518
		public void DeletePendingNotification(ulong profileId, ulong id)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].notifications,
				set_func = (() => this.m_notificationSystem.DeletePendingNotification(id))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AF7 RID: 2807 RVA: 0x00029178 File Offset: 0x00027578
		public void DeleteAllPendingByConfirmationType(ulong profileId, uint confirmationType)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].notifications,
				set_func = (() => this.m_notificationSystem.DeleteAllPendingByConfirmationType(profileId, confirmationType))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AF8 RID: 2808 RVA: 0x000291E4 File Offset: 0x000275E4
		public IEnumerable<SPendingNotification> ClearExpiredNotifications()
		{
			DALCacheProxy<IDALService>.Options<SPendingNotification> options = new DALCacheProxy<IDALService>.Options<SPendingNotification>
			{
				get_data_stream = (() => this.m_notificationSystem.ClearExpiredNotifications())
			};
			return base.GetDataStream<SPendingNotification>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AF9 RID: 2809 RVA: 0x00029218 File Offset: 0x00027618
		public void DeleteNotificationsForProfile(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].notifications,
				set_func = (() => this.m_notificationSystem.DeleteNotificationsForProfile(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000559 RID: 1369
		private INotificationSystem m_notificationSystem;
	}
}
