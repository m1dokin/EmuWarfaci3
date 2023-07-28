using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoom.Commands
{
	// Token: 0x02000496 RID: 1174
	[ConsoleCmdAttributes(CmdName = "room_list")]
	internal class GameRoomListCmd : IConsoleCmd
	{
		// Token: 0x06001886 RID: 6278 RVA: 0x00065398 File Offset: 0x00063798
		public GameRoomListCmd(IGameRoomManager roomManager)
		{
			this.m_roomManager = roomManager;
		}

		// Token: 0x06001887 RID: 6279 RVA: 0x000653A8 File Offset: 0x000637A8
		public void ExecuteCmd(string[] args)
		{
			List<IGameRoom> rooms = this.m_roomManager.GetRooms((IGameRoom x) => x.IsPveMode());
			List<IGameRoom> rooms2 = this.m_roomManager.GetRooms((IGameRoom x) => x.IsPvpMode());
			StringBuilder sb = new StringBuilder();
			Action<IGameRoom> printAction = delegate(IGameRoom r)
			{
				ServerExtension extension = r.GetExtension<ServerExtension>();
				sb.AppendFormat("ID: {0}", r.ID);
				sb.AppendFormat(", RoomRef: {0}", r.Reference);
				sb.AppendFormat(", Name: {0}", r.RoomName);
				sb.AppendFormat(", Players: {0}", r.PlayerCount);
				sb.AppendFormat(", Reservation: {0}", r.ReservedPlayers.Count<RoomPlayer>());
				sb.AppendFormat(", Rating: {0}", r.CalculateRoomRating());
				sb.AppendFormat(", Type: {0}", r.Type);
				sb.AppendFormat(", Map: {0}", r.MissionKey);
				sb.AppendFormat(", Status: {0}", extension.Status);
				sb.AppendFormat(", Region id: {0}", r.RegionId ?? "empty");
				sb.AppendFormat("\n", new object[0]);
			};
			sb.Append("\nPVE Rooms:\n");
			int num = this.PrintRoomInfo(rooms, printAction);
			sb.Append("\nPVP Rooms:\n");
			int num2 = this.PrintRoomInfo(rooms2, printAction);
			sb.Append("\nTotal:\n");
			sb.AppendFormat("PVE Rooms: {0}\n", rooms.Count - num);
			sb.AppendFormat("PVP Rooms: {0}\n", rooms2.Count - num2);
			Log.Info(sb.ToString());
		}

		// Token: 0x06001888 RID: 6280 RVA: 0x000654BC File Offset: 0x000638BC
		private int PrintRoomInfo(IEnumerable<IGameRoom> rooms, Action<IGameRoom> printAction)
		{
			int num = 0;
			foreach (IGameRoom gameRoom in rooms)
			{
				try
				{
					gameRoom.transaction(AccessMode.ReadOnly, printAction);
				}
				catch (RoomClosedException)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x04000BF6 RID: 3062
		private readonly IGameRoomManager m_roomManager;
	}
}
