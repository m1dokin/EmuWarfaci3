using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004BE RID: 1214
	internal class PlayersInTeamPlayTime
	{
		// Token: 0x06001A47 RID: 6727 RVA: 0x0006C170 File Offset: 0x0006A570
		public PlayersInTeamPlayTime()
		{
			this.m_playersInTeam = new Dictionary<int, Dictionary<ulong, PlayerPlayTime>>
			{
				{
					0,
					new Dictionary<ulong, PlayerPlayTime>()
				},
				{
					1,
					new Dictionary<ulong, PlayerPlayTime>()
				},
				{
					2,
					new Dictionary<ulong, PlayerPlayTime>()
				}
			};
			this.m_playersCurrentTeam = new Dictionary<ulong, int>();
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x0006C1C0 File Offset: 0x0006A5C0
		public void AddPlayer(RoomPlayer player)
		{
			int teamID = player.TeamID;
			ulong profileID = player.ProfileID;
			Dictionary<ulong, PlayerPlayTime> dictionary = this.m_playersInTeam[teamID];
			object obj = dictionary;
			lock (obj)
			{
				if (!dictionary.ContainsKey(profileID))
				{
					PlayerPlayTime value = new PlayerPlayTime(profileID, player.Skill.Value);
					dictionary.Add(profileID, value);
				}
			}
			object playersCurrentTeam = this.m_playersCurrentTeam;
			lock (playersCurrentTeam)
			{
				this.m_playersCurrentTeam[profileID] = teamID;
			}
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x0006C27C File Offset: 0x0006A67C
		public void UpdatePlayer(ulong profileId, TimeSpan playTime)
		{
			object playersCurrentTeam = this.m_playersCurrentTeam;
			int key;
			lock (playersCurrentTeam)
			{
				if (!this.m_playersCurrentTeam.TryGetValue(profileId, out key))
				{
					return;
				}
			}
			Dictionary<ulong, PlayerPlayTime> dictionary = this.m_playersInTeam[key];
			object obj = dictionary;
			lock (obj)
			{
				dictionary[profileId].UpdatePlayTime(playTime);
			}
		}

		// Token: 0x06001A4A RID: 6730 RVA: 0x0006C318 File Offset: 0x0006A718
		public IEnumerable<PlayerPlayTime> GetPlayers(int teamId)
		{
			Dictionary<ulong, PlayerPlayTime> dictionary;
			if (!this.m_playersInTeam.TryGetValue(teamId, out dictionary))
			{
				throw new KeyNotFoundException(string.Format("Team with id {0} was not present in the dictionary.", teamId));
			}
			object obj = dictionary;
			IEnumerable<PlayerPlayTime> result;
			lock (obj)
			{
				result = dictionary.Values.ToList<PlayerPlayTime>();
			}
			return result;
		}

		// Token: 0x04000C96 RID: 3222
		private readonly Dictionary<int, Dictionary<ulong, PlayerPlayTime>> m_playersInTeam;

		// Token: 0x04000C97 RID: 3223
		private readonly Dictionary<ulong, int> m_playersCurrentTeam;
	}
}
