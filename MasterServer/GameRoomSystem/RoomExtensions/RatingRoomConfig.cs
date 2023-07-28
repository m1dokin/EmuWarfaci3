using System;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004C0 RID: 1216
	internal class RatingRoomConfig
	{
		// Token: 0x06001A4D RID: 6733 RVA: 0x0006C3B4 File Offset: 0x0006A7B4
		public RatingRoomConfig(TimeSpan playersCheckTimeout)
		{
			this.PlayersCheckTimeout = playersCheckTimeout;
		}

		// Token: 0x170002A3 RID: 675
		// (get) Token: 0x06001A4E RID: 6734 RVA: 0x0006C3C3 File Offset: 0x0006A7C3
		// (set) Token: 0x06001A4F RID: 6735 RVA: 0x0006C3CB File Offset: 0x0006A7CB
		public TimeSpan PlayersCheckTimeout { get; private set; }
	}
}
