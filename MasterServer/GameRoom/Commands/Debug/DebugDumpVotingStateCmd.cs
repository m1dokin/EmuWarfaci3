using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x0200045F RID: 1119
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "dump_map_voting_state", Help = "Shows current voting state in specified room.")]
	internal class DebugDumpVotingStateCmd : ConsoleCommand<DebugDumpVotingStateCmdParams>
	{
		// Token: 0x060017A5 RID: 6053 RVA: 0x000628B3 File Offset: 0x00060CB3
		public DebugDumpVotingStateCmd(IGameRoomManager gameRoomManager, IMissionSystem missionSystem)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x060017A6 RID: 6054 RVA: 0x000628CC File Offset: 0x00060CCC
		protected override void Execute(DebugDumpVotingStateCmdParams param)
		{
			IGameRoom room = this.m_gameRoomManager.GetRoom(param.RoomId);
			if (room != null)
			{
				MapVotingExtension extension = room.GetExtension<MapVotingExtension>();
				IDictionary<string, int> dictionary = extension.DumpVotingState();
				if (dictionary != null)
				{
					StringBuilder stringBuilder = new StringBuilder("\nState of voting\n");
					foreach (KeyValuePair<string, int> keyValuePair in dictionary)
					{
						MissionContext mission = this.m_missionSystem.GetMission(keyValuePair.Key);
						stringBuilder.AppendFormat("\tMap: {0} MapUid: {1} Votes: {2}\n", mission.missionName, keyValuePair.Key, keyValuePair.Value);
					}
					Log.Info(stringBuilder.ToString());
				}
				else
				{
					Log.Error<ulong>("Voting in room {0} is not occurred", param.RoomId);
				}
			}
			else
			{
				Log.Warning<ulong>("There is no specified room with roomId: {0}", param.RoomId);
			}
		}

		// Token: 0x04000B6B RID: 2923
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000B6C RID: 2924
		private readonly IMissionSystem m_missionSystem;
	}
}
