using System;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000017 RID: 23
	internal class NotificationSystem : INotificationSystem
	{
		// Token: 0x060000DB RID: 219 RVA: 0x000084AC File Offset: 0x000066AC
		public NotificationSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000084C8 File Offset: 0x000066C8
		public DALResultMulti<SPendingNotification> GetPendingNotifications(ulong profile_id)
		{
			CacheProxy.Options<SPendingNotification> options = new CacheProxy.Options<SPendingNotification>
			{
				db_serializer = this.m_notifs_serializer
			};
			options.query("CALL GetPendingNotifications(?pid)", new object[]
			{
				"?pid",
				profile_id
			});
			return this.m_dal.CacheProxy.GetStream<SPendingNotification>(options);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x0000851C File Offset: 0x0000671C
		public DALResult<ulong> AddPendingNotification(SPendingNotification pendingNotification)
		{
			this.m_dal.ValidateFixedSizeColumnData("notifications", "data", pendingNotification.Data.Length);
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT AddPendingNotification(?pid, ?type, ?conf, ?data, ?lt, ?description)", new object[]
			{
				"?pid",
				pendingNotification.ProfileId,
				"?type",
				pendingNotification.Type,
				"?conf",
				pendingNotification.ConfirmationType,
				"?data",
				pendingNotification.Data,
				"?lt",
				(uint)TimeUtils.LocalTimeToUTCTimestamp(pendingNotification.ExpirationTimeUTC),
				"?description",
				pendingNotification.Message
			});
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000860C File Offset: 0x0000680C
		public DALResultVoid DeletePendingNotification(ulong id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeletePendingNotification(?id)", new object[]
			{
				"?id",
				id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00008654 File Offset: 0x00006854
		public DALResultVoid DeleteAllPendingByConfirmationType(ulong profile_id, uint confirmation_type)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteAllPendingByConfirmationType(?pid, ?confirmation)", new object[]
			{
				"?pid",
				profile_id,
				"?confirmation",
				confirmation_type
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000086AC File Offset: 0x000068AC
		public DALResultMulti<SPendingNotification> ClearExpiredNotifications()
		{
			CacheProxy.Options<SPendingNotification> options = new CacheProxy.Options<SPendingNotification>
			{
				db_serializer = this.m_notifs_serializer
			};
			options.query("CALL ClearExpiredNotifications()", new object[0]);
			return this.m_dal.CacheProxy.GetStream<SPendingNotification>(options);
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000086F0 File Offset: 0x000068F0
		public DALResultVoid DeleteNotificationsForProfile(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteNotificationsForProfile(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000053 RID: 83
		private DAL m_dal;

		// Token: 0x04000054 RID: 84
		private PendingNotificationSerializer m_notifs_serializer = new PendingNotificationSerializer();
	}
}
