using System;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem.RewardCalculators.ClanPointCalculator
{
	// Token: 0x020000E6 RID: 230
	internal class ClanPointParseValidationException : ApplicationException
	{
		// Token: 0x060003CA RID: 970 RVA: 0x000109F2 File Offset: 0x0000EDF2
		public ClanPointParseValidationException(string configName, int groupSize, float multiplier, GameRoomType gameRoomType) : base(ClanPointParseValidationException.CreateMessage(configName, groupSize, multiplier, gameRoomType))
		{
		}

		// Token: 0x060003CB RID: 971 RVA: 0x00010A04 File Offset: 0x0000EE04
		private static string CreateMessage(string configName, int groupSize, float multiplier, GameRoomType gameRoomType)
		{
			return string.Format("For a group of size: {0}, a value of multiplier: {1} is smaller than for a smaller group size in a desctiption of the {2} in section {3} in rewards_configuration.xml", new object[]
			{
				groupSize,
				multiplier,
				gameRoomType,
				configName
			});
		}
	}
}
