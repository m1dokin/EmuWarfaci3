using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x02000598 RID: 1432
	[Service]
	internal class CrownRewardMissionGeneratorVisitor : IMissionGeneratorVisitor
	{
		// Token: 0x06001EDB RID: 7899 RVA: 0x0007D2C8 File Offset: 0x0007B6C8
		public int Visit()
		{
			int num = 0;
			ConfigSection section = Resources.Rewards.GetSection("CrownRewards");
			foreach (ConfigSection configSection in section.GetSections("Reward"))
			{
				string text = configSection.Get("type");
				int num2 = int.Parse(configSection.Get("bronze"));
				int num3 = int.Parse(configSection.Get("silver"));
				int num4 = int.Parse(configSection.Get("gold"));
				num ^= (text.GetHashCode() ^ (3 ^ num2) ^ (2 ^ num3) ^ (1 ^ num4));
			}
			return num;
		}
	}
}
