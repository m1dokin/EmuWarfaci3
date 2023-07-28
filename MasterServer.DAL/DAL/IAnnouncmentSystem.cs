using System;

namespace MasterServer.DAL
{
	// Token: 0x02000012 RID: 18
	public interface IAnnouncmentSystem
	{
		// Token: 0x06000026 RID: 38
		DALResult<ulong> Add(Announcement announcement);

		// Token: 0x06000027 RID: 39
		DALResultVoid Modify(Announcement announcement);

		// Token: 0x06000028 RID: 40
		DALResultVoid Remove(ulong id);

		// Token: 0x06000029 RID: 41
		DALResult<Announcement> GetAnnouncementById(ulong id);

		// Token: 0x0600002A RID: 42
		DALResultMulti<Announcement> GetAnnouncements();

		// Token: 0x0600002B RID: 43
		DALResultMulti<Announcement> GetActiveAnnouncements();
	}
}
