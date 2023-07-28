using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005C5 RID: 1477
	[Contract]
	internal interface ISpecialProfileRewardService
	{
		// Token: 0x06001FA1 RID: 8097
		List<SNotification> ProcessEvent(string setName, ulong profileId, ILogGroup logGroup);

		// Token: 0x06001FA2 RID: 8098
		List<SNotification> ProcessEvent(string setName, ulong profileId, ILogGroup logGroup, XmlElement userData);

		// Token: 0x06001FA3 RID: 8099
		RewardSet GetRewardSet(string setName);
	}
}
