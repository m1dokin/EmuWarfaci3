using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HK2Net;
using HK2Net.Attributes.Bootstrap;

namespace MasterServer.GameLogic.VoucherSystem.VoucherProviders
{
	// Token: 0x02000475 RID: 1141
	[Service]
	[Singleton]
	[BootstrapSpecific("west")]
	[BootstrapSpecific("china")]
	[BootstrapSpecific("china_emul")]
	[BootstrapSpecific("row")]
	[BootstrapSpecific("row_emul")]
	[BootstrapSpecific("russia")]
	[BootstrapSpecific("russia_emul")]
	public class NullVoucherCommunicationClient : IPageVoucherCommunicationClient, IVoucherCommunicationClient, IDisposable
	{
		// Token: 0x060017FD RID: 6141 RVA: 0x00063329 File Offset: 0x00061729
		public void Dispose()
		{
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0006332B File Offset: 0x0006172B
		public Task<IEnumerable<Voucher>> GetVouchers(ulong pageIndex, int pageSize)
		{
			return Task.FromResult<IEnumerable<Voucher>>(Enumerable.Empty<Voucher>());
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x00063337 File Offset: 0x00061737
		public Task<IEnumerable<Voucher>> GetNewVouchers(ulong userId)
		{
			return Task.FromResult<IEnumerable<Voucher>>(Enumerable.Empty<Voucher>());
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x00063343 File Offset: 0x00061743
		public void UpdateVoucher(Voucher voucher)
		{
		}

		// Token: 0x06001801 RID: 6145 RVA: 0x00063345 File Offset: 0x00061745
		public IEnumerable<Voucher> GetAllVouchers(ulong userId)
		{
			return Enumerable.Empty<Voucher>();
		}
	}
}
