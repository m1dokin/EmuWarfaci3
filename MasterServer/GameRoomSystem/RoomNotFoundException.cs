using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005DB RID: 1499
	public class RoomNotFoundException : ApplicationException
	{
		// Token: 0x06001FED RID: 8173 RVA: 0x000820CC File Offset: 0x000804CC
		public RoomNotFoundException(ulong room_id) : base(string.Format("Room with {0} doesn't exist", room_id))
		{
		}

		// Token: 0x06001FEE RID: 8174 RVA: 0x000820E4 File Offset: 0x000804E4
		public RoomNotFoundException(string message) : base(message)
		{
		}
	}
}
