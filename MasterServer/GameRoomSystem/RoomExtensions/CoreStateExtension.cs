using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using Util.Common;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F7 RID: 1527
	[RoomExtension]
	internal class CoreStateExtension : RoomExtensionBase
	{
		// Token: 0x0600206F RID: 8303 RVA: 0x00083930 File Offset: 0x00081D30
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			CoreState state = base.Room.GetState<CoreState>(AccessMode.None);
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.Get("DefaultTeamColor1", out state.TeamColors[0]);
			section.Get("DefaultTeamColor2", out state.TeamColors[1]);
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended += this.TrSessionEnded;
			base.Room.tr_player_removed += this.OnTrPlayerRemoved;
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x000839C4 File Offset: 0x00081DC4
		public override void Close()
		{
			base.Room.tr_player_removed -= this.OnTrPlayerRemoved;
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_ended -= this.TrSessionEnded;
			base.Room.RemoveAllPlayers();
			base.Close();
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x00083A18 File Offset: 0x00081E18
		public override void GetStateUpdateRecepients(RoomUpdate.Context ctx, Set<string> recepients)
		{
			if (ctx.target != RoomUpdate.Target.Client)
			{
				return;
			}
			CoreState coreState = (CoreState)ctx.old_state;
			CoreState coreState2 = (CoreState)ctx.new_state;
			foreach (RoomPlayer roomPlayer in coreState2.Players.Values)
			{
				if (coreState.Players.ContainsKey(roomPlayer.ProfileID))
				{
					recepients.Add(roomPlayer.OnlineID);
				}
			}
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x00083AB8 File Offset: 0x00081EB8
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			CoreState old = (CoreState)ctx.old_state;
			CoreState state = (CoreState)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("core");
			CoreStateExtension.SerializeRoomCore(xmlElement, state, ctx.target);
			this.SerializePlayers(xmlElement, ctx, state, old);
			this.SerializeTeamColors(xmlElement, ctx, state, old);
			this.SerializeCurrentStateLeftPlayer(xmlElement, ctx.target, state);
			return xmlElement;
		}

		// Token: 0x06002073 RID: 8307 RVA: 0x00083B20 File Offset: 0x00081F20
		public static void SerializeRoomCore(XmlElement cel, CoreState state, RoomUpdate.Target target)
		{
			cel.SetAttribute("teams_switched", ((!state.TeamsSwapped) ? 0 : 1).ToString());
			if (target == RoomUpdate.Target.Client)
			{
				cel.SetAttribute("room_name", state.RoomName);
				cel.SetAttribute("private", ((!state.Private) ? 0 : 1).ToString());
				cel.SetAttribute("players", state.Players.Count.ToString());
				cel.SetAttribute("can_start", ((!state.CanStart) ? 0 : 1).ToString());
				cel.SetAttribute("team_balanced", ((!state.TeamBalance) ? 0 : 1).ToString());
				cel.SetAttribute("min_ready_players", state.MinReadyPlayers.ToString());
			}
		}

		// Token: 0x06002074 RID: 8308 RVA: 0x00083C34 File Offset: 0x00082034
		private void SerializeTeamColors(XmlElement cel, RoomUpdate.Context ctx, CoreState state, CoreState old)
		{
			bool flag = ctx.kind == RoomUpdate.Kind.Full || ctx.target == RoomUpdate.Target.Server || state.TeamColors.Length != old.TeamColors.Length;
			int num = 0;
			while (num != state.TeamColors.Length && !flag)
			{
				flag = (state.TeamColors[num] != old.TeamColors[num]);
				num++;
			}
			if (!flag)
			{
				return;
			}
			XmlElement xmlElement = cel.OwnerDocument.CreateElement("team_colors");
			cel.AppendChild(xmlElement);
			for (int num2 = 0; num2 != state.TeamColors.Length; num2++)
			{
				XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("team_color");
				xmlElement2.SetAttribute("id", (num2 + 1).ToString());
				xmlElement2.SetAttribute("color", state.TeamColors[num2].ToString());
				xmlElement.AppendChild(xmlElement2);
			}
		}

		// Token: 0x06002075 RID: 8309 RVA: 0x00083D40 File Offset: 0x00082140
		private void SerializePlayers(XmlElement cel, RoomUpdate.Context ctx, CoreState state, CoreState old)
		{
			bool flag = ctx.kind == RoomUpdate.Kind.Full || ctx.target == RoomUpdate.Target.Server;
			XmlElement xmlElement = cel.OwnerDocument.CreateElement("players");
			XmlElement xmlElement2 = cel.OwnerDocument.CreateElement("playersReserved");
			IEnumerable<RoomPlayer> enumerable = (state == null) ? Enumerable.Empty<RoomPlayer>() : state.Players.Values.Concat(state.ReservedPlayers.Values);
			IEnumerable<RoomPlayer> enumerable2 = (old == null) ? Enumerable.Empty<RoomPlayer>() : old.Players.Values.Concat(old.ReservedPlayers.Values);
			foreach (RoomPlayer roomPlayer in enumerable)
			{
				XmlElement xmlElement3;
				if (state.ReservedPlayers.ContainsKey(roomPlayer.ProfileID))
				{
					xmlElement3 = xmlElement2.OwnerDocument.CreateElement("player");
					xmlElement2.AppendChild(xmlElement3);
				}
				else
				{
					xmlElement3 = xmlElement.OwnerDocument.CreateElement("player");
					xmlElement.AppendChild(xmlElement3);
				}
				xmlElement3.SetAttribute("profile_id", roomPlayer.ProfileID.ToString());
				if (ctx.kind != RoomUpdate.Kind.Full && ctx.target != RoomUpdate.Target.Server)
				{
					if (!this.IsPlayerChanged(old, roomPlayer))
					{
						continue;
					}
					flag = true;
				}
				roomPlayer.Serialize(xmlElement3, ctx.target);
			}
			if (!flag)
			{
				foreach (RoomPlayer roomPlayer2 in enumerable2)
				{
					if (!state.Players.ContainsKey(roomPlayer2.ProfileID) || !state.ReservedPlayers.ContainsKey(roomPlayer2.ProfileID))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				cel.AppendChild(xmlElement);
				cel.AppendChild(xmlElement2);
			}
		}

		// Token: 0x06002076 RID: 8310 RVA: 0x00083F6C File Offset: 0x0008236C
		private void SerializeCurrentStateLeftPlayer(XmlElement cel, RoomUpdate.Target target, CoreState state)
		{
			if (!state.RoomLeftPlayers.Any<KeyValuePair<ulong, GameRoomPlayerRemoveReason>>() || target != RoomUpdate.Target.Server)
			{
				return;
			}
			XmlElement xmlElement = cel.OwnerDocument.CreateElement("room_left_players");
			foreach (KeyValuePair<ulong, GameRoomPlayerRemoveReason> keyValuePair in state.RoomLeftPlayers)
			{
				XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("player");
				xmlElement2.SetAttribute("profile_id", keyValuePair.Key.ToString());
				xmlElement2.SetAttribute("left_reason", ((uint)keyValuePair.Value).ToString());
				xmlElement.AppendChild(xmlElement2);
			}
			cel.AppendChild(xmlElement);
			state.RoomLeftPlayers.Clear();
		}

		// Token: 0x06002077 RID: 8311 RVA: 0x00084058 File Offset: 0x00082458
		private bool IsPlayerChanged(CoreState oldState, RoomPlayer player)
		{
			bool flag = true;
			bool flag2 = true;
			RoomPlayer roomPlayer;
			if (oldState.Players.TryGetValue(player.ProfileID, out roomPlayer) && player.Revision == roomPlayer.Revision && player.Equals(roomPlayer))
			{
				flag = false;
			}
			RoomPlayer roomPlayer2;
			if (oldState.ReservedPlayers.TryGetValue(player.ProfileID, out roomPlayer2) && player.Revision == roomPlayer2.Revision && player.Equals(roomPlayer2))
			{
				flag2 = false;
			}
			return flag || flag2;
		}

		// Token: 0x06002078 RID: 8312 RVA: 0x000840E4 File Offset: 0x000824E4
		private void TrSessionEnded(string sessionId, bool abnormal)
		{
			DateTime now = DateTime.Now;
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadWrite);
			foreach (RoomPlayer roomPlayer in state.Players.Values)
			{
				roomPlayer.SessionsPlayedInRoom++;
				roomPlayer.LastSessionEndTime = now;
			}
		}

		// Token: 0x06002079 RID: 8313 RVA: 0x00084168 File Offset: 0x00082568
		private void OnTrPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadWrite);
			state.RoomLeftPlayers.AddOrUpdate(player.ProfileID, (ulong v) => reason, (ulong k, GameRoomPlayerRemoveReason v) => reason);
		}
	}
}
