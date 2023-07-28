using System;
using System.Xml;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005E4 RID: 1508
	internal class RoomUpdate
	{
		// Token: 0x020005E5 RID: 1509
		public enum Target
		{
			// Token: 0x04000FAC RID: 4012
			Client,
			// Token: 0x04000FAD RID: 4013
			Server
		}

		// Token: 0x020005E6 RID: 1510
		[Flags]
		public enum InformationType
		{
			// Token: 0x04000FAF RID: 4015
			UiBaseInfo = 1,
			// Token: 0x04000FB0 RID: 4016
			CrownRewardsInfo = 2
		}

		// Token: 0x020005E7 RID: 1511
		public enum Kind
		{
			// Token: 0x04000FB2 RID: 4018
			Full,
			// Token: 0x04000FB3 RID: 4019
			Incremental
		}

		// Token: 0x020005E8 RID: 1512
		public class Context
		{
			// Token: 0x04000FB4 RID: 4020
			public RoomUpdate.Target target;

			// Token: 0x04000FB5 RID: 4021
			public RoomUpdate.Kind kind;

			// Token: 0x04000FB6 RID: 4022
			public IRoomState old_state;

			// Token: 0x04000FB7 RID: 4023
			public IRoomState new_state;

			// Token: 0x04000FB8 RID: 4024
			public XmlDocument factory;
		}
	}
}
