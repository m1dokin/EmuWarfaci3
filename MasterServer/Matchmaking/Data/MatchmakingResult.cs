using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000506 RID: 1286
	internal class MatchmakingResult
	{
		// Token: 0x170002DD RID: 733
		// (get) Token: 0x06001BCB RID: 7115 RVA: 0x0007095E File Offset: 0x0006ED5E
		public IEnumerable<MMResultEntity> SuccededEntities
		{
			get
			{
				return this.RoomUpdates.SelectMany((MMResultRoomUpdate update) => update.Entities);
			}
		}

		// Token: 0x170002DE RID: 734
		// (get) Token: 0x06001BCC RID: 7116 RVA: 0x00070988 File Offset: 0x0006ED88
		public IEnumerable<MMResultEntity> AllEntities
		{
			get
			{
				return this.FailedEntities.Concat(this.SuccededEntities);
			}
		}

		// Token: 0x04000D4D RID: 3405
		public List<MMResultRoomUpdate> RoomUpdates = new List<MMResultRoomUpdate>();

		// Token: 0x04000D4E RID: 3406
		public List<MMResultEntity> FailedEntities = new List<MMResultEntity>();
	}
}
