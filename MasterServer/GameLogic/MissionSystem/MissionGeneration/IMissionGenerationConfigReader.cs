using System;
using HK2Net;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.MissionSystem.MissionGeneration
{
	// Token: 0x020003A5 RID: 933
	[Contract]
	public interface IMissionGenerationConfigReader
	{
		// Token: 0x060014B3 RID: 5299
		Config Read();
	}
}
