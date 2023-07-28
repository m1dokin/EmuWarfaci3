using System;
using System.Linq;
using System.Text;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000459 RID: 1113
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "debug_check_room_balance", Help = "Prints the results of players balancing in the autobalance-enabled room.")]
	internal class DebugCheckRoomBalanceCmd : ConsoleCommand<DebugCheckRoomBalanceCmdParams>
	{
		// Token: 0x0600178C RID: 6028 RVA: 0x000621AA File Offset: 0x000605AA
		public DebugCheckRoomBalanceCmd(IGameRoomManager roomManager)
		{
			this.m_roomManager = roomManager;
		}

		// Token: 0x0600178D RID: 6029 RVA: 0x000621BC File Offset: 0x000605BC
		protected override void Execute(DebugCheckRoomBalanceCmdParams param)
		{
			IGameRoom room = this.m_roomManager.GetRoom(param.RoomId);
			if (room == null)
			{
				return;
			}
			bool isAutobalanceEnabled = false;
			room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				isAutobalanceEnabled = r.GetState<CustomParams>(AccessMode.ReadOnly).Autobalance;
			});
			if (!isAutobalanceEnabled && param.Force)
			{
				room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
				{
					r.GetState<CustomParams>(AccessMode.ReadWrite).Autobalance = true;
				});
			}
			room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
			{
				if (param.All)
				{
					room.GetExtension<TeamExtension>().DebugBalanceAllPlayers();
				}
				else
				{
					room.GetExtension<TeamExtension>().DebugBalanceReadyPlayers();
				}
				IOrderedEnumerable<IGrouping<int, RoomPlayer>> orderedEnumerable = from p in room.Players
				group p by p.TeamID into g
				orderby g.Key
				select g;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (IGrouping<int, RoomPlayer> grouping in orderedEnumerable)
				{
					stringBuilder.AppendFormat("Team {0}:", grouping.Key);
					double num = 0.0;
					var source = (from p in grouping
					select new
					{
						p.ProfileID,
						p.GroupID,
						p.Skill
					}).ToList();
					var orderedEnumerable2 = from p in source
					group p by p.GroupID into g
					orderby g.Key
					select g;
					foreach (var grouping2 in orderedEnumerable2)
					{
						double num2 = 0.0;
						var list = grouping2.ToList();
						double num3 = list.Max(p => p.Skill.Value);
						bool flag = !string.IsNullOrEmpty(grouping2.Key);
						if (flag)
						{
							stringBuilder.AppendFormat("Group {0}:", grouping2.Key);
						}
						else
						{
							stringBuilder.AppendLine("Not in group:");
						}
						foreach (var <>__AnonType in list)
						{
							double num4 = (!flag) ? <>__AnonType.Skill.Value : num3;
							num2 += num4;
							stringBuilder.AppendFormat("Player {0}, group: {1}, skill: {2}", <>__AnonType.ProfileID, <>__AnonType.GroupID, num4);
						}
						stringBuilder.AppendFormat("Total group skill : {0}", num2);
						num += num2;
					}
					stringBuilder.AppendFormat("Total team skill : {0}", num);
				}
				Log.Info(stringBuilder.ToString());
			});
		}

		// Token: 0x04000B59 RID: 2905
		private readonly IGameRoomManager m_roomManager;
	}
}
