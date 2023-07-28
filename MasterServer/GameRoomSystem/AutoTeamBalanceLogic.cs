using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004A1 RID: 1185
	internal abstract class AutoTeamBalanceLogic : IAutoTeamBalanceLogic
	{
		// Token: 0x06001937 RID: 6455 RVA: 0x00060FCC File Offset: 0x0005F3CC
		protected AutoTeamBalanceLogic(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom").GetSection("AutoTeamBalance");
			this.m_useGroups = (int.Parse(section.Get("use_groups")) > 0);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06001938 RID: 6456 RVA: 0x0006102C File Offset: 0x0005F42C
		public bool CanBalancePlayers(IGameRoom room, IEnumerable<RoomPlayer> players)
		{
			Dictionary<int, TeamInfo> dictionary = this.CreateTeams(room.MaxTeamSize, players);
			TeamInfo firstTeam = dictionary[1];
			TeamInfo secondTeam = dictionary[2];
			IEnumerable<RoomPlayer> players2 = from x in players
			where !firstTeam.HasPlayer(x) && !secondTeam.HasPlayer(x)
			select x;
			this.PutGroupsIntoTeams(room, dictionary, players2);
			return !players.Any((RoomPlayer x) => !firstTeam.HasPlayer(x) && !secondTeam.HasPlayer(x));
		}

		// Token: 0x06001939 RID: 6457 RVA: 0x00061098 File Offset: 0x0005F498
		public Dictionary<int, TeamInfo> BalancePlayers(IGameRoom room, IEnumerable<RoomPlayer> players)
		{
			Dictionary<int, TeamInfo> dictionary = this.CreateTeams(room.MaxTeamSize, players);
			TeamInfo firstTeam = dictionary[1];
			TeamInfo secondTeam = dictionary[2];
			bool flag = this.UseGroups(room);
			IEnumerable<RoomPlayer> enumerable = from x in players
			where !firstTeam.HasPlayer(x) && !secondTeam.HasPlayer(x)
			select x;
			this.PutGroupsIntoTeams(room, dictionary, enumerable);
			enumerable = from x in players
			where !firstTeam.HasPlayer(x) && !secondTeam.HasPlayer(x)
			select x;
			if (enumerable.Any<RoomPlayer>())
			{
				if (flag)
				{
					StringBuilder stringBuilder = new StringBuilder(string.Format("AutoTeamBalanceLogic using fallback strategy and splitting groups for room {0}\n", room.ID));
					foreach (RoomPlayer roomPlayer in players)
					{
						stringBuilder.AppendLine(roomPlayer.ToString());
					}
					Log.Warning(stringBuilder.ToString());
				}
				foreach (RoomPlayer item in enumerable)
				{
					this.PutGroupsIntoTeams(room, dictionary, new List<RoomPlayer>
					{
						item
					});
				}
			}
			return dictionary;
		}

		// Token: 0x0600193A RID: 6458 RVA: 0x000611F8 File Offset: 0x0005F5F8
		public int ChooseTeam(RoomPlayer player, IGameRoom room, IEnumerable<RoomPlayer> players)
		{
			if (player.TeamID != 0)
			{
				throw new ArgumentException(string.Format("Room {0}, Player {1} already has team {2}", room.ID, player.ProfileID, player.TeamID));
			}
			Dictionary<int, TeamInfo> dictionary = this.CreateTeams(room.MaxTeamSize, players);
			this.PutGroupsIntoTeams(room, dictionary, new List<RoomPlayer>
			{
				player
			});
			return dictionary.FirstOrDefault((KeyValuePair<int, TeamInfo> x) => x.Value.HasPlayer(player)).Key;
		}

		// Token: 0x0600193B RID: 6459
		protected abstract void PutGroupsIntoTeams(IGameRoom room, Dictionary<int, TeamInfo> teamInfo, IEnumerable<RoomPlayer> players);

		// Token: 0x0600193C RID: 6460 RVA: 0x000612A4 File Offset: 0x0005F6A4
		protected IEnumerable<GroupInfo> CreateGroups(bool useGroups, IEnumerable<RoomPlayer> players)
		{
			Dictionary<string, GroupInfo> dictionary = new Dictionary<string, GroupInfo>();
			foreach (RoomPlayer roomPlayer in players)
			{
				GroupInfo groupInfo;
				if (!roomPlayer.HasGroup || !useGroups)
				{
					groupInfo = new GroupInfo(roomPlayer);
					dictionary.Add(roomPlayer.UserID.ToString(CultureInfo.InvariantCulture), groupInfo);
				}
				else if (!dictionary.TryGetValue(roomPlayer.GroupID, out groupInfo))
				{
					groupInfo = new GroupInfo(roomPlayer);
					dictionary.Add(roomPlayer.GroupID, groupInfo);
				}
				else
				{
					groupInfo.Add(roomPlayer);
				}
			}
			return dictionary.Values;
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00061364 File Offset: 0x0005F764
		protected bool UseGroups(IGameRoom room)
		{
			ServerExtension extension = room.GetExtension<ServerExtension>();
			MissionState state = room.GetState<MissionState>(AccessMode.ReadOnly);
			GameModeSetting gameModeSetting = this.m_gameModesSystem.GetGameModeSetting(state.Mission.gameMode);
			bool flag = false;
			if (gameModeSetting != null)
			{
				gameModeSetting.GetSetting(room.Type, ERoomSetting.AUTOBALANCE_GROUP_MODE, out flag);
			}
			return this.m_useGroups && flag && !extension.GameRunning;
		}

		// Token: 0x0600193E RID: 6462 RVA: 0x000613CC File Offset: 0x0005F7CC
		private Dictionary<int, TeamInfo> CreateTeams(int teamSize, IEnumerable<RoomPlayer> players)
		{
			Dictionary<int, TeamInfo> dictionary = new Dictionary<int, TeamInfo>
			{
				{
					1,
					new TeamInfo(teamSize, 1)
				},
				{
					2,
					new TeamInfo(teamSize, 2)
				}
			};
			foreach (RoomPlayer roomPlayer in players)
			{
				if (roomPlayer.TeamID != 0)
				{
					TeamInfo teamInfo;
					if (!dictionary.TryGetValue(roomPlayer.TeamID, out teamInfo))
					{
						throw new KeyNotFoundException(string.Format("Player {0} has incorrect team", roomPlayer));
					}
					teamInfo.AddPlayer(roomPlayer);
				}
			}
			return dictionary;
		}

		// Token: 0x0600193F RID: 6463 RVA: 0x0006147C File Offset: 0x0005F87C
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (args.Name.Equals("use_groups", StringComparison.InvariantCultureIgnoreCase))
			{
				this.m_useGroups = (args.iValue > 0);
			}
		}

		// Token: 0x04000C0D RID: 3085
		private const string UseGroupsConfig = "use_groups";

		// Token: 0x04000C0E RID: 3086
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x04000C0F RID: 3087
		private bool m_useGroups;
	}
}
