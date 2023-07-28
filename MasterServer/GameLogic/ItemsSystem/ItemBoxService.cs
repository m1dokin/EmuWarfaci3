using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation;
using MasterServer.GameLogic.ItemsSystem.WinModels;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000345 RID: 837
	[Service]
	[Singleton]
	internal class ItemBoxService : ServiceModule, IItemBoxService
	{
		// Token: 0x060012B6 RID: 4790 RVA: 0x0004B4F8 File Offset: 0x000498F8
		public ItemBoxService(IWinModelFactory winModelFactory, IProfileItems profileItems, IRandomBoxChoiceLimitationService randomBoxChoiceLimitation, IItemStats itemStats)
		{
			this.m_winModelFactory = winModelFactory;
			this.m_profileItems = profileItems;
			this.m_randomBoxChoiceLimitation = randomBoxChoiceLimitation;
			this.m_itemStats = itemStats;
			this.m_randomer = new Random((int)DateTime.Now.Ticks);
		}

		// Token: 0x060012B7 RID: 4791 RVA: 0x0004B541 File Offset: 0x00049941
		public override void Init()
		{
			base.Init();
			this.m_currentWinModel = this.m_winModelFactory.GetWinModel();
			this.m_currentWinModel.Init();
		}

		// Token: 0x060012B8 RID: 4792 RVA: 0x0004B565 File Offset: 0x00049965
		public override void Stop()
		{
			this.m_currentWinModel.Dispose();
		}

		// Token: 0x060012B9 RID: 4793 RVA: 0x0004B574 File Offset: 0x00049974
		public IEnumerable<RandomBoxDesc.Choice> OpenRandomBox(ulong userId, ulong profileId, RandomBoxDesc desc)
		{
			List<RandomBoxDesc.Choice> list = new List<RandomBoxDesc.Choice>();
			foreach (RandomBoxDesc.Group group in desc.Groups)
			{
				if (group.Choices.Count > 0)
				{
					RandomBoxDesc.Choice choice = group.Choices.FirstOrDefault((RandomBoxDesc.Choice c) => c.HasTopPrizeTokenDefined());
					if (choice != null && this.m_currentWinModel.AddPrizeToken(userId, choice.TopPrizeToken) >= choice.WinLimit)
					{
						list.Add(choice);
						this.m_currentWinModel.ResetPrizeTokensCount(userId, profileId, choice.TopPrizeToken);
					}
					else
					{
						float num = 0f;
						List<RandomBoxDesc.Choice> list2 = group.Choices.ToList<RandomBoxDesc.Choice>();
						double normalizedSeed = this.GetNormalizedSeed(profileId, list2);
						foreach (RandomBoxDesc.Choice choice2 in list2)
						{
							num += choice2.Weight;
							if (normalizedSeed <= (double)num)
							{
								list.Add(choice2);
								if (choice2.HasTopPrizeTokenDefined())
								{
									this.m_currentWinModel.ResetPrizeTokensCount(userId, profileId, choice2.TopPrizeToken);
								}
								break;
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x060012BA RID: 4794 RVA: 0x0004B708 File Offset: 0x00049B08
		public IEnumerable<BundleDesc.BundledItem> OpenBundle(ulong profileId, ulong itemId)
		{
			Dictionary<ulong, SProfileItem>.ValueCollection profileItems = this.m_profileItems.GetProfileItems(profileId).Values;
			return from item in this.m_itemStats.GetBundleDesc(itemId).Items
			where this.IsChoiceAvailable(item, profileItems)
			select item;
		}

		// Token: 0x060012BB RID: 4795 RVA: 0x0004B75C File Offset: 0x00049B5C
		private double GetNormalizedSeed(ulong profileId, IList<RandomBoxDesc.Choice> choices)
		{
			object randomer = this.m_randomer;
			double num;
			lock (randomer)
			{
				num = this.m_randomer.NextDouble();
			}
			if (!this.m_randomBoxChoiceLimitation.Enabled)
			{
				return num;
			}
			double num2 = 0.0;
			List<RandomBoxDesc.Choice> source = choices.ToList<RandomBoxDesc.Choice>();
			Dictionary<ulong, SProfileItem>.ValueCollection profileItems = this.m_profileItems.GetProfileItems(profileId).Values;
			foreach (RandomBoxDesc.Choice choice in from ch in source
			where !this.IsChoiceAvailable(ch, profileItems)
			select ch)
			{
				num2 += (double)choice.Weight;
				choices.Remove(choice);
			}
			double num3 = num - num2;
			return (num3 >= 0.0) ? num3 : 0.0;
		}

		// Token: 0x060012BC RID: 4796 RVA: 0x0004B880 File Offset: 0x00049C80
		private bool IsChoiceAvailable(IGenericItem item, IEnumerable<SProfileItem> profileItems)
		{
			if (!this.m_randomBoxChoiceLimitation.Enabled)
			{
				return true;
			}
			if (item.IsRegular)
			{
				return !profileItems.Any((SProfileItem n) => n.GameItem.Name == item.Name && n.OfferType == OfferType.Regular);
			}
			BundleDesc bundleDesc = this.m_itemStats.GetBundlesDesc().FirstOrDefault((BundleDesc b) => b.Name == item.Name);
			if (bundleDesc == null)
			{
				return true;
			}
			List<string> source = (from bi in bundleDesc.Items
			where bi.IsRegular
			select bi into i
			select i.Name).ToList<string>();
			int num = source.Count((string itemName) => profileItems.Any((SProfileItem pi) => pi.GameItem.Name == itemName && pi.OfferType == OfferType.Regular));
			return num == 0 || num != source.Count<string>();
		}

		// Token: 0x040008A5 RID: 2213
		private readonly IWinModelFactory m_winModelFactory;

		// Token: 0x040008A6 RID: 2214
		private readonly IProfileItems m_profileItems;

		// Token: 0x040008A7 RID: 2215
		private readonly IRandomBoxChoiceLimitationService m_randomBoxChoiceLimitation;

		// Token: 0x040008A8 RID: 2216
		private readonly IItemStats m_itemStats;

		// Token: 0x040008A9 RID: 2217
		private readonly Random m_randomer;

		// Token: 0x040008AA RID: 2218
		private IWinModel m_currentWinModel;
	}
}
