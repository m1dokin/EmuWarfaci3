using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005DD RID: 1501
	public class RoomUnsupportedExtensionException : ApplicationException
	{
		// Token: 0x06001FF0 RID: 8176 RVA: 0x00082106 File Offset: 0x00080506
		public RoomUnsupportedExtensionException(ulong roomId, GameRoomType type, Type extensionType, IEnumerable<Type> extensionTypes) : base(RoomUnsupportedExtensionException.CreateMessage(roomId, type, extensionType, extensionTypes))
		{
		}

		// Token: 0x06001FF1 RID: 8177 RVA: 0x00082118 File Offset: 0x00080518
		private static string CreateMessage(ulong roomId, GameRoomType type, Type extensionType, IEnumerable<Type> extensionTypes)
		{
			string format = "Getting unsupported extension '{0}' from '{1}' failed for room {2} {3}";
			object[] array = new object[4];
			array[0] = extensionType.Name;
			array[1] = string.Join(";", from e in extensionTypes
			select e.Name);
			array[2] = roomId;
			array[3] = type;
			return string.Format(format, array);
		}
	}
}
