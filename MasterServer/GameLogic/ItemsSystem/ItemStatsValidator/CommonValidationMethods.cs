using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem.ItemStatsValidator
{
	// Token: 0x02000326 RID: 806
	internal static class CommonValidationMethods
	{
		// Token: 0x06001243 RID: 4675 RVA: 0x00048B28 File Offset: 0x00046F28
		public static ValidationFlowAdvice ValidateContainedItemAmount(string containerName, IGenericItem item)
		{
			int? amount = item.Amount;
			if (amount == null)
			{
				return ValidationFlowAdvice.Continue;
			}
			if (amount.Value < 1)
			{
				throw new GenericValidationException(string.Format("Incorrect amount for {0} in container {1}", item.Name, containerName));
			}
			return ValidationFlowAdvice.Finish;
		}

		// Token: 0x06001244 RID: 4676 RVA: 0x00048B70 File Offset: 0x00046F70
		public static ValidationFlowAdvice ValidateNonPermanentContainedItem(string containerName, IGenericItem item)
		{
			bool isExpirable = item.IsExpirable;
			bool isRegular = item.IsRegular;
			if (isRegular && isExpirable)
			{
				throw new GenericValidationException(string.Format("Item {0} cannot contain both regular and expiration, container {1}", item.Name, containerName));
			}
			return (!isRegular && !isExpirable) ? ValidationFlowAdvice.Continue : ValidationFlowAdvice.Finish;
		}

		// Token: 0x06001245 RID: 4677 RVA: 0x00048BC4 File Offset: 0x00046FC4
		public static ValidationFlowAdvice ValidateRegularItem(string containerName, IGenericItem item, IItemCache itemCache, IItemRepairDescriptionRepository repairRepository, IEnumerable<StoreOffer> storeOffers)
		{
			SItem sitem;
			itemCache.TryGetItem(item.Name, false, out sitem);
			StoreOffer storeOffer = storeOffers.FirstOrDefault((StoreOffer x) => x.Content.Item.Name == item.Name && x.Type == OfferType.Permanent);
			Dictionary<ulong, SRepairItemDesc> boxedItemRepairDesc = repairRepository.ParseBoxedItemRepairData(storeOffers);
			SRepairItemDesc srepairItemDesc;
			bool repairItemDesc = repairRepository.GetRepairItemDesc(sitem.ID, (storeOffer == null) ? 0UL : storeOffer.Content.Item.ID, storeOffers, boxedItemRepairDesc, out srepairItemDesc);
			if (!item.IsRegular)
			{
				return ValidationFlowAdvice.Continue;
			}
			if (repairItemDesc)
			{
				throw new ValidationException(string.Format("Item {0} in {1} couldn't be regular and permanent at the same time", item.Name, containerName));
			}
			return ValidationFlowAdvice.Finish;
		}

		// Token: 0x06001246 RID: 4678 RVA: 0x00048C78 File Offset: 0x00047078
		public static void ValidateContainedItemRepairDescription(IItemsContainer container, string itemName, string repairCostStr, IItemCache itemCache, IItemRepairDescriptionRepository repairRepository, IEnumerable<StoreOffer> storeOffers)
		{
			SItem sitem;
			itemCache.TryGetItem(itemName, false, out sitem);
			StoreOffer storeOffer = storeOffers.FirstOrDefault((StoreOffer x) => x.Content.Item.Name == itemName && x.Type == OfferType.Permanent);
			Dictionary<string, SItem> allItemsByName = itemCache.GetAllItemsByName(false);
			Dictionary<ulong, SRepairItemDesc> boxedItemRepairDesc = repairRepository.ParseBoxedItemRepairData(storeOffers);
			SRepairItemDesc srepairItemDesc;
			bool repairItemDesc = repairRepository.GetRepairItemDesc(sitem.ID, (storeOffer == null) ? 0UL : storeOffer.Content.Item.ID, storeOffers, boxedItemRepairDesc, out srepairItemDesc);
			if (!string.IsNullOrEmpty(repairCostStr) && repairCostStr != "0")
			{
				Match match = Regex.Match(repairCostStr, string.Format("{0},(.*?),(.*?);", itemName));
				if (match.Groups.Count != 3)
				{
					throw new RepairDescriptionValidationException(string.Format("Incorrect format for repairCost attribute in {0}, correct format '{1},repair_cost,durability' received {2}", container.Name, itemName, repairCostStr));
				}
				int num = int.Parse(match.Groups[1].Value);
				int num2 = int.Parse(match.Groups[2].Value);
				SItem sitem2;
				if (!allItemsByName.TryGetValue(itemName, out sitem2))
				{
					throw new RepairDescriptionValidationException(string.Format("Repair cost description in {0} contains nonexistent item {1}", container.Name, itemName));
				}
				if (!container.HasItemNamed(itemName))
				{
					throw new RepairDescriptionValidationException(string.Format("Repair cost description in {0} contains item {1} than can't be found in randomBox ", container.Name, itemName));
				}
				if (repairItemDesc && (srepairItemDesc.Durability != num2 || srepairItemDesc.RepairCost != num))
				{
					throw new RepairDescriptionValidationException(string.Format("Item {0} from {1}, has repair cost ({2}) and/or durability ({3}) different from the one stored in the ItemStats ({4})", new object[]
					{
						itemName,
						container.Name,
						num,
						num2,
						srepairItemDesc
					}));
				}
			}
			if (!repairItemDesc)
			{
				throw new RepairDescriptionValidationException(string.Format("Permanent item {0} in {1} lacks repair cost in offer and in item description", itemName, container.Name));
			}
		}
	}
}
