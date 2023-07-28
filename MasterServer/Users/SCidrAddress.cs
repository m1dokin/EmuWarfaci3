using System;
using System.Runtime.InteropServices;
using MasterServer.Core;

namespace MasterServer.Users
{
	// Token: 0x020007E2 RID: 2018
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SCidrAddress
	{
		// Token: 0x06002951 RID: 10577 RVA: 0x000B364C File Offset: 0x000B1A4C
		public SCidrAddress(string cidr)
		{
			this = default(SCidrAddress);
			string[] array = cidr.Split(new char[]
			{
				'/'
			});
			if (array.Length < 2)
			{
				throw new IncorrectIpAddressException(string.Format("IP mask {0} has incorrect format", cidr));
			}
			this.IP = array[0];
			this.Mask = int.Parse(array[1]);
		}

		// Token: 0x170003D9 RID: 985
		// (get) Token: 0x06002952 RID: 10578 RVA: 0x000B36A3 File Offset: 0x000B1AA3
		// (set) Token: 0x06002953 RID: 10579 RVA: 0x000B36AB File Offset: 0x000B1AAB
		public string IP { get; private set; }

		// Token: 0x170003DA RID: 986
		// (get) Token: 0x06002954 RID: 10580 RVA: 0x000B36B4 File Offset: 0x000B1AB4
		// (set) Token: 0x06002955 RID: 10581 RVA: 0x000B36BC File Offset: 0x000B1ABC
		public int Mask { get; private set; }

		// Token: 0x06002956 RID: 10582 RVA: 0x000B36C5 File Offset: 0x000B1AC5
		public override string ToString()
		{
			return string.Format("{0}/{1}", this.IP, this.Mask);
		}
	}
}
