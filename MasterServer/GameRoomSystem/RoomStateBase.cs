using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005E0 RID: 1504
	internal class RoomStateBase : IRoomState, ICloneable
	{
		// Token: 0x06001FF5 RID: 8181 RVA: 0x0006CA49 File Offset: 0x0006AE49
		public RoomStateBase()
		{
			this.Revision = 1U;
		}

		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001FF6 RID: 8182 RVA: 0x0006CA58 File Offset: 0x0006AE58
		// (set) Token: 0x06001FF7 RID: 8183 RVA: 0x0006CA60 File Offset: 0x0006AE60
		public uint Revision { get; private set; }

		// Token: 0x06001FF8 RID: 8184 RVA: 0x0006CA6C File Offset: 0x0006AE6C
		public virtual object Clone()
		{
			RoomStateBase roomStateBase = (RoomStateBase)base.MemberwiseClone();
			roomStateBase.Revision += 1U;
			return roomStateBase;
		}
	}
}
