using System;
using System.Globalization;
using System.Linq;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using Util.Common;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000456 RID: 1110
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_change_players_status", Help = "Changes players states.")]
	internal class DebugChangePlayersStatusCmd : ConsoleCommand<DebugChangePlayersStatusCmdParams>
	{
		// Token: 0x0600177F RID: 6015 RVA: 0x00061F74 File Offset: 0x00060374
		public DebugChangePlayersStatusCmd(IGameRoomManager roomManager)
		{
			this.m_roomManager = roomManager;
		}

		// Token: 0x06001780 RID: 6016 RVA: 0x00061F84 File Offset: 0x00060384
		protected override void Execute(DebugChangePlayersStatusCmdParams param)
		{
			RoomPlayer.EStatus status = ReflectionUtils.EnumParse<RoomPlayer.EStatus>(param.Status.ToString(CultureInfo.InvariantCulture));
			Func<IGameRoom, RoomPlayer, bool> playerSelector;
			IGameRoom gameRoom;
			if (param.ProfileId != 0UL)
			{
				gameRoom = this.m_roomManager.GetRoomByPlayer(param.ProfileId);
				playerSelector = ((IGameRoom r, RoomPlayer p) => p.ProfileID == param.ProfileId);
			}
			else
			{
				gameRoom = this.m_roomManager.GetRoom(param.RoomId);
				DebugChangePlayersStatusCmd.AffectedPlayers affectedPlayers = ReflectionUtils.EnumParse<DebugChangePlayersStatusCmd.AffectedPlayers>(param.PlayersAffected.ToString(CultureInfo.InvariantCulture));
				if (affectedPlayers == DebugChangePlayersStatusCmd.AffectedPlayers.All)
				{
					playerSelector = ((IGameRoom r, RoomPlayer p) => true);
				}
				else
				{
					playerSelector = ((IGameRoom r, RoomPlayer p) => !r.GetExtension<RoomMasterExtension>().IsRoomMaster(p.ProfileID));
				}
			}
			gameRoom.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				(from p in r.Players
				where playerSelector(r, p)
				select p).SafeForEach(delegate(RoomPlayer p)
				{
					p.RoomStatus = status;
				});
			});
			Log.Info("debug_change_players_status: ok");
		}

		// Token: 0x04000B4F RID: 2895
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x02000457 RID: 1111
		private enum AffectedPlayers
		{
			// Token: 0x04000B53 RID: 2899
			All = 1,
			// Token: 0x04000B54 RID: 2900
			AllButMaster
		}
	}
}
