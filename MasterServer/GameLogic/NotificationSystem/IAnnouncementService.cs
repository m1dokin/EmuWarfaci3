using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000586 RID: 1414
	[Contract]
	public interface IAnnouncementService
	{
		// Token: 0x06001E5B RID: 7771
		IEnumerable<Announcement> GetAnnouncements();

		// Token: 0x06001E5C RID: 7772
		IEnumerable<Announcement> GetAnnouncementsToSend();

		// Token: 0x06001E5D RID: 7773
		void Add(Announcement announcement);

		// Token: 0x06001E5E RID: 7774
		bool Remove(ulong id);

		// Token: 0x06001E5F RID: 7775
		bool GetAnnouncement(ulong id, out Announcement announcement);

		// Token: 0x06001E60 RID: 7776
		void ModifyAnnouncement(Announcement announcement);

		// Token: 0x06001E61 RID: 7777
		void UpdateCache(ulong deleteId);
	}
}
