using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005DC RID: 1500
	public class RoomGenericException : ApplicationException
	{
		// Token: 0x06001FEF RID: 8175 RVA: 0x000820ED File Offset: 0x000804ED
		public RoomGenericException(ulong room_id, string message) : base(string.Format("Room {0} throws '{1}'", room_id, message))
		{
		}
	}
}
