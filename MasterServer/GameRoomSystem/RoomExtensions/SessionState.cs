using System;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000610 RID: 1552
	[RoomState(new Type[]
	{
		typeof(SessionExtension)
	})]
	internal class SessionState : RoomStateBase
	{
		// Token: 0x0600213D RID: 8509 RVA: 0x00088941 File Offset: 0x00086D41
		public void Clear()
		{
			this.Status = SessionStatus.None;
			this.SessionID = null;
			this.GameProgress = 0f;
			this.RewardsProcessed = false;
			this.EndedAbnormally = false;
		}

		// Token: 0x0400102C RID: 4140
		public SessionStatus Status;

		// Token: 0x0400102D RID: 4141
		public string SessionID;

		// Token: 0x0400102E RID: 4142
		public float GameProgress;

		// Token: 0x0400102F RID: 4143
		public bool RewardsProcessed;

		// Token: 0x04001030 RID: 4144
		public bool EndedAbnormally;

		// Token: 0x04001031 RID: 4145
		public DateTime SessionStartTime;

		// Token: 0x04001032 RID: 4146
		public int Team1StartScore;

		// Token: 0x04001033 RID: 4147
		public int Team2StartScore;
	}
}
