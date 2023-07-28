using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005BC RID: 1468
	[Contract]
	internal interface IRewardService
	{
		// Token: 0x14000078 RID: 120
		// (add) Token: 0x06001F7A RID: 8058
		// (remove) Token: 0x06001F7B RID: 8059
		event OnRewardsGivenDeleg OnRewardsGiven;

		// Token: 0x06001F7C RID: 8060
		XmlElement GetUnlockedItemsXml(XmlDocument factory, ulong profileId, IEnumerable<SponsorDataUpdate.ItemIDs> unlockedItems);

		// Token: 0x06001F7D RID: 8061
		void GiveRewards(string sessionId, MissionContext missionContext, RewardInputData rewardData);

		// Token: 0x06001F7E RID: 8062
		void RewardMoney(ulong userId, ulong profileId, Currency currency, ulong rewardMoney, ILogGroup logGroup, LogGroup.ProduceType rewardSource);

		// Token: 0x1700033E RID: 830
		// (get) Token: 0x06001F7F RID: 8063
		TimeSpan AwardExpirationTime { get; }
	}
}
