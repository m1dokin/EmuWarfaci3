using System;
using HK2Net;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x0200058C RID: 1420
	[Contract]
	internal interface IPunishmentService
	{
		// Token: 0x14000072 RID: 114
		// (add) Token: 0x06001E7A RID: 7802
		// (remove) Token: 0x06001E7B RID: 7803
		event Action<ulong, DateTime, string> PlayerBanned;

		// Token: 0x14000073 RID: 115
		// (add) Token: 0x06001E7C RID: 7804
		// (remove) Token: 0x06001E7D RID: 7805
		event Action<ulong> PlayerUnBanned;

		// Token: 0x06001E7E RID: 7806
		bool IsBanned(IProfileProxy profile);

		// Token: 0x06001E7F RID: 7807
		void BanPlayer(ulong profileId, TimeSpan time, string message, BanReportSource source);

		// Token: 0x06001E80 RID: 7808
		void UnBanPlayer(ulong profileId);

		// Token: 0x06001E81 RID: 7809
		void MutePlayer(ulong profileId, TimeSpan time);

		// Token: 0x06001E82 RID: 7810
		void UnMute(ulong profileId);

		// Token: 0x06001E83 RID: 7811
		void KickPlayer(ulong profileId);

		// Token: 0x06001E84 RID: 7812
		bool KickPlayerLocal(ulong profileId, GameRoomPlayerRemoveReason reason);

		// Token: 0x06001E85 RID: 7813
		void ForceLogout(ulong profileId);

		// Token: 0x06001E86 RID: 7814
		string MakeScreenShot(ulong profileId, bool frontBuffer, int count, float scaleWidth, float scaleHeight);

		// Token: 0x06001E87 RID: 7815
		string MakeScreenShot(ulong profileId, bool frontBuffer, int count, float scaleWidth, float scaleHeight, long screenShotKey, string initiator);

		// Token: 0x06001E88 RID: 7816
		void OnScreenShotResult(long screenshotKey, string result);

		// Token: 0x06001E89 RID: 7817
		bool CanKick(ulong profileId);
	}
}
