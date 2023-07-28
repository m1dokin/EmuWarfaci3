using System;
using System.Text;
using MasterServer.Core;

namespace MasterServer.DAL
{
	// Token: 0x02000063 RID: 99
	[Serializable]
	public struct CatalogItem
	{
		// Token: 0x060000E2 RID: 226 RVA: 0x000040B4 File Offset: 0x000024B4
		public override string ToString()
		{
			return string.Format("<CatalogItem> {0} [{1}] active: {2}, type {3}, stackable {4}", new object[]
			{
				this.Name,
				this.ID,
				this.Active,
				this.Type,
				this.Stackable
			});
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00004110 File Offset: 0x00002510
		public override int GetHashCode()
		{
			CRC32 crc = new CRC32();
			crc.GetHash(Encoding.ASCII.GetBytes(this.Name));
			return (int)crc.CRCVal;
		}

		// Token: 0x04000103 RID: 259
		public ulong ID;

		// Token: 0x04000104 RID: 260
		public string Name;

		// Token: 0x04000105 RID: 261
		public bool Active;

		// Token: 0x04000106 RID: 262
		public string Type;

		// Token: 0x04000107 RID: 263
		public bool Stackable;

		// Token: 0x04000108 RID: 264
		public int MaxAmount;
	}
}
