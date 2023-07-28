using System;
using System.Xml;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D6 RID: 1494
	[Contract]
	internal interface ITutorialStatsTracker
	{
		// Token: 0x1400007E RID: 126
		// (add) Token: 0x06001FD9 RID: 8153
		// (remove) Token: 0x06001FDA RID: 8154
		event OnTutorialCompletedDeleg OnTutorialCompleted;

		// Token: 0x06001FDB RID: 8155
		void TrackEvent(TutorialEvent eventType, UserInfo.User user, int tutorialId, string tutorialStep, XmlElement response);
	}
}
