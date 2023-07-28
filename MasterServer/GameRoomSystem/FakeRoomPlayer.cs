using System;
using MasterServer.GameLogic.MissionAccessLimitation;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000486 RID: 1158
	internal class FakeRoomPlayer : RoomPlayer
	{
		// Token: 0x0600185B RID: 6235 RVA: 0x0006533C File Offset: 0x0006373C
		public FakeRoomPlayer(int desiredTeamId)
		{
			if (desiredTeamId != 1)
			{
				if (desiredTeamId != 2)
				{
					throw new ArgumentException("Team id should be equal to 1 or 2");
				}
				this.m_clanName = "second_team_clan";
			}
			else
			{
				this.m_clanName = "first_team_clan";
			}
		}

		// Token: 0x0600185C RID: 6236 RVA: 0x0006538D File Offset: 0x0006378D
		public override string GetClanName()
		{
			return this.m_clanName;
		}

		// Token: 0x0600185D RID: 6237 RVA: 0x00065395 File Offset: 0x00063795
		public override bool CanJoinMission(IMissionAccessLimitationService limitationService, string missionName)
		{
			return true;
		}

		// Token: 0x04000BB2 RID: 2994
		private const string FirstTeamClanName = "first_team_clan";

		// Token: 0x04000BB3 RID: 2995
		private const string SecondTeamClanName = "second_team_clan";

		// Token: 0x04000BB4 RID: 2996
		private readonly string m_clanName;
	}
}
