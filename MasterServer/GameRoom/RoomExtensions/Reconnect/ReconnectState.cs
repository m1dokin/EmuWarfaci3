using System;
using System.Collections.Generic;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Reconnect
{
	// Token: 0x020004C3 RID: 1219
	[RoomState(new Type[]
	{
		typeof(ReconnectExtension)
	})]
	internal class ReconnectState : RoomStateBase
	{
		// Token: 0x06001A67 RID: 6759 RVA: 0x0006CA94 File Offset: 0x0006AE94
		public ReconnectState()
		{
			this.ReconnectInfos = new Dictionary<ulong, ReconnectInfo>();
		}

		// Token: 0x170002A4 RID: 676
		// (get) Token: 0x06001A68 RID: 6760 RVA: 0x0006CAA7 File Offset: 0x0006AEA7
		// (set) Token: 0x06001A69 RID: 6761 RVA: 0x0006CAAF File Offset: 0x0006AEAF
		public Dictionary<ulong, ReconnectInfo> ReconnectInfos { get; private set; }

		// Token: 0x06001A6A RID: 6762 RVA: 0x0006CAB8 File Offset: 0x0006AEB8
		public override object Clone()
		{
			ReconnectState reconnectState = (ReconnectState)base.Clone();
			reconnectState.ReconnectInfos = new Dictionary<ulong, ReconnectInfo>();
			foreach (KeyValuePair<ulong, ReconnectInfo> keyValuePair in this.ReconnectInfos)
			{
				reconnectState.ReconnectInfos.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return reconnectState;
		}
	}
}
