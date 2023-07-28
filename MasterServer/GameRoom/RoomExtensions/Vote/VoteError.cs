using System;

namespace MasterServer.GameRoom.RoomExtensions.Vote
{
	// Token: 0x020004E9 RID: 1257
	internal enum VoteError
	{
		// Token: 0x04000CED RID: 3309
		None,
		// Token: 0x04000CEE RID: 3310
		VoteInProgress,
		// Token: 0x04000CEF RID: 3311
		NotEnoughPlayers,
		// Token: 0x04000CF0 RID: 3312
		VoteTimeout,
		// Token: 0x04000CF1 RID: 3313
		InvalidInitiator,
		// Token: 0x04000CF2 RID: 3314
		InvalidTarget,
		// Token: 0x04000CF3 RID: 3315
		RoomNotFound
	}
}
