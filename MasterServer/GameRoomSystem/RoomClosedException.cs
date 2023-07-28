using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005DA RID: 1498
	public class RoomClosedException : ApplicationException
	{
		// Token: 0x06001FEC RID: 8172 RVA: 0x000820B4 File Offset: 0x000804B4
		public RoomClosedException(ulong room_id) : base(string.Format("Room {0} is closed on transaction start", room_id))
		{
		}
	}
}
