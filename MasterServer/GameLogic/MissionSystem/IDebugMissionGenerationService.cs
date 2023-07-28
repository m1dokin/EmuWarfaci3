using System;
using HK2Net;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B2 RID: 946
	[Contract]
	internal interface IDebugMissionGenerationService
	{
		// Token: 0x060014F8 RID: 5368
		void DebugValidateMissionGraphs();

		// Token: 0x060014F9 RID: 5369
		void DebugEmulateRotation(int elementsNum, int shufflesNum);

		// Token: 0x060014FA RID: 5370
		void DebugDumpMissionSet();

		// Token: 0x060014FB RID: 5371
		void DebugDumpMissionSetContent();
	}
}
