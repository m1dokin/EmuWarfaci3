using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x0200001A RID: 26
	[Serializable]
	public class ClanMember
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00002BF0 File Offset: 0x00000FF0
		public override string ToString()
		{
			return string.Format("ClanID={0}, ProfileID={1}, Nickname={2}, ClanPoints={3}, Expirience={4}, InviteDate={5}, ClanRole={6}", new object[]
			{
				this.ClanID,
				this.ProfileID,
				this.Nickname,
				this.ClanPoints,
				this.Expirience,
				TimeUtils.UTCTimestampToUTCTime(this.InviteDate),
				this.ClanRole
			});
		}

		// Token: 0x0400003E RID: 62
		public ulong ClanID;

		// Token: 0x0400003F RID: 63
		public ulong ProfileID;

		// Token: 0x04000040 RID: 64
		public string Nickname;

		// Token: 0x04000041 RID: 65
		public ulong ClanPoints;

		// Token: 0x04000042 RID: 66
		public ulong Expirience;

		// Token: 0x04000043 RID: 67
		public ulong InviteDate;

		// Token: 0x04000044 RID: 68
		public EClanRole ClanRole;
	}
}
