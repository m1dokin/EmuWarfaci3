using System;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F4 RID: 1524
	[RoomState(new Type[]
	{
		typeof(ClanWarExtension)
	})]
	internal class ClanWar : RoomStateBase
	{
		// Token: 0x04000FDC RID: 4060
		public string Clan1;

		// Token: 0x04000FDD RID: 4061
		public string Clan2;
	}
}
