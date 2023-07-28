using System;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000603 RID: 1539
	[RoomState(new Type[]
	{
		typeof(RoomMasterExtension)
	})]
	internal class RoomMasterState : RoomStateBase
	{
		// Token: 0x04001011 RID: 4113
		public ulong RoomMaster;
	}
}
