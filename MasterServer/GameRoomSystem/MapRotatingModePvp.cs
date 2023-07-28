using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200049A RID: 1178
	internal class MapRotatingModePvp : MapRotatingMode
	{
		// Token: 0x0600191E RID: 6430 RVA: 0x0006648D File Offset: 0x0006488D
		internal MapRotatingModePvp(IMissionSystem missionSystem, IMatchmakingMissionsProvider matchmakingMissionsProvider, MapVotingParams pParams) : base(missionSystem, matchmakingMissionsProvider, pParams)
		{
		}

		// Token: 0x0600191F RID: 6431 RVA: 0x00066498 File Offset: 0x00064898
		public override string ChooseWinner()
		{
			object @lock = this.m_lock;
			string result;
			lock (@lock)
			{
				string key = this.m_mapsForVote.Aggregate((KeyValuePair<string, int> l, KeyValuePair<string, int> r) => (l.Value <= r.Value) ? r : l).Key;
				if (string.Compare(key, this.m_mission.uid, StringComparison.InvariantCulture) == 0)
				{
					int votes = this.m_mapsForVote[key];
					string key2 = this.m_mapsForVote.FirstOrDefault((KeyValuePair<string, int> m) => m.Key != this.m_mission.uid && m.Value == votes).Key;
					if (!string.IsNullOrEmpty(key2))
					{
						return key2;
					}
				}
				result = key;
			}
			return result;
		}

		// Token: 0x06001920 RID: 6432 RVA: 0x0006657C File Offset: 0x0006497C
		protected override void PrepareMissionsForVoting(MissionContext missionContext)
		{
			IEnumerable<string> autostartMaps = this.m_matchmakingMissionsProvider.AutostartMissions;
			IEnumerable<MissionContextBase> source = from m in this.m_missionSystem.GetMatchmakingMissions()
			where autostartMaps.Contains(m.uid)
			select m;
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_playersVotes.Clear();
				this.m_mapsForVote.Clear();
				this.m_mapsForVote.Add(this.m_mission.uid, 0);
				if (!this.m_missionsByMode.Any<MissionContextBase>())
				{
					string[] limitRotatingMode = this.m_params.Mode.FirstOrDefault((string[] md) => md.Any((string m) => string.Compare(m, missionContext.gameMode, StringComparison.InvariantCulture) == 0));
					if (limitRotatingMode == null)
					{
						return;
					}
					this.m_missionsByMode = from m in source
					where limitRotatingMode.Contains(m.gameMode)
					select m;
				}
				this.m_missionsByMode = (from c in this.m_missionsByMode
				orderby this.m_random.Next(this.m_missionsByMode.Count<MissionContextBase>())
				select c).ToList<MissionContextBase>();
				if (base.IsVoteAvailableByNumMapsInMode)
				{
					foreach (MissionContextBase missionContextBase in (from m in this.m_missionsByMode
					where !string.Equals(m.uid, this.m_mapsForVote.FirstOrDefault<KeyValuePair<string, int>>().Key)
					select m).Take(this.m_params.NewMaps))
					{
						this.m_mapsForVote.Add(missionContextBase.uid, 0);
					}
				}
			}
		}
	}
}
