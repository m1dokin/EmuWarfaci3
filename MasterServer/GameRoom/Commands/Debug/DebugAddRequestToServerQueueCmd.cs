using System;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000101 RID: 257
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "add_request_server_queue", Help = "Add spam to dedicated server request queue")]
	internal class DebugAddRequestToServerQueueCmd : ConsoleCommand<DebugAddRequestToServerQueueParams>
	{
		// Token: 0x06000433 RID: 1075 RVA: 0x000123A8 File Offset: 0x000107A8
		public DebugAddRequestToServerQueueCmd(IGameRoomServer gameRoomServer)
		{
			this.m_gameRoomServer = gameRoomServer;
		}

		// Token: 0x06000434 RID: 1076 RVA: 0x000123B8 File Offset: 0x000107B8
		protected override void Execute(DebugAddRequestToServerQueueParams param)
		{
			for (ulong num = 0UL; num < param.Count; num += 1UL)
			{
				this.m_gameRoomServer.RequestServer(num, num.ToString(), num.ToString());
			}
		}

		// Token: 0x040001BA RID: 442
		private readonly IGameRoomServer m_gameRoomServer;
	}
}
