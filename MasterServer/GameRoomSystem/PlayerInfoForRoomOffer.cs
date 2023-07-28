using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000528 RID: 1320
	public struct PlayerInfoForRoomOffer
	{
		// Token: 0x17000311 RID: 785
		// (get) Token: 0x06001CA9 RID: 7337 RVA: 0x00072C88 File Offset: 0x00071088
		public bool IsNicknameUsed
		{
			get
			{
				return this.ProfileId == 0UL && !string.IsNullOrEmpty(this.Nickname);
			}
		}

		// Token: 0x04000DA0 RID: 3488
		public ulong ProfileId;

		// Token: 0x04000DA1 RID: 3489
		public string Nickname;

		// Token: 0x04000DA2 RID: 3490
		public string GroupId;
	}
}
