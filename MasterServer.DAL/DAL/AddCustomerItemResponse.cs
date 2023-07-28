using System;
using System.Collections.Generic;

namespace MasterServer.DAL
{
	// Token: 0x0200005B RID: 91
	[Serializable]
	public struct AddCustomerItemResponse
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00003E1A File Offset: 0x0000221A
		public override string ToString()
		{
			return string.Format("<AddCustomerItemResponse> {0}", this.Status);
		}

		// Token: 0x040000F2 RID: 242
		public TransactionStatus Status;

		// Token: 0x040000F3 RID: 243
		public List<ulong> Items;
	}
}
