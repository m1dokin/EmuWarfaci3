using System;
using System.Text;
using MasterServer.Core;

namespace MasterServer.DAL
{
	// Token: 0x02000072 RID: 114
	[Serializable]
	public class SItem
	{
		// Token: 0x17000024 RID: 36
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00004824 File Offset: 0x00002C24
		public bool IsBoosterItem
		{
			get
			{
				return this.Type == "booster";
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x0600013C RID: 316 RVA: 0x00004836 File Offset: 0x00002C36
		public bool IsCoinItem
		{
			get
			{
				return this.Type == "coin";
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x0600013D RID: 317 RVA: 0x00004848 File Offset: 0x00002C48
		public bool IsAttachmentItem
		{
			get
			{
				return this.Type == "attachment";
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600013E RID: 318 RVA: 0x0000485A File Offset: 0x00002C5A
		public bool IsAccessItem
		{
			get
			{
				return this.Type == "mission_access";
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000486C File Offset: 0x00002C6C
		public override int GetHashCode()
		{
			CRC32 crc = new CRC32();
			crc.GetHash(Encoding.ASCII.GetBytes(this.Name));
			return this.ID.GetHashCode() ^ (int)crc.CRCVal ^ this.Locked.GetHashCode() ^ this.MaxAmount.GetHashCode();
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000048D4 File Offset: 0x00002CD4
		public override string ToString()
		{
			return string.Format("Id: {0}, Name: {1}, Type: {2}, Slots: {3}, Active: {4}, Locked: {5}, Max amount : {6}", new object[]
			{
				this.ID,
				this.Name,
				this.Type,
				this.Slots,
				this.Active,
				this.Locked,
				this.MaxAmount
			});
		}

		// Token: 0x04000128 RID: 296
		public ulong ID;

		// Token: 0x04000129 RID: 297
		public string Name;

		// Token: 0x0400012A RID: 298
		public string Slots;

		// Token: 0x0400012B RID: 299
		public bool Active;

		// Token: 0x0400012C RID: 300
		public bool Locked;

		// Token: 0x0400012D RID: 301
		public bool ShopContent;

		// Token: 0x0400012E RID: 302
		public string Type;

		// Token: 0x0400012F RID: 303
		public int MaxAmount;
	}
}
