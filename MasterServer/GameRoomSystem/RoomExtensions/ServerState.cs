using System;
using MasterServer.ServerInfo;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000607 RID: 1543
	[RoomState(new Type[]
	{
		typeof(ServerExtension)
	})]
	internal class ServerState : RoomStateBase
	{
		// Token: 0x06002102 RID: 8450 RVA: 0x00088064 File Offset: 0x00086464
		public override object Clone()
		{
			ServerState serverState = (ServerState)base.Clone();
			serverState.Server = (ServerEntity)this.Server.Clone();
			return serverState;
		}

		// Token: 0x06002103 RID: 8451 RVA: 0x00088094 File Offset: 0x00086494
		public void Invalidate()
		{
			this.Server = new ServerEntity();
		}

		// Token: 0x0400101A RID: 4122
		public ServerEntity Server = new ServerEntity();
	}
}
