using System;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem.RewardCalculators.ClanPointCalculator
{
	// Token: 0x020000E5 RID: 229
	internal class ClanPointParseConfigurationException : ApplicationException
	{
		// Token: 0x060003C8 RID: 968 RVA: 0x000109CE File Offset: 0x0000EDCE
		internal ClanPointParseConfigurationException(string configName, string attribName, GameRoomType roomTypeName) : base(ClanPointParseConfigurationException.CreateMessage(attribName, configName, roomTypeName))
		{
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x000109DE File Offset: 0x0000EDDE
		private static string CreateMessage(string attribName, string configName, GameRoomType roomTypeName)
		{
			return string.Format("Can't parse attribute {0} for {1} of {2} section in rewards_configuration.xml", attribName, roomTypeName, configName);
		}
	}
}
