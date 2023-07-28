using System;
using HK2Net;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x0200057B RID: 1403
	[Contract]
	public interface IDebugNotificationService
	{
		// Token: 0x06001E2A RID: 7722
		void DeleteNotificationsForProfile(ulong profileId);
	}
}
