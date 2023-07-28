using System;
using MasterServer.Core;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D5 RID: 1493
	// (Invoke) Token: 0x06001FD6 RID: 8150
	internal delegate void OnTutorialCompletedDeleg(ulong profileId, int tutorialId, ref ProfileProgressionInfo output, ILogGroup logGroup);
}
