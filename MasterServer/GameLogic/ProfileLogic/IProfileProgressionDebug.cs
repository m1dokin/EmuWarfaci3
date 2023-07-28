using System;
using HK2Net;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000560 RID: 1376
	[Contract]
	internal interface IProfileProgressionDebug
	{
		// Token: 0x06001DC2 RID: 7618
		void DumpRules();

		// Token: 0x06001DC3 RID: 7619
		void UpdateTutorialUnlockPlaytime(int index, TimeSpan time);
	}
}
