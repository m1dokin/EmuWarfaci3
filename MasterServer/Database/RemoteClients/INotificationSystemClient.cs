using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000205 RID: 517
	internal interface INotificationSystemClient
	{
		// Token: 0x06000AEC RID: 2796
		IEnumerable<SPendingNotification> GetPendingNotifications(ulong profileId);

		// Token: 0x06000AED RID: 2797
		ulong AddPendingNotification(SPendingNotification pendingNotification);

		// Token: 0x06000AEE RID: 2798
		void DeletePendingNotification(ulong profileId, ulong id);

		// Token: 0x06000AEF RID: 2799
		void DeleteAllPendingByConfirmationType(ulong profileId, uint confirmationType);

		// Token: 0x06000AF0 RID: 2800
		IEnumerable<SPendingNotification> ClearExpiredNotifications();

		// Token: 0x06000AF1 RID: 2801
		void DeleteNotificationsForProfile(ulong profileId);
	}
}
