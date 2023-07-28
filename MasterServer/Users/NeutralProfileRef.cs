using System;
using System.Collections.Generic;

namespace MasterServer.Users
{
	// Token: 0x020007EC RID: 2028
	public class NeutralProfileRef
	{
		// Token: 0x06002992 RID: 10642 RVA: 0x000B3881 File Offset: 0x000B1C81
		public NeutralProfileRef(string nick)
		{
			this.Nickname = nick;
		}

		// Token: 0x06002993 RID: 10643 RVA: 0x000B3890 File Offset: 0x000B1C90
		public NeutralProfileRef(ulong pid)
		{
			this.ProfileId = pid;
		}

		// Token: 0x170003DB RID: 987
		// (get) Token: 0x06002994 RID: 10644 RVA: 0x000B389F File Offset: 0x000B1C9F
		// (set) Token: 0x06002995 RID: 10645 RVA: 0x000B38A7 File Offset: 0x000B1CA7
		public string Nickname { get; private set; }

		// Token: 0x170003DC RID: 988
		// (get) Token: 0x06002996 RID: 10646 RVA: 0x000B38B0 File Offset: 0x000B1CB0
		// (set) Token: 0x06002997 RID: 10647 RVA: 0x000B38B8 File Offset: 0x000B1CB8
		public ulong ProfileId { get; private set; }

		// Token: 0x06002998 RID: 10648 RVA: 0x000B38C1 File Offset: 0x000B1CC1
		public bool Check(string nick, ulong pid)
		{
			return (this.ProfileId != 0UL && this.ProfileId == pid) || (!string.IsNullOrEmpty(this.Nickname) && this.Nickname.Equals(nick, StringComparison.InvariantCultureIgnoreCase));
		}

		// Token: 0x06002999 RID: 10649 RVA: 0x000B3900 File Offset: 0x000B1D00
		public override string ToString()
		{
			List<string> list = new List<string>(2);
			if (!string.IsNullOrEmpty(this.Nickname))
			{
				list.Add(this.Nickname);
			}
			if (this.ProfileId != 0UL)
			{
				list.Add(this.ProfileId.ToString());
			}
			return string.Join("#", list.ToArray());
		}
	}
}
