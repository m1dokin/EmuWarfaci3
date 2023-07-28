using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F5 RID: 1525
	[RoomExtension]
	internal class ClanWarExtension : RoomExtensionBase
	{
		// Token: 0x06002063 RID: 8291 RVA: 0x000832D4 File Offset: 0x000816D4
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			base.Room.tr_player_add_check += this.trOnPlayerAddCheck;
			base.Room.tr_player_added += this.trOnPlayerAdded;
			base.Room.tr_player_removed += this.trOnPlayerRemoved;
			base.Room.GetExtension<KickExtension>().tr_player_kick += this.trOnPlayerKick;
		}

		// Token: 0x06002064 RID: 8292 RVA: 0x0008334C File Offset: 0x0008174C
		protected override void OnDisposing()
		{
			base.Room.tr_player_add_check -= this.trOnPlayerAddCheck;
			base.Room.tr_player_added -= this.trOnPlayerAdded;
			base.Room.tr_player_removed -= this.trOnPlayerRemoved;
			base.Room.GetExtension<KickExtension>().tr_player_kick -= this.trOnPlayerKick;
			base.OnDisposing();
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x000833C0 File Offset: 0x000817C0
		private GameRoomRetCode trOnPlayerAddCheck(RoomPlayer player)
		{
			if (!player.IsInClan())
			{
				return GameRoomRetCode.NOT_IN_CLAN;
			}
			ClanWar state = base.Room.GetState<ClanWar>(AccessMode.ReadOnly);
			if (!string.IsNullOrEmpty(state.Clan1) && !string.IsNullOrEmpty(state.Clan2) && !player.IsInClan(state.Clan1) && !player.IsInClan(state.Clan2))
			{
				return GameRoomRetCode.NOT_PARTICIPATE_IN_CLAN_WAR;
			}
			CoreState state2 = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			int num = base.Room.MaxPlayers / 2;
			int teamId = this.DetermineTeamId(player, state);
			int num2 = state2.Players.Values.Count((RoomPlayer p) => p.TeamID == teamId);
			return (num2 < num) ? GameRoomRetCode.OK : GameRoomRetCode.FULL;
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x0008348C File Offset: 0x0008188C
		private void trOnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			RoomPlayer player = base.Room.GetPlayer(profileId);
			ClanWar state = base.Room.GetState<ClanWar>(AccessMode.ReadWrite);
			string clanName = player.GetClanName();
			int num = this.DetermineTeamId(player, state);
			if (num == 1 && string.IsNullOrEmpty(state.Clan1))
			{
				state.Clan1 = clanName;
			}
			else if (num == 2 && string.IsNullOrEmpty(state.Clan2))
			{
				state.Clan2 = clanName;
			}
			if (num == 0)
			{
				throw new ClanWarException(string.Format("Can't find team for player, nickname: {0}, profileId: {1}", player.Nickname, player.ProfileID));
			}
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x0008352C File Offset: 0x0008192C
		private int DetermineTeamId(RoomPlayer player, ClanWar clanWar)
		{
			int result = 0;
			if (player.IsInClan(clanWar.Clan1))
			{
				result = 1;
			}
			else if (player.IsInClan(clanWar.Clan2))
			{
				result = 2;
			}
			else if (string.IsNullOrEmpty(clanWar.Clan1))
			{
				result = 1;
			}
			else if (string.IsNullOrEmpty(clanWar.Clan2))
			{
				result = 2;
			}
			return result;
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x00083598 File Offset: 0x00081998
		private void trOnPlayerRemoved(RoomPlayer player, GameRoomPlayerRemoveReason reason)
		{
			bool[] array = new bool[2];
			CoreState state = base.Room.GetState<CoreState>(AccessMode.ReadOnly);
			ClanWar state2 = base.Room.GetState<ClanWar>(AccessMode.ReadWrite);
			foreach (RoomPlayer roomPlayer in state.Players.Values)
			{
				array[0] |= roomPlayer.IsInClan(state2.Clan1);
				array[1] |= roomPlayer.IsInClan(state2.Clan2);
				if (array[0] && array[1])
				{
					break;
				}
			}
			if (!array[0])
			{
				state2.Clan1 = null;
			}
			if (!array[1])
			{
				state2.Clan2 = null;
			}
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x00083678 File Offset: 0x00081A78
		private void trOnPlayerKick(RoomPlayer p, GameRoomPlayerRemoveReason reason)
		{
			if (reason != GameRoomPlayerRemoveReason.KickMaster || !base.Room.IsClanWarMode() || base.Room.GameRunning)
			{
				return;
			}
			Dictionary<ulong, RoomPlayer> players = base.Room.GetState<CoreState>(AccessMode.ReadWrite).Players;
			foreach (RoomPlayer roomPlayer in players.Values)
			{
				roomPlayer.RoomStatus = ((roomPlayer.RoomStatus == RoomPlayer.EStatus.CantBeReady) ? RoomPlayer.EStatus.CantBeReady : RoomPlayer.EStatus.NotReady);
			}
			base.Room.SignalPlayersChanged();
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x0008372C File Offset: 0x00081B2C
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			ClanWar clanWar = (ClanWar)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("clan_war");
			ClanWarExtension.SerializeClanWar(xmlElement, clanWar);
			return xmlElement;
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x0008375E File Offset: 0x00081B5E
		public static void SerializeClanWar(XmlElement el, ClanWar clanWar)
		{
			el.SetAttribute("clan_1", clanWar.Clan1 ?? string.Empty);
			el.SetAttribute("clan_2", clanWar.Clan2 ?? string.Empty);
		}
	}
}
