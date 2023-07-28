using System;
using System.Collections.Generic;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Reconnect
{
	// Token: 0x020004C7 RID: 1223
	internal class ReconnectConfig
	{
		// Token: 0x06001A7F RID: 6783 RVA: 0x0006CDCF File Offset: 0x0006B1CF
		public ReconnectConfig(TimeSpan reconnectTimeout, IEnumerable<GameRoomType> reconnectableRoomTypes)
		{
			this.ReconnectTimeout = reconnectTimeout;
			this.ReconnectableRoomTypes = reconnectableRoomTypes;
		}

		// Token: 0x170002A6 RID: 678
		// (get) Token: 0x06001A80 RID: 6784 RVA: 0x0006CDE5 File Offset: 0x0006B1E5
		// (set) Token: 0x06001A81 RID: 6785 RVA: 0x0006CDED File Offset: 0x0006B1ED
		public TimeSpan ReconnectTimeout { get; private set; }

		// Token: 0x170002A7 RID: 679
		// (get) Token: 0x06001A82 RID: 6786 RVA: 0x0006CDF6 File Offset: 0x0006B1F6
		// (set) Token: 0x06001A83 RID: 6787 RVA: 0x0006CDFE File Offset: 0x0006B1FE
		public IEnumerable<GameRoomType> ReconnectableRoomTypes { get; private set; }
	}
}
