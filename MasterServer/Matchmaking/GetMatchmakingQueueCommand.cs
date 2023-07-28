using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000502 RID: 1282
	[ConsoleCmdAttributes(CmdName = "mm_get_matchmaking_queue", ArgsSize = 0, Help = "Returns current matchmaking queue state info.")]
	internal class GetMatchmakingQueueCommand : IConsoleCmd
	{
		// Token: 0x06001BB5 RID: 7093 RVA: 0x000704C8 File Offset: 0x0006E8C8
		public GetMatchmakingQueueCommand(IMatchmakingSystem matchmakingSystem)
		{
			this.m_matchmakingSystem = matchmakingSystem;
		}

		// Token: 0x06001BB6 RID: 7094 RVA: 0x000704D8 File Offset: 0x0006E8D8
		public void ExecuteCmd(string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("\nMatchmaking queue:");
			foreach (KeyValuePair<GameRoomType, MMEntityPool> keyValuePair in this.m_matchmakingSystem.GetQueue())
			{
				stringBuilder.AppendFormat("\tRoom type '{0}' queue:\r\n", keyValuePair.Key);
				foreach (MMEntityInfo mmentityInfo in keyValuePair.Value.GetEntities())
				{
					stringBuilder.AppendFormat("\t\tEntity: <Id:'{0}' Initiator: <Id:'{1}' Name:'{2}'> Skill:{3}>:\r\n", new object[]
					{
						mmentityInfo.Id,
						mmentityInfo.Initiator.ProfileID,
						mmentityInfo.Initiator.Nickname,
						mmentityInfo.GetSkill()
					});
					foreach (MMPlayerInfo mmplayerInfo in mmentityInfo.Players)
					{
						stringBuilder.AppendFormat("\t\t\tPlayer: <Id:'{0}' Name:'{1}' Skill type:{2} Skill:{3}>:\r\n", new object[]
						{
							mmplayerInfo.User.ProfileID,
							mmplayerInfo.User.Nickname,
							mmplayerInfo.Skill.Type,
							mmplayerInfo.Skill.Value
						});
					}
				}
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x04000D45 RID: 3397
		private readonly IMatchmakingSystem m_matchmakingSystem;
	}
}
