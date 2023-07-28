using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F9 RID: 761
	[ConsoleCmdAttributes(CmdName = "set_gameroom_teambalance", ArgsSize = 2, Help = "Set teambalance in room with player")]
	internal class SetGameRoomTeamBalance : IConsoleCmd
	{
		// Token: 0x06001190 RID: 4496 RVA: 0x00045875 File Offset: 0x00043C75
		public SetGameRoomTeamBalance(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06001191 RID: 4497 RVA: 0x00045884 File Offset: 0x00043C84
		public void ExecuteCmd(string[] args)
		{
			if (args.Length >= 3)
			{
				IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(ulong.Parse(args[1]));
				if (roomByPlayer != null)
				{
					roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.TeamBalance = (uint.Parse(args[2]) != 0U);
					});
				}
				else
				{
					Log.Info("You should be joined to room to use this command");
				}
			}
		}

		// Token: 0x040007BF RID: 1983
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
