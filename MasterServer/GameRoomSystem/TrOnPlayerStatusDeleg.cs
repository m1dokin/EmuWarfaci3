using System;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200048B RID: 1163
	// (Invoke) Token: 0x0600186F RID: 6255
	internal delegate void TrOnPlayerStatusDeleg(ulong profileId, UserStatus oldStatus, UserStatus newStatus);
}
