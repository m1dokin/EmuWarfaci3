using System;
using System.Xml;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B5 RID: 1461
	internal interface IRewardPool
	{
		// Token: 0x06001F5E RID: 8030
		bool TryParse(XmlTextReader reader);

		// Token: 0x06001F5F RID: 8031
		void Dump();

		// Token: 0x06001F60 RID: 8032
		bool IsValid();

		// Token: 0x06001F61 RID: 8033
		void ToDefault();
	}
}
