using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.RandomBoxValidationSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200006A RID: 106
	[Service]
	[Singleton]
	internal class ItemRepairDescriptionRepository : ServiceModule, IItemRepairDescriptionRepository
	{
		// Token: 0x06000192 RID: 402 RVA: 0x0000A479 File Offset: 0x00008879
		public ItemRepairDescriptionRepository(IShopService shopService, IItemStats itemStats, IItemCache itemCache, IConfigProvider<XmlDocument> confiProvider, IOfferValidationService offerValidationService)
		{
			this.m_shopService = shopService;
			this.m_itemCache = itemCache;
			this.m_confiProvider = confiProvider;
			this.m_offerValidationService = offerValidationService;
			this.m_itemStats = itemStats;
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000A4A6 File Offset: 0x000088A6
		public override void Init()
		{
			base.Init();
			this.m_shopService.OffersUpdated += this.OnOffersUpdated;
			this.m_itemStats.ItemStatsUpdated += this.OnItemStatsUpdated;
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000A4DC File Offset: 0x000088DC
		public override void Stop()
		{
			this.m_shopService.OffersUpdated -= this.OnOffersUpdated;
			this.m_itemStats.ItemStatsUpdated -= this.OnItemStatsUpdated;
			base.Stop();
		}

		// Token: 0x06000195 RID: 405 RVA: 0x0000A512 File Offset: 0x00008912
		public bool GetBoxedItemRepairDesc(ulong itemID, out SRepairItemDesc repairInfo)
		{
			return this.m_boxedItemRepairDesc.TryGetValue(itemID, out repairInfo);
		}

		// Token: 0x06000196 RID: 406 RVA: 0x0000A521 File Offset: 0x00008921
		public bool GetRepairItemDesc(ulong itemId, ulong catalogId, out SRepairItemDesc repairDesc)
		{
			return this.GetRepairItemDesc(itemId, catalogId, this.m_shopService.GetOffers(), this.m_boxedItemRepairDesc, out repairDesc);
		}

		// Token: 0x06000197 RID: 407 RVA: 0x0000A540 File Offset: 0x00008940
		public bool GetRepairItemDesc(ulong itemId, ulong catalogId, IEnumerable<StoreOffer> storeOffers, Dictionary<ulong, SRepairItemDesc> boxedItemRepairDesc, out SRepairItemDesc repairDesc)
		{
			StoreOffer[] array = (from offer in storeOffers
			where offer.Content.Item.ID == catalogId && offer.Type == OfferType.Permanent
			select offer).ToArray<StoreOffer>();
			if (array.Any<StoreOffer>())
			{
				StoreOffer storeOffer = array.MinEx((StoreOffer offer) => offer.Content.DurabilityPoints);
				repairDesc = new SRepairItemDesc(storeOffer.Content.DurabilityPoints, int.Parse(storeOffer.Content.RepairCost));
				return true;
			}
			return boxedItemRepairDesc.TryGetValue(itemId, out repairDesc) || this.m_itemRepairDesc.TryGetValue(itemId, out repairDesc);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x0000A5E8 File Offset: 0x000089E8
		public Dictionary<ulong, SRepairItemDesc> ParseBoxedItemRepairData(IEnumerable<StoreOffer> eStoreList)
		{
			Dictionary<ulong, SRepairItemDesc> dictionary = new Dictionary<ulong, SRepairItemDesc>();
			IList<RandomBoxDesc> randomBoxesDesc = this.m_itemStats.GetRandomBoxesDesc();
			IList<BundleDesc> bundlesDesc = this.m_itemStats.GetBundlesDesc();
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName(false);
			using (IEnumerator<IItemsContainer> enumerator = randomBoxesDesc.Concat(bundlesDesc.Cast<IItemsContainer>()).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IItemsContainer containerDesc = enumerator.Current;
					StoreOffer storeOffer = eStoreList.FirstOrDefault((StoreOffer o) => o.Content.Item.Name == containerDesc.Name);
					if (storeOffer != null)
					{
						string[] source = storeOffer.Content.RepairCost.Split(new char[]
						{
							';'
						});
						foreach (string[] array in from part in source
						where !string.IsNullOrEmpty(part) && !part.Equals("0")
						select part.Split(new char[]
						{
							','
						}))
						{
							string key = array[0];
							int repairCost = int.Parse(array[1]);
							int durability = int.Parse(array[2]);
							SItem sitem = allItemsByName[key];
							SRepairItemDesc value = new SRepairItemDesc(durability, repairCost);
							SRepairItemDesc srepairItemDesc;
							if (!dictionary.TryGetValue(sitem.ID, out srepairItemDesc))
							{
								dictionary.Add(sitem.ID, value);
							}
						}
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06000199 RID: 409 RVA: 0x0000A7B0 File Offset: 0x00008BB0
		private void UpdateStats(XmlDocument items)
		{
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			Dictionary<ulong, SRepairItemDesc> dictionary = new Dictionary<ulong, SRepairItemDesc>();
			IEnumerator enumerator = items.DocumentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					string attribute = xmlElement.GetAttribute("name");
					SItem sitem;
					if (!allItemsByName.TryGetValue(attribute, out sitem))
					{
						Log.Info<string>("Skip non-active item {0}", attribute);
					}
					else if (xmlElement.FirstChild != null && xmlElement.FirstChild.FirstChild != null)
					{
						XmlElement xmlElement2 = (XmlElement)xmlElement.FirstChild.FirstChild;
						foreach (XmlElement xmlElement3 in xmlElement2.ChildNodes.OfType<XmlElement>())
						{
							if (xmlElement3.Name == "mmo_stats")
							{
								SRepairItemDesc value = this.ParseRepairStats(xmlElement3);
								if (value.Durability != 0 && value.RepairCost != 0)
								{
									dictionary.Add(sitem.ID, value);
								}
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			Interlocked.Exchange<Dictionary<ulong, SRepairItemDesc>>(ref this.m_itemRepairDesc, dictionary);
		}

		// Token: 0x0600019A RID: 410 RVA: 0x0000A940 File Offset: 0x00008D40
		private SRepairItemDesc ParseRepairStats(XmlElement statEl)
		{
			int durability = 0;
			int repairCost = 0;
			IEnumerator enumerator = statEl.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						string attribute = xmlElement.GetAttribute("name");
						if (attribute == "durability")
						{
							durability = int.Parse(xmlElement.GetAttribute("value"));
						}
						else if (attribute == "repair_cost")
						{
							repairCost = int.Parse(xmlElement.GetAttribute("value"));
						}
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return new SRepairItemDesc(durability, repairCost);
		}

		// Token: 0x0600019B RID: 411 RVA: 0x0000AA20 File Offset: 0x00008E20
		private void OnItemStatsUpdated()
		{
			XmlDocument items = this.m_confiProvider.Get();
			this.UpdateStats(items);
			IEnumerable<StoreOffer> offers = this.m_shopService.GetOffers();
			this.m_offerValidationService.Validate(offers);
		}

		// Token: 0x0600019C RID: 412 RVA: 0x0000AA58 File Offset: 0x00008E58
		private void OnOffersUpdated(IEnumerable<StoreOffer> storeOffers)
		{
			Dictionary<ulong, SRepairItemDesc> value = this.ParseBoxedItemRepairData(storeOffers);
			Interlocked.Exchange<Dictionary<ulong, SRepairItemDesc>>(ref this.m_boxedItemRepairDesc, value);
		}

		// Token: 0x040000B7 RID: 183
		private readonly IShopService m_shopService;

		// Token: 0x040000B8 RID: 184
		private readonly IOfferValidationService m_offerValidationService;

		// Token: 0x040000B9 RID: 185
		private readonly IItemCache m_itemCache;

		// Token: 0x040000BA RID: 186
		private readonly IItemStats m_itemStats;

		// Token: 0x040000BB RID: 187
		private readonly IConfigProvider<XmlDocument> m_confiProvider;

		// Token: 0x040000BC RID: 188
		private Dictionary<ulong, SRepairItemDesc> m_itemRepairDesc;

		// Token: 0x040000BD RID: 189
		private Dictionary<ulong, SRepairItemDesc> m_boxedItemRepairDesc;
	}
}
