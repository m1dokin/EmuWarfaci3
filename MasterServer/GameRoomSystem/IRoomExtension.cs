using System;
using System.Xml;
using HK2Net;
using MasterServer.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005E2 RID: 1506
	[Contract]
	internal interface IRoomExtension : IDisposable
	{
		// Token: 0x06001FFA RID: 8186
		void Init(IGameRoom room);

		// Token: 0x06001FFB RID: 8187
		void Close();

		// Token: 0x06001FFC RID: 8188
		void GetStateUpdateRecepients(RoomUpdate.Context ctx, Set<string> recepients);

		// Token: 0x06001FFD RID: 8189
		XmlElement SerializeStateChanges(RoomUpdate.Context ctx);

		// Token: 0x06001FFE RID: 8190
		void PostStateChanged(IRoomState new_state, IRoomState old_state);
	}
}
