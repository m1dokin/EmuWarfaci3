using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020007BB RID: 1979
	[QueryAttributes(TagName = "sponsor_info_updated")]
	internal class SponsorInfoUpdatedQuery : BaseQuery
	{
		// Token: 0x06002896 RID: 10390 RVA: 0x000AE960 File Offset: 0x000ACD60
		public SponsorInfoUpdatedQuery(IItemCache itemCache, IRewardService rewardService)
		{
			this.m_itemCache = itemCache;
			this.m_rewardService = rewardService;
		}

		// Token: 0x06002897 RID: 10391 RVA: 0x000AE978 File Offset: 0x000ACD78
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SponsorInfoUpdatedQuery"))
			{
				Dictionary<ulong, SItem> allItems = this.m_itemCache.GetAllItems();
				RewardUpdateData rewardUpdateData = queryParams[0] as RewardUpdateData;
				request.SetAttribute("sponsor_id", rewardUpdateData.rewards.SponsorData.sponsorId.ToString(CultureInfo.InvariantCulture));
				request.SetAttribute("sponsor_points", rewardUpdateData.rewards.gainedSponsorPoints.ToString(CultureInfo.InvariantCulture));
				request.SetAttribute("total_sponsor_points", rewardUpdateData.rewards.SponsorData.totalSponsorPoints.ToString(CultureInfo.InvariantCulture));
				string value = string.Empty;
				if (allItems.ContainsKey(rewardUpdateData.rewards.SponsorData.nextUnlockItemId))
				{
					value = allItems[rewardUpdateData.rewards.SponsorData.nextUnlockItemId].Name;
				}
				request.SetAttribute("next_unlock_item", value);
				XmlElement unlockedItemsXml = this.m_rewardService.GetUnlockedItemsXml(request.OwnerDocument, rewardUpdateData.rewards.profileId, rewardUpdateData.rewards.SponsorData.unlockedItems);
				request.AppendChild(unlockedItemsXml);
			}
		}

		// Token: 0x04001589 RID: 5513
		private readonly IItemCache m_itemCache;

		// Token: 0x0400158A RID: 5514
		private readonly IRewardService m_rewardService;
	}
}
