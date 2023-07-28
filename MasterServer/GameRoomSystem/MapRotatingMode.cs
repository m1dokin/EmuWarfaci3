using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Matchmaking;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020004B4 RID: 1204
	internal abstract class MapRotatingMode : IMapRotatingMode
	{
		// Token: 0x060019A9 RID: 6569 RVA: 0x00066288 File Offset: 0x00064688
		protected MapRotatingMode(IMissionSystem missionSystem, IMatchmakingMissionsProvider matchmakingMissionsProvider, MapVotingParams pParams)
		{
			this.m_params = pParams;
			this.m_missionSystem = missionSystem;
			this.m_matchmakingMissionsProvider = matchmakingMissionsProvider;
		}

		// Token: 0x060019AA RID: 6570
		public abstract string ChooseWinner();

		// Token: 0x060019AB RID: 6571
		protected abstract void PrepareMissionsForVoting(MissionContext missionContext);

		// Token: 0x060019AC RID: 6572 RVA: 0x00066302 File Offset: 0x00064702
		public void OnSetMissionEnded(MissionContext mission)
		{
			this.m_mission = mission;
			this.PrepareMissionsForVoting(this.m_mission);
		}

		// Token: 0x17000281 RID: 641
		// (get) Token: 0x060019AD RID: 6573 RVA: 0x00066317 File Offset: 0x00064717
		public bool IsVoteAvailable
		{
			get
			{
				return this.m_params.Enabled && this.IsVoteAvailableByNumMapsInMode;
			}
		}

		// Token: 0x17000282 RID: 642
		// (get) Token: 0x060019AE RID: 6574 RVA: 0x00066334 File Offset: 0x00064734
		public IEnumerable<string> GetRotatingMaps
		{
			get
			{
				object @lock = this.m_lock;
				IEnumerable<string> result;
				lock (@lock)
				{
					result = new List<string>(this.m_mapsForVote.Keys);
				}
				return result;
			}
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x00066384 File Offset: 0x00064784
		public bool TryCountVote(ulong profileId, string mission_uid)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (!this.m_mapsForVote.ContainsKey(mission_uid))
				{
					return false;
				}
				string text;
				Dictionary<string, int> mapsForVote;
				if (this.m_playersVotes.TryGetValue(profileId, out text))
				{
					string key;
					(mapsForVote = this.m_mapsForVote)[key = text] = mapsForVote[key] - 1;
				}
				this.m_playersVotes[profileId] = mission_uid;
				(mapsForVote = this.m_mapsForVote)[mission_uid] = mapsForVote[mission_uid] + 1;
			}
			return true;
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x00066438 File Offset: 0x00064838
		public IDictionary<string, int> DumpVotingState()
		{
			object @lock = this.m_lock;
			IDictionary<string, int> mapsForVote;
			lock (@lock)
			{
				mapsForVote = this.m_mapsForVote;
			}
			return mapsForVote;
		}

		// Token: 0x17000283 RID: 643
		// (get) Token: 0x060019B1 RID: 6577 RVA: 0x00066480 File Offset: 0x00064880
		protected bool IsVoteAvailableByNumMapsInMode
		{
			get
			{
				return this.m_missionsByMode.Any<MissionContextBase>();
			}
		}

		// Token: 0x04000C44 RID: 3140
		protected IMatchmakingMissionsProvider m_matchmakingMissionsProvider;

		// Token: 0x04000C45 RID: 3141
		protected IMissionSystem m_missionSystem;

		// Token: 0x04000C46 RID: 3142
		protected MapVotingParams m_params;

		// Token: 0x04000C47 RID: 3143
		protected object m_lock = new object();

		// Token: 0x04000C48 RID: 3144
		protected Random m_random = new Random(DateTime.UtcNow.Ticks.GetHashCode());

		// Token: 0x04000C49 RID: 3145
		protected MissionContext m_mission;

		// Token: 0x04000C4A RID: 3146
		protected IEnumerable<MissionContextBase> m_missionsByMode = new HashSet<MissionContextBase>();

		// Token: 0x04000C4B RID: 3147
		protected readonly Dictionary<string, int> m_mapsForVote = new Dictionary<string, int>();

		// Token: 0x04000C4C RID: 3148
		protected readonly Dictionary<ulong, string> m_playersVotes = new Dictionary<ulong, string>();
	}
}
