using System;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog.ShopSupplier.Serializers
{
	// Token: 0x0200024F RID: 591
	internal static class StoreOfferValidator
	{
		// Token: 0x06000D04 RID: 3332 RVA: 0x00032340 File Offset: 0x00030740
		public static bool IsOfferValid(StoreOffer offer)
		{
			StoreOfferValidator.ValidateOfferPrice(offer);
			if (offer.IsSaleOffer())
			{
				StoreOfferValidator.ValidateSaleOffer(offer);
			}
			else if (offer.Discount != 0U)
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' not marked as \"SALE\" but have discount ({2}%)", offer.StoreID, offer.Content.Item.Name, offer.Discount));
			}
			StoreOfferValidator.ValidateOfferTypesCombination(offer);
			StoreOfferValidator.ValidateOfferContent(offer);
			StoreOfferValidator.ValidateOfferRepairCost(offer);
			return true;
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x000323C0 File Offset: 0x000307C0
		private static void ValidateOfferRepairCost(StoreOffer offer)
		{
			if (offer.Type != OfferType.Permanent)
			{
				return;
			}
			int num;
			if (!int.TryParse(offer.Content.RepairCost, out num) || num <= 0)
			{
				throw new StoreOfferParseException(string.Format("Permanent offer '{0}:{1}' has incorrect repair cost, current value is '{2}'", offer.SupplierID, offer.StoreID, offer.Content.RepairCost));
			}
		}

		// Token: 0x06000D06 RID: 3334 RVA: 0x0003242C File Offset: 0x0003082C
		private static void ValidateOfferPrice(StoreOffer offer)
		{
			ulong num = offer.GetPriceByCurrency(Currency.GameMoney) + offer.GetPriceByCurrency(Currency.CryMoney) + offer.GetPriceByCurrency(Currency.CrownMoney);
			if (num == 0UL && offer.Content.Item.Type != "contract")
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' doesn't have price", offer.StoreID, offer.Content.Item.Name));
			}
			if (num > Math.Max(offer.GetPriceByCurrency(Currency.GameMoney), Math.Max(offer.GetPriceByCurrency(Currency.CryMoney), offer.GetPriceByCurrency(Currency.CrownMoney))))
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' has price in different currencies. It's restricted by design.", offer.StoreID, offer.Content.Item.Name));
			}
		}

		// Token: 0x06000D07 RID: 3335 RVA: 0x000324F4 File Offset: 0x000308F4
		private static void ValidateSaleOffer(StoreOffer offer)
		{
			if (offer.IsKeyPriceOffer())
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' marked as \"SALE\" and KeyPrice offer", offer.StoreID, offer.Content.Item.Name));
			}
			if (offer.Discount < 1U || offer.Discount > 99U)
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' marked as \"SALE\" and have incorrect discount ({2}%)", offer.StoreID, offer.Content.Item.Name, offer.Discount));
			}
			foreach (PriceTag priceTag in offer.OriginalPrices)
			{
				if (priceTag.Price != 0UL)
				{
					ulong priceByCurrency = offer.GetPriceByCurrency(priceTag.Currency);
					if (priceByCurrency == 0UL || priceByCurrency > priceTag.Price)
					{
						throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' marked as \"SALE\" and have incorect discounted price {2}(-{3}%) for currency {4}", new object[]
						{
							offer.StoreID,
							offer.Content.Item.Name,
							priceByCurrency,
							offer.Discount,
							priceTag.Currency
						}));
					}
				}
			}
		}

		// Token: 0x06000D08 RID: 3336 RVA: 0x00032664 File Offset: 0x00030A64
		private static void ValidateOfferTypesCombination(StoreOffer offer)
		{
			if (offer.Content.ExpirationTime > TimeSpan.Zero && (offer.Content.DurabilityPoints > 0 || offer.Content.Quantity > 0UL))
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' can't be expirable, durable, consumble at same time", offer.StoreID, offer.Content.Item.Name));
			}
			if (offer.Content.DurabilityPoints > 0 && offer.Content.Quantity > 0UL)
			{
				throw new StoreOfferParseException(string.Format("Offer {0} for item '{1}' can't be expirable, durable, consumble at same time", offer.StoreID, offer.Content.Item.Name));
			}
		}

		// Token: 0x06000D09 RID: 3337 RVA: 0x00032728 File Offset: 0x00030B28
		private static void ValidateOfferContent(StoreOffer offer)
		{
			if (offer.Content.Item.Stackable && offer.Content.Quantity == 0UL)
			{
				throw new StoreOfferParseException(string.Format("'quantity' parameter for stackable offer {0} is not specified", offer.StoreID));
			}
			if ((offer.Content.Item.Type == "random_box" || offer.Content.Item.Type == "weapon_skin" || offer.Content.Item.Type == "meta_game") && (offer.Content.DurabilityPoints != 0 || offer.Content.ExpirationTime != TimeSpan.Zero))
			{
				throw new StoreOfferParseException(string.Format("'durability' or 'expiration' parameter for {1} offer {0} shouldn't be specified", offer.StoreID, offer.Content.Item.Type));
			}
			if (offer.Content.Item.Name == "game_money_item_01" && offer.Content.Quantity > 1000000UL)
			{
				throw new StoreOfferParseException(string.Format("Game money offer {0} shouldn't contains more than {1} money", offer.StoreID, 1000000UL));
			}
		}

		// Token: 0x040005F7 RID: 1527
		public const ulong OfferGameMoneyCap = 1000000UL;
	}
}
