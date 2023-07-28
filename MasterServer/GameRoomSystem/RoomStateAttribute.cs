using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005DE RID: 1502
	internal class RoomStateAttribute : ServiceAttribute
	{
		// Token: 0x06001FF3 RID: 8179 RVA: 0x0008218A File Offset: 0x0008058A
		public RoomStateAttribute(params Type[] ownerExtensions)
		{
			this.OwnerExtensions = ownerExtensions;
		}

		// Token: 0x04000FA7 RID: 4007
		public Type[] OwnerExtensions;
	}
}
