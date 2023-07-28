using System;
using System.Collections.Generic;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020004B1 RID: 1201
	internal struct MapVotingParams
	{
		// Token: 0x06001983 RID: 6531 RVA: 0x000679B4 File Offset: 0x00065DB4
		internal MapVotingParams(HashSet<string[]> mode, bool active, int newMaps, TimeSpan votingTime)
		{
			this.Mode = mode;
			this.Enabled = active;
			this.NewMaps = newMaps;
			this.VotingTime = votingTime;
		}

		// Token: 0x04000C2F RID: 3119
		internal bool Enabled;

		// Token: 0x04000C30 RID: 3120
		internal int NewMaps;

		// Token: 0x04000C31 RID: 3121
		internal TimeSpan VotingTime;

		// Token: 0x04000C32 RID: 3122
		internal HashSet<string[]> Mode;
	}
}
