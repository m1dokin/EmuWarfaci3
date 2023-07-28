using System;
using System.Collections.Generic;
using MasterServer.GameLogic.GameInterface;

namespace MasterServer.Users
{
	// Token: 0x020007E3 RID: 2019
	public struct SUserAccessLevel
	{
		// Token: 0x06002957 RID: 10583 RVA: 0x000B36E2 File Offset: 0x000B1AE2
		public SUserAccessLevel(ulong dbID, ulong userID, AccessLevel accessLevel, string ipMask)
		{
			this.db_id = dbID;
			this.user_id = userID;
			this.accessLevel = accessLevel;
			this.ip_mask = new SCidrAddress(ipMask);
		}

		// Token: 0x06002958 RID: 10584 RVA: 0x000B3708 File Offset: 0x000B1B08
		public bool CheckPrivileges(string ip_addr)
		{
			if (this.accessLevel.HasFlag(AccessLevel.Debug))
			{
				return true;
			}
			uint num = (this.ip_mask.Mask == 0) ? 0U : (~((1U << 32 - this.ip_mask.Mask) - 1U));
			uint num2 = this.IP2UInt(ip_addr);
			uint num3 = this.IP2UInt(this.ip_mask.IP) & num;
			return (num & num2) == num3;
		}

		// Token: 0x06002959 RID: 10585 RVA: 0x000B3788 File Offset: 0x000B1B88
		public static SUserAccessLevel GetUserAccessLevel(ulong userID, string ipAddress, IEnumerable<SUserAccessLevel> accessLevelList)
		{
			SUserAccessLevel result = new SUserAccessLevel(0UL, userID, AccessLevel.Basic, "0.0.0.0/0");
			foreach (SUserAccessLevel suserAccessLevel in accessLevelList)
			{
				if (suserAccessLevel.CheckPrivileges(ipAddress) && result.accessLevel < suserAccessLevel.accessLevel)
				{
					result = suserAccessLevel;
				}
			}
			return result;
		}

		// Token: 0x0600295A RID: 10586 RVA: 0x000B3808 File Offset: 0x000B1C08
		private uint IP2UInt(string ip)
		{
			double num = 0.0;
			if (!string.IsNullOrEmpty(ip))
			{
				string[] array = ip.Split(new char[]
				{
					'.'
				});
				for (int i = array.Length - 1; i >= 0; i--)
				{
					num += (double)(int.Parse(array[i]) % 256) * Math.Pow(256.0, (double)(3 - i));
				}
			}
			return (uint)num;
		}

		// Token: 0x04001601 RID: 5633
		public const string UndefinedIp = "0.0.0.0";

		// Token: 0x04001602 RID: 5634
		public ulong db_id;

		// Token: 0x04001603 RID: 5635
		public ulong user_id;

		// Token: 0x04001604 RID: 5636
		public AccessLevel accessLevel;

		// Token: 0x04001605 RID: 5637
		public SCidrAddress ip_mask;
	}
}
