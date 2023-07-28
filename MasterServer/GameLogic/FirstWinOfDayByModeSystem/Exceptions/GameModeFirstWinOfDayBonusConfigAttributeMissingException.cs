using System;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions
{
	// Token: 0x0200007F RID: 127
	public class GameModeFirstWinOfDayBonusConfigAttributeMissingException : FirstWinOfDayByModeSystemException
	{
		// Token: 0x060001DE RID: 478 RVA: 0x0000B488 File Offset: 0x00009888
		public GameModeFirstWinOfDayBonusConfigAttributeMissingException(string attributeName) : base(GameModeFirstWinOfDayBonusConfigAttributeMissingException.GetExceptionMessage(attributeName))
		{
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000B496 File Offset: 0x00009896
		private static string GetExceptionMessage(string attributeName)
		{
			return string.Format("Mandatory attribute '{0}' in GameModeFirstWinOfDayBonus config in rewards_configuration.xml is missing", attributeName);
		}
	}
}
