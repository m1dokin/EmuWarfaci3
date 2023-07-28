using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000589 RID: 1417
	[ConsoleCmdAttributes(CmdName = "announcement_delete", ArgsSize = 0, Help = "Delete all announcements")]
	internal class AnnouncementDelete : IConsoleCmd
	{
		// Token: 0x06001E76 RID: 7798 RVA: 0x0007BACB File Offset: 0x00079ECB
		public AnnouncementDelete(IAnnouncementService announcementService)
		{
			this.m_announcementService = announcementService;
		}

		// Token: 0x06001E77 RID: 7799 RVA: 0x0007BADC File Offset: 0x00079EDC
		public void ExecuteCmd(string[] args)
		{
			Log.Info("Announcement delete");
			foreach (Announcement announcement in this.m_announcementService.GetAnnouncements())
			{
				this.m_announcementService.Remove(announcement.ID);
				Log.Info<ulong>("Announcement {0} removed successfully", announcement.ID);
			}
		}

		// Token: 0x04000ED5 RID: 3797
		private readonly IAnnouncementService m_announcementService;
	}
}
