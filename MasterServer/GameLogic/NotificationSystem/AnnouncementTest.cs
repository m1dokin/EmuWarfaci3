using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000588 RID: 1416
	[ConsoleCmdAttributes(CmdName = "announcement_test", ArgsSize = 0, Help = "Add 10 different announcements")]
	internal class AnnouncementTest : IConsoleCmd
	{
		// Token: 0x06001E74 RID: 7796 RVA: 0x0007B9D0 File Offset: 0x00079DD0
		public AnnouncementTest(IAnnouncementService announcementService)
		{
			this.m_announcementService = announcementService;
		}

		// Token: 0x06001E75 RID: 7797 RVA: 0x0007B9E0 File Offset: 0x00079DE0
		public void ExecuteCmd(string[] args)
		{
			Log.Info("Announcement test");
			Random random = new Random();
			for (int i = 0; i < 10; i++)
			{
				Announcement announcement = new Announcement
				{
					IsSystem = (i % 2 == 0),
					Message = string.Format("Auto generated {0} announcement {1}", (i % 2 != 0) ? string.Empty : "system", i),
					StartTimeUTC = DateTime.UtcNow.AddMinutes(1.0),
					EndTimeUTC = DateTime.UtcNow.AddMinutes(20.0),
					RepeatTimes = (uint)random.Next(1, 10),
					Channel = string.Empty,
					Server = string.Empty,
					Place = EAnnouncementPlace.None,
					Target = 0UL
				};
				this.m_announcementService.Add(announcement);
			}
		}

		// Token: 0x04000ED4 RID: 3796
		private readonly IAnnouncementService m_announcementService;
	}
}
