using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200077E RID: 1918
	public struct SlotDefData
	{
		// Token: 0x060027BB RID: 10171 RVA: 0x000A9CC9 File Offset: 0x000A80C9
		public SlotDefData(ulong id_, bool alwaysEquip_)
		{
			this.id = id_;
			this.alwaysEquip = alwaysEquip_;
		}

		// Token: 0x040014C7 RID: 5319
		public ulong id;

		// Token: 0x040014C8 RID: 5320
		public bool alwaysEquip;
	}
}
