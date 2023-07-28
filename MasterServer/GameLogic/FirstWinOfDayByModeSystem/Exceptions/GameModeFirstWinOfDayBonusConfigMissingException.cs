using System;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions
{
	// Token: 0x02000081 RID: 129
	public class GameModeFirstWinOfDayBonusConfigMissingException : FirstWinOfDayByModeSystemException
	{
		// Token: 0x060001E2 RID: 482 RVA: 0x0000B4A3 File Offset: 0x000098A3
		public GameModeFirstWinOfDayBonusConfigMissingException() : base("GameModeFirstWinOfDayBonus config is missing from rewards_configuration.xml")
		{
		}
	}
}
