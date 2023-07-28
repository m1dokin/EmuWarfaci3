using System;
using MasterServer.Core;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000463 RID: 1123
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_kick_room_player", Help = "Kick player.")]
	internal class DebugKickRoomPlayerCmd : ConsoleCommand<DebugKickRoomPlayerCmdParams>
	{
		// Token: 0x060017AF RID: 6063 RVA: 0x00062AA1 File Offset: 0x00060EA1
		public DebugKickRoomPlayerCmd(IPunishmentService punishmentService)
		{
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00062AB0 File Offset: 0x00060EB0
		protected override void Execute(DebugKickRoomPlayerCmdParams param)
		{
			bool flag = this.m_punishmentService.KickPlayerLocal(param.ProfileId, param.Reason);
			Log.Info<string, ulong, GameRoomPlayerRemoveReason>("Profile {0} {1} kicked with reason {2}", (!flag) ? "isn't" : "is", param.ProfileId, param.Reason);
		}

		// Token: 0x04000B70 RID: 2928
		private readonly IPunishmentService m_punishmentService;
	}
}
