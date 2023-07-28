using System;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005FE RID: 1534
	[RoomState(new Type[]
	{
		typeof(MissionExtension)
	})]
	internal class MissionState : RoomStateBase
	{
		// Token: 0x04001008 RID: 4104
		public MissionContext Mission = new MissionContext();
	}
}
