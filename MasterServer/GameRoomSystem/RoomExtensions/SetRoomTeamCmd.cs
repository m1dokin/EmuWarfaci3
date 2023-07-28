using System;
using MasterServer.Core;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004CD RID: 1229
	[ConsoleCmdAttributes(CmdName = "set_room_team", ArgsSize = 3, Help = "Force set team args: <roomId> <team> [profileId]")]
	internal class SetRoomTeamCmd : IConsoleCmd
	{
		// Token: 0x06001A9C RID: 6812 RVA: 0x0006D32D File Offset: 0x0006B72D
		public SetRoomTeamCmd(IDebugGameRoomService debugGameRoomService)
		{
			this.m_debugGameRoomService = debugGameRoomService;
		}

		// Token: 0x06001A9D RID: 6813 RVA: 0x0006D33C File Offset: 0x0006B73C
		public void ExecuteCmd(string[] args)
		{
			ulong num = ulong.Parse(args[1]);
			int team = int.Parse(args[2]);
			if (args.Length > 3)
			{
				ulong profileId = ulong.Parse(args[3]);
				this.m_debugGameRoomService.SetPlayerTeam(num, profileId, team);
			}
			else
			{
				this.m_debugGameRoomService.SetTeamForAllPalyers(num, team);
			}
			Log.Info<ulong>("Team set for room {0} done", num);
		}

		// Token: 0x04000CBB RID: 3259
		private readonly IDebugGameRoomService m_debugGameRoomService;
	}
}
