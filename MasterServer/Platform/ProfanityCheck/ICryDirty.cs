using System;
using System.Text;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006A6 RID: 1702
	[Contract]
	[BootstrapExplicit]
	internal interface ICryDirty
	{
		// Token: 0x060023BD RID: 9149
		void Init();

		// Token: 0x060023BE RID: 9150
		void Free();

		// Token: 0x060023BF RID: 9151
		bool ReadProfanityDict(string path);

		// Token: 0x060023C0 RID: 9152
		bool ReadReservedDict(string path);

		// Token: 0x060023C1 RID: 9153
		bool HaveProfanityWords(string message);

		// Token: 0x060023C2 RID: 9154
		bool HaveReservedWords(string message);

		// Token: 0x060023C3 RID: 9155
		bool FilterMessage(StringBuilder message);
	}
}
