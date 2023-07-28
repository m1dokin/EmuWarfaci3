using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.DAL.VoucherSystem;
using Util.Common;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x0200047B RID: 1147
	public class Voucher : IEnumerable<VoucherItem>, IEnumerable
	{
		// Token: 0x06001824 RID: 6180 RVA: 0x00063D10 File Offset: 0x00062110
		public Voucher(ulong id, ulong userId, string message, IEnumerable<VoucherItem> items) : this(id, userId, VoucherStatus.New, message, items)
		{
		}

		// Token: 0x06001825 RID: 6181 RVA: 0x00063D1E File Offset: 0x0006211E
		public Voucher(ulong id, ulong userId, VoucherStatus status, string message, IEnumerable<VoucherItem> items)
		{
			this.Id = id;
			this.UserId = userId;
			this.Status = status;
			this.Message = message;
			this.m_voucherItems = items;
		}

		// Token: 0x17000255 RID: 597
		// (get) Token: 0x06001826 RID: 6182 RVA: 0x00063D4B File Offset: 0x0006214B
		// (set) Token: 0x06001827 RID: 6183 RVA: 0x00063D53 File Offset: 0x00062153
		public ulong Id { get; private set; }

		// Token: 0x17000256 RID: 598
		// (get) Token: 0x06001828 RID: 6184 RVA: 0x00063D5C File Offset: 0x0006215C
		// (set) Token: 0x06001829 RID: 6185 RVA: 0x00063D64 File Offset: 0x00062164
		public ulong UserId { get; private set; }

		// Token: 0x17000257 RID: 599
		// (get) Token: 0x0600182A RID: 6186 RVA: 0x00063D6D File Offset: 0x0006216D
		// (set) Token: 0x0600182B RID: 6187 RVA: 0x00063D75 File Offset: 0x00062175
		public VoucherStatus Status { get; set; }

		// Token: 0x17000258 RID: 600
		// (get) Token: 0x0600182C RID: 6188 RVA: 0x00063D7E File Offset: 0x0006217E
		// (set) Token: 0x0600182D RID: 6189 RVA: 0x00063D86 File Offset: 0x00062186
		public string Message { get; set; }

		// Token: 0x0600182E RID: 6190 RVA: 0x00063D90 File Offset: 0x00062190
		public override bool Equals(object obj)
		{
			if (!(obj is Voucher))
			{
				return false;
			}
			Voucher other = (Voucher)obj;
			bool seed = this.Id == other.Id && this.UserId == other.UserId && this.Count<VoucherItem>() == other.Count<VoucherItem>();
			return this.m_voucherItems.Aggregate(seed, (bool current, VoucherItem item) => current && other.Any((VoucherItem x) => x.Equals(item)));
		}

		// Token: 0x0600182F RID: 6191 RVA: 0x00063E17 File Offset: 0x00062217
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06001830 RID: 6192 RVA: 0x00063E1F File Offset: 0x0006221F
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06001831 RID: 6193 RVA: 0x00063E27 File Offset: 0x00062227
		public IEnumerator<VoucherItem> GetEnumerator()
		{
			return this.m_voucherItems.GetEnumerator();
		}

		// Token: 0x06001832 RID: 6194 RVA: 0x00063E34 File Offset: 0x00062234
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder(string.Format("ID: {0}, UserID: {1}, Message: '{2}', Status: {3}", new object[]
			{
				this.Id,
				this.UserId,
				this.Message,
				this.Status
			}));
			if (this.m_voucherItems != null)
			{
				sb.Append(Environment.NewLine);
				sb.AppendLine("Voucher items:");
				this.m_voucherItems.ForEach(delegate(VoucherItem vi)
				{
					sb.AppendLine(vi.ToString());
				});
			}
			return sb.ToString();
		}

		// Token: 0x04000B9E RID: 2974
		private readonly IEnumerable<VoucherItem> m_voucherItems;
	}
}
