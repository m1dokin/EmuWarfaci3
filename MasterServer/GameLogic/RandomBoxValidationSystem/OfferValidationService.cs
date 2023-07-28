using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.ShopSupplier.Serializers;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.GameLogic.RandomBoxValidationSystem
{
	// Token: 0x020000BD RID: 189
	[Service]
	[Singleton]
	internal class OfferValidationService : ServiceModule, IOfferValidationService
	{
		// Token: 0x060002FD RID: 765 RVA: 0x0000E15E File Offset: 0x0000C55E
		public OfferValidationService(IOnlineVariables onlineVariablesService, IEnumerable<IItemStatsValidator> validators)
		{
			this.m_onlineVariablesService = onlineVariablesService;
			this.m_validators = validators;
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000E174 File Offset: 0x0000C574
		public void Validate(IEnumerable<StoreOffer> newOffers)
		{
			this.Validate(Enumerable.Empty<StoreOffer>(), newOffers);
		}

		// Token: 0x060002FF RID: 767 RVA: 0x0000E184 File Offset: 0x0000C584
		public void Validate(IEnumerable<StoreOffer> currentOffers, IEnumerable<StoreOffer> newOffers)
		{
			this.ValidateIngameCoinsOffers(currentOffers, newOffers);
			this.ValidateOfferSet(newOffers);
			foreach (IItemStatsValidator itemStatsValidator in this.m_validators)
			{
				itemStatsValidator.Validate(newOffers);
			}
		}

		// Token: 0x06000300 RID: 768 RVA: 0x0000E1EC File Offset: 0x0000C5EC
		private void ValidateIngameCoinsOffers(IEnumerable<StoreOffer> oldOffers, IEnumerable<StoreOffer> newOffers)
		{
			if (oldOffers.IsNullOrEmpty<StoreOffer>())
			{
				return;
			}
			StoreOffer storeOffer = newOffers.FirstOrDefault((StoreOffer o) => o.IsIngameCoin());
			StoreOffer storeOffer2 = oldOffers.FirstOrDefault((StoreOffer o) => o.IsIngameCoin());
			if (storeOffer2 == null)
			{
				return;
			}
			if (storeOffer2 != null && storeOffer == null)
			{
				throw new StoreOfferParseException("You've deleted an ingame coin offer from catalog_offers.xml! This change is forbidden!");
			}
			StoreOfferComparer storeOfferComparer = new StoreOfferComparer();
			if (!storeOfferComparer.Equals(storeOffer2, storeOffer))
			{
				throw new StoreOfferParseException("Current offer for an ingame coin is different from the previous one! This change is forbidden!");
			}
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0000E28C File Offset: 0x0000C68C
		private void ValidateOfferSet(IEnumerable<StoreOffer> offers)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, uint> dictionary2 = new Dictionary<string, uint>();
			Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
			Dictionary<ulong, List<StoreOffer>> dictionary4 = new Dictionary<ulong, List<StoreOffer>>();
			foreach (StoreOffer storeOffer in offers)
			{
				string status;
				if (dictionary.TryGetValue(storeOffer.Content.Item.Name, out status))
				{
					this.CheckOfferStatus(status, storeOffer);
				}
				else
				{
					dictionary.Add(storeOffer.Content.Item.Name, storeOffer.Status);
				}
				if (!storeOffer.IsKeyPriceOffer())
				{
					uint discount;
					if (dictionary2.TryGetValue(storeOffer.Content.Item.Name, out discount))
					{
						this.CheckOfferDiscount(storeOffer, discount);
					}
					else
					{
						dictionary2.Add(storeOffer.Content.Item.Name, storeOffer.Discount);
					}
				}
				int rank;
				if (dictionary3.TryGetValue(storeOffer.Content.Item.Name, out rank))
				{
					this.CheckOfferRank(storeOffer, rank);
				}
				else
				{
					dictionary3.Add(storeOffer.Content.Item.Name, storeOffer.Rank);
				}
				this.CheckKeyOffer(storeOffer);
				if (storeOffer.Content.Item.Type == "random_box" && !storeOffer.IsKeyPriceOffer())
				{
					List<StoreOffer> list;
					if (dictionary4.TryGetValue(storeOffer.Content.Item.ID, out list))
					{
						list.Add(storeOffer);
					}
					else
					{
						dictionary4.Add(storeOffer.Content.Item.ID, new List<StoreOffer>
						{
							storeOffer
						});
					}
				}
			}
			this.CheckRandomBoxOffers(dictionary4);
		}

		// Token: 0x06000302 RID: 770 RVA: 0x0000E480 File Offset: 0x0000C880
		private void CheckOfferStatus(string status, StoreOffer offer)
		{
			if (!status.Equals("normal", StringComparison.InvariantCultureIgnoreCase) && !offer.Status.Equals("normal", StringComparison.InvariantCultureIgnoreCase) && offer.Status != status)
			{
				throw new StoreOfferParseException(string.Format("Offer for item {0}-{1}: {2} already has not normal status", offer.SupplierID, offer.StoreID, offer.Content.Item.Name));
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x0000E4FC File Offset: 0x0000C8FC
		private void CheckOfferDiscount(StoreOffer offer, uint discount)
		{
			if (offer.Discount != discount)
			{
				throw new StoreOfferParseException(string.Format("Offer for item {0}-{1}: {2} has different discount ({3} and {4})", new object[]
				{
					offer.SupplierID,
					offer.StoreID,
					offer.Content.Item.Name,
					discount,
					offer.Discount
				}));
			}
		}

		// Token: 0x06000304 RID: 772 RVA: 0x0000E574 File Offset: 0x0000C974
		private void CheckOfferRank(StoreOffer offer, int rank)
		{
			if (offer.Rank != rank)
			{
				throw new StoreOfferParseException(string.Format("Offer for item {0}-{1}: {2} has different rank restrictions ({3} and {4})", new object[]
				{
					offer.SupplierID,
					offer.StoreID,
					offer.Content.Item.Name,
					rank,
					offer.Rank
				}));
			}
		}

		// Token: 0x06000305 RID: 773 RVA: 0x0000E5E9 File Offset: 0x0000C9E9
		private void CheckKeyOffer(StoreOffer offer)
		{
			if (offer.IsKeyPriceOffer() && offer.SupplierID != 1)
			{
				throw new StoreOfferParseException(string.Format("Offer for item {0}-{1}: key item price is supported for local provider only", offer.SupplierID, offer.StoreID));
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x0000E628 File Offset: 0x0000CA28
		private void CheckRandomBoxOffers(Dictionary<ulong, List<StoreOffer>> randomBoxOffers)
		{
			uint num = uint.Parse(this.m_onlineVariablesService.Get("randombox.offers_count", OnlineVariableDestination.Client));
			foreach (KeyValuePair<ulong, List<StoreOffer>> keyValuePair in randomBoxOffers)
			{
				int count = keyValuePair.Value.Count;
				if ((long)count != (long)((ulong)num))
				{
					throw new StoreOfferParseException(string.Format("Wrong offers count for random box {0} have {1} offers instead of {2}", keyValuePair.Value[0].Content.Item.Name, count, num));
				}
				StoreOffer storeOffer = keyValuePair.Value.First<StoreOffer>();
				PriceTag price = storeOffer.Prices.Find((PriceTag x) => x.Price > 0UL);
				StoreOffer storeOffer2 = keyValuePair.Value.FirstOrDefault((StoreOffer x) => x.Prices.Find((PriceTag p) => p.Price > 0UL).Currency != price.Currency);
				if (storeOffer2 != null)
				{
					throw new StoreOfferParseException(string.Format("Offer {0} and offer {1} have prices in different currencies", storeOffer.StoreID, storeOffer2.StoreID));
				}
			}
		}

		// Token: 0x04000145 RID: 325
		private const string NormalOfferStatus = "normal";

		// Token: 0x04000146 RID: 326
		private readonly IOnlineVariables m_onlineVariablesService;

		// Token: 0x04000147 RID: 327
		private readonly IEnumerable<IItemStatsValidator> m_validators;
	}
}
