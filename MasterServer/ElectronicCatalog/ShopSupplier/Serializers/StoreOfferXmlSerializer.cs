using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using Util.Common;

namespace MasterServer.ElectronicCatalog.ShopSupplier.Serializers
{
	// Token: 0x02000250 RID: 592
	internal class StoreOfferXmlSerializer : IXmlSerializer<StoreOffer>
	{
		// Token: 0x06000D0A RID: 3338 RVA: 0x00032884 File Offset: 0x00030C84
		public StoreOfferXmlSerializer(IDALService dalService) : this(dalService, null)
		{
		}

		// Token: 0x06000D0B RID: 3339 RVA: 0x00032890 File Offset: 0x00030C90
		public StoreOfferXmlSerializer(IDALService dalService, XmlDocument xmlDocument)
		{
			this.m_xmlDocument = xmlDocument;
			this.m_catalogItemsByName = dalService.ECatalog.GetCatalogItems().ToDictionary((CatalogItem i) => i.Name);
		}

		// Token: 0x06000D0C RID: 3340 RVA: 0x000328E0 File Offset: 0x00030CE0
		public XmlElement Serialize(StoreOffer offer)
		{
			XmlElement xmlElement = this.m_xmlDocument.CreateElement("offer");
			xmlElement.SetAttribute("id", offer.StoreID.ToString());
			xmlElement.SetAttribute("expirationTime", TimeUtils.GetExpireTime(offer.Content.ExpirationTime));
			xmlElement.SetAttribute("durabilityPoints", offer.Content.DurabilityPoints.ToString());
			xmlElement.SetAttribute("repair_cost", offer.Content.RepairCost);
			xmlElement.SetAttribute("quantity", offer.Content.Quantity.ToString());
			xmlElement.SetAttribute("name", offer.Content.Item.Name);
			xmlElement.SetAttribute("item_category_override", offer.Category);
			xmlElement.SetAttribute("offer_status", offer.Status.ToLower());
			xmlElement.SetAttribute("supplier_id", offer.SupplierID.ToString());
			xmlElement.SetAttribute("discount", offer.Discount.ToString());
			xmlElement.SetAttribute("rank", offer.Rank.ToString());
			xmlElement.SetAttribute("game_price", offer.GetPriceByCurrency(Currency.GameMoney).ToString());
			xmlElement.SetAttribute("cry_price", offer.GetPriceByCurrency(Currency.CryMoney).ToString());
			xmlElement.SetAttribute("crown_price", offer.GetPriceByCurrency(Currency.CrownMoney).ToString());
			xmlElement.SetAttribute("game_price_origin", offer.GetOriginalPriceByCurrency(Currency.GameMoney).ToString());
			xmlElement.SetAttribute("cry_price_origin", offer.GetOriginalPriceByCurrency(Currency.CryMoney).ToString());
			xmlElement.SetAttribute("crown_price_origin", offer.GetOriginalPriceByCurrency(Currency.CrownMoney).ToString());
			xmlElement.SetAttribute("key_item_name", offer.GetPriceTagByCurrency(Currency.KeyMoney).KeyCatalogName);
			return xmlElement;
		}

		// Token: 0x06000D0D RID: 3341 RVA: 0x00032B04 File Offset: 0x00030F04
		public StoreOffer Deserialize(XmlElement element)
		{
			string attribute = element.GetAttribute("item_name");
			string attribute2 = element.GetAttribute("store_id");
			if (!element.Name.Equals("offer", StringComparison.InvariantCultureIgnoreCase) && !element.Name.Equals("product", StringComparison.InvariantCultureIgnoreCase))
			{
				throw new StoreOfferParseException(string.Format("Incorrect xml node expected 'offer' received '{0}'", element.Name));
			}
			if (string.IsNullOrEmpty(attribute2))
			{
				throw new StoreOfferParseException(string.Format("Offer for item '{0}' contains empty store_id", attribute));
			}
			CatalogItem item;
			if (!this.m_catalogItemsByName.TryGetValue(attribute, out item))
			{
				Log.Warning<string, string>("Skip offer '{0}' with missed catalog item '{1}'", attribute2, attribute);
				return null;
			}
			if (!item.Active)
			{
				Log.Warning<string, string>("Skip offer '{0}' with non-active item '{1}'", attribute2, attribute);
				return null;
			}
			StoreOffer storeOffer = new StoreOffer();
			storeOffer.StoreID = (ulong)uint.Parse(attribute2);
			storeOffer.Status = element.GetAttribute("offer_status").ToLower();
			storeOffer.Discount = ((!element.HasAttribute("discount")) ? 0U : uint.Parse(element.GetAttribute("discount")));
			uint num = (!element.HasAttribute("game_money")) ? 0U : uint.Parse(element.GetAttribute("game_money"));
			uint num2 = (!element.HasAttribute("cry_money")) ? 0U : uint.Parse(element.GetAttribute("cry_money"));
			uint num3 = (!element.HasAttribute("crown_money")) ? 0U : uint.Parse(element.GetAttribute("crown_money"));
			string keyCatalogName = (!element.HasAttribute("key_item_name")) ? string.Empty : element.GetAttribute("key_item_name");
			storeOffer.AddOriginalPrice(new PriceTag
			{
				Currency = Currency.GameMoney,
				Price = (ulong)num
			}).AddOriginalPrice(new PriceTag
			{
				Currency = Currency.CrownMoney,
				Price = (ulong)num3
			}).AddOriginalPrice(new PriceTag
			{
				Currency = Currency.CryMoney,
				Price = (ulong)num2
			}).AddOriginalPrice(new PriceTag
			{
				Currency = Currency.KeyMoney,
				KeyCatalogName = keyCatalogName,
				Price = 0UL
			});
			storeOffer.Category = element.GetAttribute("item_category_override");
			storeOffer.Rank = ((!element.HasAttribute("rank")) ? 0 : int.Parse(element.GetAttribute("rank")));
			storeOffer.Content.Item = item;
			storeOffer.Content.ExpirationTime = TimeUtils.GetOfferExpireTime(element.GetAttribute("expiration_time"));
			storeOffer.Content.DurabilityPoints = int.Parse(element.GetAttribute("durability_points"));
			storeOffer.Content.RepairCost = element.GetAttribute("repair_cost");
			storeOffer.Content.Quantity = ((!string.IsNullOrEmpty(element.GetAttribute("quantity"))) ? ulong.Parse(element.GetAttribute("quantity")) : 0UL);
			storeOffer.Type = OfferType.Regular;
			if (storeOffer.Content.ExpirationTime > TimeSpan.Zero)
			{
				storeOffer.Type = OfferType.Expiration;
			}
			else if (storeOffer.Content.DurabilityPoints > 0)
			{
				storeOffer.Type = OfferType.Permanent;
			}
			else if (storeOffer.Content.Quantity > 0UL)
			{
				storeOffer.Type = OfferType.Consumable;
			}
			return (!StoreOfferValidator.IsOfferValid(storeOffer)) ? null : storeOffer;
		}

		// Token: 0x040005F8 RID: 1528
		private readonly Dictionary<string, CatalogItem> m_catalogItemsByName;

		// Token: 0x040005F9 RID: 1529
		private readonly XmlDocument m_xmlDocument;
	}
}
