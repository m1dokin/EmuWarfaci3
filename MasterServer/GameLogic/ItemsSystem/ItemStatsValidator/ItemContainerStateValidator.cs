using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem.ItemStatsValidator
{
	// Token: 0x0200006B RID: 107
	[Service]
	[Singleton]
	internal class ItemContainerStateValidator : IItemStatsValidator
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x0000AB13 File Offset: 0x00008F13
		public ItemContainerStateValidator(IItemStats itemStats)
		{
			this.m_itemStats = itemStats;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000AB24 File Offset: 0x00008F24
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			IList<RandomBoxDesc> randomBoxesDesc = this.m_itemStats.GetRandomBoxesDesc();
			IList<BundleDesc> bundlesDesc = this.m_itemStats.GetBundlesDesc();
			using (IEnumerator<IItemsContainer> enumerator = randomBoxesDesc.Concat(bundlesDesc.Cast<IItemsContainer>()).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IItemsContainer containerDesc = enumerator.Current;
					StoreOffer storeOffer = offers.FirstOrDefault((StoreOffer o) => o.Content.Item.Name == containerDesc.Name);
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
							if (array.Length != 3)
							{
								throw new RepairDescriptionValidationException(string.Format("Incorrect format for repairCost attribute in {0}, correct format 'itemName,repair_cost,durability, received {1}", containerDesc.Name, storeOffer.Content.RepairCost));
							}
							string name = array[0];
							if (!containerDesc.HasItemNamed(name))
							{
								throw new RepairDescriptionValidationException(string.Format("Repair cost description in {0} contains item {1} than can't be found in randomBox/bundle", containerDesc.Name, array[0]));
							}
						}
					}
				}
			}
		}

		// Token: 0x040000C1 RID: 193
		private readonly IItemStats m_itemStats;
	}
}
