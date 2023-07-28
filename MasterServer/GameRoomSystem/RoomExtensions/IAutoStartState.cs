using System;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x02000519 RID: 1305
	internal interface IAutoStartState
	{
		// Token: 0x06001C56 RID: 7254
		void OnEnterState();

		// Token: 0x06001C57 RID: 7255
		void OnExitState();

		// Token: 0x06001C58 RID: 7256
		void OnPlayerAdded(ulong profileId);

		// Token: 0x06001C59 RID: 7257
		void OnPlayerRemoved(RoomPlayer player);

		// Token: 0x06001C5A RID: 7258
		void OnSessionStarted(string session_id);

		// Token: 0x06001C5B RID: 7259
		void OnSessionEnded(string session_id);

		// Token: 0x06001C5C RID: 7260
		void OnSessionStartFailed();

		// Token: 0x06001C5D RID: 7261
		void OnManualStart();
	}
}
