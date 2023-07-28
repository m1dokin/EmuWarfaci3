using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F1 RID: 497
	internal interface IAnnouncementSystemClient
	{
		// Token: 0x060009B3 RID: 2483
		ulong Add(Announcement announcement);

		// Token: 0x060009B4 RID: 2484
		void Modify(Announcement announcement);

		// Token: 0x060009B5 RID: 2485
		void Remove(ulong id);

		// Token: 0x060009B6 RID: 2486
		Announcement GetAnnouncementById(ulong id);

		// Token: 0x060009B7 RID: 2487
		IEnumerable<Announcement> GetAnnouncements();

		// Token: 0x060009B8 RID: 2488
		IEnumerable<Announcement> GetActiveAnnouncements();
	}
}
