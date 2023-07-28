using System;
using System.Collections.Generic;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200000D RID: 13
	internal class GroupSizeConfig
	{
		// Token: 0x06000035 RID: 53 RVA: 0x00004EB1 File Offset: 0x000032B1
		public GroupSizeConfig(Dictionary<GameRoomType, int> groupSize)
		{
			this.m_groupSize = groupSize;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00004EC0 File Offset: 0x000032C0
		public int GetGroupSize(GameRoomType roomType)
		{
			int result;
			if (this.TryGet(roomType, out result))
			{
				return result;
			}
			throw new KeyNotFoundException(string.Format("Can't find group size for room {0}", roomType));
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00004EF2 File Offset: 0x000032F2
		public bool TryGet(GameRoomType roomType, out int size)
		{
			return this.m_groupSize.TryGetValue(roomType, out size);
		}

		// Token: 0x0400001B RID: 27
		private readonly Dictionary<GameRoomType, int> m_groupSize;
	}
}
