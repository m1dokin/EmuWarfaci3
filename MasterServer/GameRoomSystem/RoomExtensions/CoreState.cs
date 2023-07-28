using System;
using System.Collections.Generic;
using MasterServer.Common;
using MasterServer.Core;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F6 RID: 1526
	[RoomState(new Type[]
	{
		typeof(CoreStateExtension)
	})]
	internal class CoreState : RoomStateBase
	{
		// Token: 0x0600206D RID: 8301 RVA: 0x0008380C File Offset: 0x00081C0C
		public override object Clone()
		{
			CoreState coreState = (CoreState)base.Clone();
			coreState.Players = new Dictionary<ulong, RoomPlayer>();
			foreach (RoomPlayer roomPlayer in this.Players.Values)
			{
				coreState.Players.Add(roomPlayer.ProfileID, (RoomPlayer)roomPlayer.Clone());
			}
			coreState.ReservedPlayers = new Dictionary<ulong, RoomPlayer>();
			foreach (RoomPlayer roomPlayer2 in this.ReservedPlayers.Values)
			{
				coreState.ReservedPlayers.Add(roomPlayer2.ProfileID, (RoomPlayer)roomPlayer2.Clone());
			}
			coreState.TeamColors = Utils.Copy<uint>(this.TeamColors);
			this.RoomLeftPlayers = new Dictionary<ulong, GameRoomPlayerRemoveReason>();
			return coreState;
		}

		// Token: 0x04000FDE RID: 4062
		public string RoomName;

		// Token: 0x04000FDF RID: 4063
		public ulong MMGeneration;

		// Token: 0x04000FE0 RID: 4064
		public bool Private;

		// Token: 0x04000FE1 RID: 4065
		public bool Locked;

		// Token: 0x04000FE2 RID: 4066
		public int MinReadyPlayers;

		// Token: 0x04000FE3 RID: 4067
		public int TeamsReadyPlayersDiff;

		// Token: 0x04000FE4 RID: 4068
		public Dictionary<ulong, RoomPlayer> Players = new Dictionary<ulong, RoomPlayer>();

		// Token: 0x04000FE5 RID: 4069
		public Dictionary<ulong, RoomPlayer> ReservedPlayers = new Dictionary<ulong, RoomPlayer>();

		// Token: 0x04000FE6 RID: 4070
		public bool TeamsSwapped;

		// Token: 0x04000FE7 RID: 4071
		public uint[] TeamColors = new uint[2];

		// Token: 0x04000FE8 RID: 4072
		public bool CanStart;

		// Token: 0x04000FE9 RID: 4073
		public bool TeamBalance = !Resources.DebugGameModeSettingsEnabled;

		// Token: 0x04000FEA RID: 4074
		public bool AllowManualJoin = true;

		// Token: 0x04000FEB RID: 4075
		public Dictionary<ulong, GameRoomPlayerRemoveReason> RoomLeftPlayers = new Dictionary<ulong, GameRoomPlayerRemoveReason>();
	}
}
