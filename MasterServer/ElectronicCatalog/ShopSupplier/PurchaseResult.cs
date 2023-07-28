using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog.ShopSupplier
{
	// Token: 0x0200024D RID: 589
	public class PurchaseResult
	{
		// Token: 0x06000CF9 RID: 3321 RVA: 0x000322C3 File Offset: 0x000306C3
		public PurchaseResult() : this(TransactionStatus.INTERNAL_ERROR)
		{
		}

		// Token: 0x06000CFA RID: 3322 RVA: 0x000322CC File Offset: 0x000306CC
		public PurchaseResult(TransactionStatus status)
		{
			this.Status = status;
			this.CatalogItem = new List<CustomerItem>();
		}

		// Token: 0x06000CFB RID: 3323 RVA: 0x000322E6 File Offset: 0x000306E6
		public PurchaseResult(TransactionStatus status, IEnumerable<CustomerItem> catalogItem) : this(status)
		{
			if (catalogItem != null)
			{
				this.CatalogItem.AddRange(catalogItem);
			}
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06000CFC RID: 3324 RVA: 0x00032301 File Offset: 0x00030701
		// (set) Token: 0x06000CFD RID: 3325 RVA: 0x00032309 File Offset: 0x00030709
		public TransactionStatus Status { get; set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06000CFE RID: 3326 RVA: 0x00032312 File Offset: 0x00030712
		// (set) Token: 0x06000CFF RID: 3327 RVA: 0x0003231A File Offset: 0x0003071A
		public List<CustomerItem> CatalogItem { get; private set; }

		// Token: 0x06000D00 RID: 3328 RVA: 0x00032323 File Offset: 0x00030723
		public void AddRange(IEnumerable<CustomerItem> item)
		{
			this.CatalogItem.AddRange(item);
		}

		// Token: 0x06000D01 RID: 3329 RVA: 0x00032331 File Offset: 0x00030731
		public void Add(CustomerItem item)
		{
			this.CatalogItem.Add(item);
		}
	}
}
