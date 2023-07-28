using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Core;

namespace MasterServer.DAL
{
	// Token: 0x02000064 RID: 100
	[Serializable]
	public class StoreOffer
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x00004141 File Offset: 0x00002541
		public StoreOffer()
		{
			this.OriginalPrices = new List<PriceTag>();
			this.Prices = new List<PriceTag>();
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000415F File Offset: 0x0000255F
		// (set) Token: 0x060000E6 RID: 230 RVA: 0x00004167 File Offset: 0x00002567
		public List<PriceTag> Prices { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000E7 RID: 231 RVA: 0x00004170 File Offset: 0x00002570
		// (set) Token: 0x060000E8 RID: 232 RVA: 0x00004178 File Offset: 0x00002578
		public List<PriceTag> OriginalPrices { get; private set; }

		// Token: 0x060000E9 RID: 233 RVA: 0x00004184 File Offset: 0x00002584
		public ulong GetPriceByCurrency(Currency currency)
		{
			return this.GetPriceTagByCurrency(currency).Price;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x000041A0 File Offset: 0x000025A0
		public ulong GetOriginalPriceByCurrency(Currency currency)
		{
			return this.GetOriginalPriceTag(currency).Price;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x000041BC File Offset: 0x000025BC
		public PriceTag GetPriceTagByCurrency(Currency currency)
		{
			return this.Prices.FirstOrDefault((PriceTag x) => x.Currency == currency);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000041ED File Offset: 0x000025ED
		public PriceTag GetPriceTag()
		{
			return this.Prices.First((PriceTag p) => p.Price > 0UL || !string.IsNullOrEmpty(p.KeyCatalogName) || this.Content.Item.Type == "contract");
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00004208 File Offset: 0x00002608
		public StoreOffer AddOriginalPrice(PriceTag price)
		{
			this.OriginalPrices.Add(price);
			if (this.IsSaleOffer() && price.Currency != Currency.KeyMoney)
			{
				price.Price = (ulong)Math.Round((double)(price.Price * (1f - this.Discount / 100f)));
			}
			this.Prices.Add(price);
			return this;
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00004274 File Offset: 0x00002674
		public PriceTag GetOriginalPriceTag(Currency currency)
		{
			return this.OriginalPrices.FirstOrDefault((PriceTag x) => x.Currency == currency);
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000042A5 File Offset: 0x000026A5
		public bool IsKeyPriceOffer()
		{
			return this.Prices.Any((PriceTag p) => !string.IsNullOrEmpty(p.KeyCatalogName));
		}

		// Token: 0x060000F0 RID: 240 RVA: 0x000042CF File Offset: 0x000026CF
		public bool IsSaleOffer()
		{
			return !string.IsNullOrEmpty(this.Status) && this.Status.Equals("sale", StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x000042F5 File Offset: 0x000026F5
		public bool IsIngameCoin()
		{
			return this.Category.Equals("InGameCoins");
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x00004308 File Offset: 0x00002708
		public override string ToString()
		{
			return string.Format("<StoreOffer> {0} - {1} : category={2}, type={3}, expiration={4}, durability={5}, quantity={6}, status={7}, item_name={8}", new object[]
			{
				this.SupplierID,
				this.StoreID,
				this.Category,
				this.Type,
				this.Content.ExpirationTime,
				this.Content.DurabilityPoints,
				this.Content.Quantity,
				this.Status,
				this.Content.Item.Name
			});
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000043B0 File Offset: 0x000027B0
		public override int GetHashCode()
		{
			CRC32 crc = new CRC32();
			crc.GetHash(Encoding.ASCII.GetBytes(this.Category));
			int num = this.StoreID.GetHashCode();
			num ^= this.SupplierID.GetHashCode();
			num ^= this.Rank.GetHashCode();
			num ^= (int)crc.CRCVal;
			num ^= this.Content.GetHashCode();
			num ^= this.Discount.GetHashCode();
			crc.Reset();
			crc.GetHash(Encoding.ASCII.GetBytes(this.Status));
			num ^= (int)crc.CRCVal;
			foreach (PriceTag priceTag in this.Prices)
			{
				num ^= priceTag.GetHashCode();
			}
			return num;
		}

		// Token: 0x04000109 RID: 265
		public int SupplierID;

		// Token: 0x0400010A RID: 266
		public ulong StoreID;

		// Token: 0x0400010B RID: 267
		public string Category;

		// Token: 0x0400010C RID: 268
		public string Status;

		// Token: 0x0400010D RID: 269
		public OfferType Type;

		// Token: 0x0400010E RID: 270
		public OfferItem Content;

		// Token: 0x0400010F RID: 271
		public int Rank;

		// Token: 0x04000110 RID: 272
		public uint Discount;
	}
}
