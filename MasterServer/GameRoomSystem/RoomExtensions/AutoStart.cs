using System;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F1 RID: 1521
	[RoomState(new Type[]
	{
		typeof(AutoStartExtension)
	})]
	internal class AutoStart : RoomStateBase
	{
		// Token: 0x1700034F RID: 847
		// (get) Token: 0x0600204F RID: 8271 RVA: 0x00082C78 File Offset: 0x00081078
		// (set) Token: 0x06002050 RID: 8272 RVA: 0x00082C80 File Offset: 0x00081080
		public bool IsIntermission { get; set; }

		// Token: 0x17000350 RID: 848
		// (get) Token: 0x06002051 RID: 8273 RVA: 0x00082C89 File Offset: 0x00081089
		// (set) Token: 0x06002052 RID: 8274 RVA: 0x00082C91 File Offset: 0x00081091
		public DateTime IntermissionEnd { get; set; }

		// Token: 0x17000351 RID: 849
		// (get) Token: 0x06002053 RID: 8275 RVA: 0x00082C9A File Offset: 0x0008109A
		// (set) Token: 0x06002054 RID: 8276 RVA: 0x00082CA2 File Offset: 0x000810A2
		public bool CanManualStart { get; set; }
	}
}
