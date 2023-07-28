using System;

namespace MasterServer.DAL
{
	// Token: 0x02000045 RID: 69
	public interface INotificationSystem
	{
		// Token: 0x060000AA RID: 170
		DALResultMulti<SPendingNotification> GetPendingNotifications(ulong profile_id);

		// Token: 0x060000AB RID: 171
		DALResult<ulong> AddPendingNotification(SPendingNotification pendingNotification);

		// Token: 0x060000AC RID: 172
		DALResultVoid DeletePendingNotification(ulong id);

		// Token: 0x060000AD RID: 173
		DALResultVoid DeleteAllPendingByConfirmationType(ulong profile_id, uint confirmation_type);

		// Token: 0x060000AE RID: 174
		DALResultMulti<SPendingNotification> ClearExpiredNotifications();

		// Token: 0x060000AF RID: 175
		DALResultVoid DeleteNotificationsForProfile(ulong profileId);
	}
}
