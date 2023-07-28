using System;
using System.Collections.Generic;
using System.Text;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002DF RID: 735
	public struct SProfileInfoEx
	{
		// Token: 0x06001028 RID: 4136 RVA: 0x0003F170 File Offset: 0x0003D570
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("UserId {0}\n", this.UserId);
			stringBuilder.AppendFormat("ProfileId {0}\n", this.ProfileId);
			stringBuilder.AppendFormat("Nickname {0}\n", this.Nickname);
			stringBuilder.AppendFormat("Gender {0}\n", this.Gender);
			stringBuilder.AppendFormat("GameMoney {0}\n", this.GameMoney);
			stringBuilder.AppendFormat("CrownMoney {0}\n", this.CrownMoney);
			stringBuilder.AppendFormat("CryMoney {0}\n", this.CryMoney);
			stringBuilder.AppendFormat("Experience {0}\n", this.Experience);
			stringBuilder.AppendFormat("RankId {0}\n", this.RankId);
			int num = 1;
			foreach (int num2 in this.SponsorStages)
			{
				stringBuilder.AppendFormat("SponsorStage{0} {1}\n", num++, num2);
			}
			num = 1;
			foreach (ulong num3 in this.SponsorPoints)
			{
				stringBuilder.AppendFormat("SponsorStage{0} {1}\n", num++, num3);
			}
			stringBuilder.AppendFormat("CreationTime {0}\n", this.CreationTime);
			stringBuilder.AppendFormat("LastSeenTime {0}\n", this.LastSeenTime);
			stringBuilder.AppendFormat("IsOnline {0}\n", this.IsOnline);
			stringBuilder.AppendFormat("PunishmentStatus {0}\n", this.PunishmentStatus);
			stringBuilder.AppendFormat("BanEndTime {0}\n", this.BanEndTime);
			stringBuilder.AppendFormat("MuteEndTime {0}\n", this.MuteEndTime);
			stringBuilder.AppendFormat("ClanId {0}\n", this.ClanId);
			stringBuilder.AppendFormat("ClanName {0}\n", this.ClanName);
			return stringBuilder.ToString();
		}

		// Token: 0x04000759 RID: 1881
		public ulong UserId;

		// Token: 0x0400075A RID: 1882
		public ulong ProfileId;

		// Token: 0x0400075B RID: 1883
		public string Nickname;

		// Token: 0x0400075C RID: 1884
		public string Gender;

		// Token: 0x0400075D RID: 1885
		public ulong GameMoney;

		// Token: 0x0400075E RID: 1886
		public ulong CrownMoney;

		// Token: 0x0400075F RID: 1887
		public ulong CryMoney;

		// Token: 0x04000760 RID: 1888
		public ulong Experience;

		// Token: 0x04000761 RID: 1889
		public int RankId;

		// Token: 0x04000762 RID: 1890
		public List<ulong> SponsorPoints;

		// Token: 0x04000763 RID: 1891
		public List<int> SponsorStages;

		// Token: 0x04000764 RID: 1892
		public DateTime CreationTime;

		// Token: 0x04000765 RID: 1893
		public DateTime LastSeenTime;

		// Token: 0x04000766 RID: 1894
		public bool IsOnline;

		// Token: 0x04000767 RID: 1895
		public PunishmentStatus PunishmentStatus;

		// Token: 0x04000768 RID: 1896
		public DateTime BanEndTime;

		// Token: 0x04000769 RID: 1897
		public DateTime MuteEndTime;

		// Token: 0x0400076A RID: 1898
		public ulong ClanId;

		// Token: 0x0400076B RID: 1899
		public string ClanName;
	}
}
