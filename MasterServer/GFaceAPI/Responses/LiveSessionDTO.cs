using System;

namespace MasterServer.GFaceAPI.Responses
{
	// Token: 0x02000660 RID: 1632
	public class LiveSessionDTO
	{
		// Token: 0x04001140 RID: 4416
		public IdDTO playable;

		// Token: 0x04001141 RID: 4417
		[GRspOptional]
		public string message;

		// Token: 0x04001142 RID: 4418
		public string state;

		// Token: 0x04001143 RID: 4419
		[GRspOptional]
		public RoomDTO room;

		// Token: 0x04001144 RID: 4420
		[GRspOptional]
		public _LiveSessionPlayers players;

		// Token: 0x04001145 RID: 4421
		[GRspOptional]
		public IdDTO attachedto;
	}
}
