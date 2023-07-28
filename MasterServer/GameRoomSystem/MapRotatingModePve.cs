using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200049B RID: 1179
	internal class MapRotatingModePve : MapRotatingMode
	{
		// Token: 0x06001922 RID: 6434 RVA: 0x00066872 File Offset: 0x00064C72
		internal MapRotatingModePve(IMissionSystem missionSystem, IMatchmakingMissionsProvider matchmakingMissionsProvider, MapVotingParams pParams) : base(missionSystem, matchmakingMissionsProvider, pParams)
		{
		}

		// Token: 0x06001923 RID: 6435 RVA: 0x00066880 File Offset: 0x00064C80
		public override string ChooseWinner()
		{
			object @lock = this.m_lock;
			string result;
			lock (@lock)
			{
				string key = this.m_mapsForVote.Aggregate((KeyValuePair<string, int> l, KeyValuePair<string, int> r) => (l.Value < r.Value) ? r : l).Key;
				result = key;
			}
			return result;
		}

		// Token: 0x06001924 RID: 6436 RVA: 0x000668F4 File Offset: 0x00064CF4
		protected override void PrepareMissionsForVoting(MissionContext missionContext)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.m_playersVotes.Clear();
				this.m_mapsForVote.Clear();
				this.m_mapsForVote.Add(this.m_mission.uid, 0);
				this.m_missionsByMode = ((!this.IsSurvival(missionContext)) ? (from m in this.m_missionSystem.GetMatchmakingMissions()
				where m.IsPveMode() && !this.IsSurvival(m)
				select m into c
				orderby this.m_random.Next()
				select c) : this.m_missionSystem.GetMatchmakingMissions().Where((MissionContextBase m) => m.IsPveMode() && this.IsSurvival(m)).OrderBy((MissionContextBase c) => this.m_random.Next()));
				foreach (MissionContextBase missionContextBase in (from m in this.m_missionsByMode
				where !string.Equals(m.uid, this.m_mapsForVote.FirstOrDefault<KeyValuePair<string, int>>().Key)
				select m).Take(this.m_params.NewMaps))
				{
					this.m_mapsForVote.Add(missionContextBase.uid, 0);
				}
			}
		}

		// Token: 0x06001925 RID: 6437 RVA: 0x00066A5C File Offset: 0x00064E5C
		private bool IsSurvival(MissionContextBase mc)
		{
			return mc.missionType.IsSurvival();
		}
	}
}
