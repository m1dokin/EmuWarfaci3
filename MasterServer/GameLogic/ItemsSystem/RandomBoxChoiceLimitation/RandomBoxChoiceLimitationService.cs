using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation
{
	// Token: 0x0200008D RID: 141
	[Service]
	[Singleton]
	internal class RandomBoxChoiceLimitationService : ServiceModule, IRandomBoxChoiceLimitationService
	{
		// Token: 0x0600021C RID: 540 RVA: 0x0000BDE1 File Offset: 0x0000A1E1
		public RandomBoxChoiceLimitationService(IConfigProvider<RandomBoxChoiceLimitationConfig> configProvider, IProfileItems profileItems, IItemStats itemStats)
		{
			this.m_configProvider = configProvider;
			this.m_profileItems = profileItems;
			this.m_itemStats = itemStats;
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600021D RID: 541 RVA: 0x0000BDFE File Offset: 0x0000A1FE
		public bool Enabled
		{
			get
			{
				return this.m_configProvider.Get().Enabled;
			}
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000BE10 File Offset: 0x0000A210
		public bool IsBoxAvailable(ulong profileId, string boxName)
		{
			if (!this.Enabled)
			{
				return true;
			}
			IEnumerable<string> regularItemsFromBox = this.GetRegularItemsFromBox(boxName);
			List<string> profileRegularItemsNames = (from it in this.m_profileItems.GetProfileItems(profileId).Values
			where it.OfferType == OfferType.Regular
			select it.GameItem.Name).ToList<string>();
			int num = regularItemsFromBox.Count((string itemName) => profileRegularItemsNames.Contains(itemName));
			return num == 0 || num != regularItemsFromBox.Count<string>();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000BEC0 File Offset: 0x0000A2C0
		public bool IsRegularItemInBox(string boxName)
		{
			if (!this.Enabled)
			{
				return false;
			}
			IEnumerable<string> regularItemsFromBox = this.GetRegularItemsFromBox(boxName);
			return regularItemsFromBox.Any<string>();
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000BEE8 File Offset: 0x0000A2E8
		private IEnumerable<string> GetRegularItemsFromBox(string boxName)
		{
			List<string> list = new List<string>();
			RandomBoxDesc randomBoxDesc = this.m_itemStats.GetRandomBoxesDesc().FirstOrDefault((RandomBoxDesc r) => r.Name == boxName);
			if (randomBoxDesc != null)
			{
				foreach (RandomBoxDesc.Group group in randomBoxDesc.Groups)
				{
					foreach (RandomBoxDesc.Choice choice in group.Choices)
					{
						if (choice.IsRegular)
						{
							list.Add(choice.Name);
						}
						list.AddRange(this.GetRegularItemsFromBundle(choice.Name));
					}
				}
			}
			list.AddRange(this.GetRegularItemsFromBundle(boxName));
			return list;
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000BFFC File Offset: 0x0000A3FC
		private IEnumerable<string> GetRegularItemsFromBundle(string bundleName)
		{
			List<string> list = new List<string>();
			IList<BundleDesc> bundlesDesc = this.m_itemStats.GetBundlesDesc();
			BundleDesc bundleDesc = bundlesDesc.FirstOrDefault((BundleDesc b) => b.Name == bundleName);
			if (bundleDesc != null)
			{
				list.AddRange(from i in bundleDesc.Items
				where i.IsRegular
				select i into it
				select it.Name);
			}
			return list;
		}

		// Token: 0x040000EE RID: 238
		private readonly IProfileItems m_profileItems;

		// Token: 0x040000EF RID: 239
		private readonly IConfigProvider<RandomBoxChoiceLimitationConfig> m_configProvider;

		// Token: 0x040000F0 RID: 240
		private readonly IItemStats m_itemStats;
	}
}
