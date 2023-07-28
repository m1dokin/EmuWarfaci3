using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.RandomBoxValidationSystem;

namespace MasterServer.GameLogic.ItemsSystem.ItemStatsValidator
{
	// Token: 0x02000330 RID: 816
	[Service]
	[Singleton]
	internal class RandomBoxStatsValidator : IItemStatsValidator
	{
		// Token: 0x06001259 RID: 4697 RVA: 0x00049600 File Offset: 0x00047A00
		public RandomBoxStatsValidator(IItemCache itemCache, IItemStats itemStats, IItemRepairDescriptionRepository repairRepository)
		{
			this.m_itemCache = itemCache;
			this.m_itemStats = itemStats;
			this.m_repairRepository = repairRepository;
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x00049620 File Offset: 0x00047A20
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			IList<RandomBoxDesc> randomBoxesDesc = this.m_itemStats.GetRandomBoxesDesc();
			foreach (RandomBoxDesc randomBox in randomBoxesDesc)
			{
				this.CheckRandomBoxData(randomBox, offers);
			}
			this.ValidateRandomBoxItemsCount(randomBoxesDesc, offers);
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x0004968C File Offset: 0x00047A8C
		public void CheckRandomBoxData(RandomBoxDesc randomBox, IEnumerable<StoreOffer> storeOffers)
		{
			string[] array = (from @group in randomBox.Groups
			from choice in @group.Choices
			select new
			{
				@group,
				choice
			} into <>__TranspIdent2
			where !this.m_itemCache.GetAllItemsByName(false).ContainsKey(<>__TranspIdent2.choice.Name)
			select <>__TranspIdent2.choice.Name).ToArray<string>();
			string[] array2 = (from @group in randomBox.Groups
			from choice in @group.Choices
			select new
			{
				@group,
				choice
			} into <>__TranspIdent3
			where this.m_itemCache.GetAllItemsByName(false).ContainsKey(<>__TranspIdent3.choice.Name) && !this.m_itemCache.GetAllItemsByName(true).ContainsKey(<>__TranspIdent3.choice.Name)
			select <>__TranspIdent3.choice.Name).ToArray<string>();
			if (array.Any<string>())
			{
				throw new RandomBoxValidationException(string.Format("Random box {0} contains non-existent item(s) {1}", randomBox.Name, string.Join(",", array)));
			}
			if (array2.Any<string>())
			{
				throw new RandomBoxValidationException(string.Format("Random box {0} contains non-active item(s) {1}", randomBox.Name, string.Join(",", array2)));
			}
			List<StoreOffer> list = (from offer in storeOffers
			where offer.Content.Item.Name == randomBox.Name
			select offer).ToList<StoreOffer>();
			if (!list.Any<StoreOffer>())
			{
				return;
			}
			int rank = list[0].Rank;
			if (list.Any((StoreOffer x) => x.Rank != rank))
			{
				throw new RandomBoxValidationException(string.Format("Offers for random box {0} has different rank restriction", randomBox.Name));
			}
			string text = null;
			bool flag = true;
			foreach (StoreOffer storeOffer in list)
			{
				if (string.IsNullOrEmpty(text))
				{
					text = storeOffer.Content.RepairCost;
				}
				flag &= text.Equals(storeOffer.Content.RepairCost);
				if (!flag)
				{
					throw new RandomBoxValidationException(string.Format("RepairCost isn't same in all offers for {0}", randomBox.Name));
				}
			}
			RandomBoxStatsValidator.ValidateSingleItemDefinedAsTopPrize(randomBox);
			bool flag2 = true;
			foreach (RandomBoxDesc.Group group2 in randomBox.Groups)
			{
				foreach (RandomBoxDesc.Choice choice2 in group2.Choices)
				{
					string itemName = choice2.Name;
					if (!this.m_itemStats.GetBundlesDesc().Any((BundleDesc e) => e.Name == itemName))
					{
						flag2 &= choice2.IsRegular;
						if (choice2.HasTopPrizeTokenDefined())
						{
							this.ValidateTopPrizeDescription(choice2, itemName, randomBox.Name);
						}
						if (CommonValidationMethods.ValidateContainedItemAmount(randomBox.Name, choice2) != ValidationFlowAdvice.Finish)
						{
							if (CommonValidationMethods.ValidateNonPermanentContainedItem(randomBox.Name, choice2) != ValidationFlowAdvice.Finish)
							{
								if (CommonValidationMethods.ValidateRegularItem(randomBox.Name, choice2, this.m_itemCache, this.m_repairRepository, storeOffers) != ValidationFlowAdvice.Finish)
								{
									CommonValidationMethods.ValidateContainedItemRepairDescription(randomBox, itemName, text, this.m_itemCache, this.m_repairRepository, storeOffers);
								}
							}
						}
					}
				}
			}
			if (flag2)
			{
				throw new RandomBoxValidationException(string.Format("RandomBox {0} contains only regular items, at least one filler required", randomBox.Name));
			}
		}

		// Token: 0x0600125C RID: 4700 RVA: 0x00049AF8 File Offset: 0x00047EF8
		private static void ValidateSingleItemDefinedAsTopPrize(RandomBoxDesc randomBox)
		{
			if (randomBox.Groups.SelectMany((RandomBoxDesc.Group group) => from c in @group.Choices
			where c.HasTopPrizeTokenDefined() && c.WinLimit > 0
			select c).Count<RandomBoxDesc.Choice>() > 1)
			{
				throw new RandomBoxValidationException(string.Format("RandomBox {0} contains more then one item marked as top prize", randomBox.Name));
			}
		}

		// Token: 0x0600125D RID: 4701 RVA: 0x00049B50 File Offset: 0x00047F50
		private void ValidateTopPrizeDescription(RandomBoxDesc.Choice choice, string itemName, string boxName)
		{
			SItem sitem;
			if (!this.m_itemCache.TryGetItem(choice.TopPrizeToken, out sitem))
			{
				throw new RandomBoxValidationException(string.Format("RandomBox {0} item {1} uses top prize token item {2} missing from items cache", boxName, itemName, choice.TopPrizeToken));
			}
			if (!sitem.Active)
			{
				throw new RandomBoxValidationException(string.Format("RandomBox {0} item {1} uses top prize token item {2} marked as inactive", boxName, itemName, sitem.Name));
			}
			StackableItemStats stackableItemStats = this.m_itemStats.GetStackableItemStats(sitem.ID);
			if (!stackableItemStats.IsStackable)
			{
				throw new RandomBoxValidationException(string.Format("RandomBox {0} item {1} uses non-stackeble top prize token item {2}", boxName, itemName, sitem.Name));
			}
			if (choice.WinLimit < 1 || choice.WinLimit > (int)stackableItemStats.MaxBuyAmount)
			{
				throw new RandomBoxValidationException(string.Format("RandomBox {0} item {1} uses invalid win limit for the top prize token item {2}. Win limit must be in [1, {3}]", new object[]
				{
					boxName,
					itemName,
					sitem.Name,
					stackableItemStats.MaxBuyAmount
				}));
			}
		}

		// Token: 0x0600125E RID: 4702 RVA: 0x00049C38 File Offset: 0x00048038
		private void ValidateRandomBoxItemsCount(IEnumerable<RandomBoxDesc> randomBoxes, IEnumerable<StoreOffer> availableStoreOffers)
		{
			IEnumerable<StoreOffer> source = from x in availableStoreOffers
			where !x.IsKeyPriceOffer()
			select x;
			int num = int.Parse(Resources.ModuleSettings.GetSection("Shop").GetSection("RandomBox").Get("max_items_supported"));
			using (IEnumerator<RandomBoxDesc> enumerator = randomBoxes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RandomBoxDesc randomBoxDesc = enumerator.Current;
					if (source.Any((StoreOffer x) => x.Content.Item.Name == randomBoxDesc.Name))
					{
						int num2 = (from c in randomBoxDesc.Groups.SelectMany((RandomBoxDesc.Group g) => g.Choices)
						select c.Name).Distinct<string>().Count<string>();
						if (num2 > num)
						{
							throw new RandomBoxValidationException(string.Format("RandomBox {0} contains more unique items than supported: {1}. Max: {2}", randomBoxDesc.Name, num2, num));
						}
					}
				}
			}
		}

		// Token: 0x04000873 RID: 2163
		private readonly IItemRepairDescriptionRepository m_repairRepository;

		// Token: 0x04000874 RID: 2164
		private readonly IItemStats m_itemStats;

		// Token: 0x04000875 RID: 2165
		private readonly IItemCache m_itemCache;
	}
}
