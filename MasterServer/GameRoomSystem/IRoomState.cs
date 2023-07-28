using System;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005DF RID: 1503
	[Contract]
	internal interface IRoomState : ICloneable
	{
		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001FF4 RID: 8180
		uint Revision { get; }
	}
}
