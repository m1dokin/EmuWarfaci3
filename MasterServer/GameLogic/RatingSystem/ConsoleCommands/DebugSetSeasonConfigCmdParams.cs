using System;
using CommandLine;

namespace MasterServer.GameLogic.RatingSystem.ConsoleCommands
{
	// Token: 0x02000091 RID: 145
	internal class DebugSetSeasonConfigCmdParams
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000230 RID: 560 RVA: 0x0000C1C1 File Offset: 0x0000A5C1
		// (set) Token: 0x06000231 RID: 561 RVA: 0x0000C1C9 File Offset: 0x0000A5C9
		[Option('i', "season_id", Required = false, DefaultValue = "", HelpText = "Season id")]
		public string SeasonId { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000232 RID: 562 RVA: 0x0000C1D2 File Offset: 0x0000A5D2
		// (set) Token: 0x06000233 RID: 563 RVA: 0x0000C1DA File Offset: 0x0000A5DA
		[Option('a', "announcement_end_date", Required = true, HelpText = "Rating season announcement end date")]
		public DateTime AnnouncementEndDate { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000234 RID: 564 RVA: 0x0000C1E3 File Offset: 0x0000A5E3
		// (set) Token: 0x06000235 RID: 565 RVA: 0x0000C1EB File Offset: 0x0000A5EB
		[Option('e', "games_end_date", Required = true, HelpText = "Rating season games end date")]
		public DateTime GamesEndDate { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000236 RID: 566 RVA: 0x0000C1F4 File Offset: 0x0000A5F4
		// (set) Token: 0x06000237 RID: 567 RVA: 0x0000C1FC File Offset: 0x0000A5FC
		[Option('s', "season_active", Required = true, HelpText = "Sets season 'started' state)")]
		public bool SeasonActive { get; set; }
	}
}
