using System;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions
{
	// Token: 0x0200007D RID: 125
	public class GameModeFirstWinOfDayBonusConfigInvalidValueException : FirstWinOfDayByModeSystemException
	{
		// Token: 0x060001DA RID: 474 RVA: 0x0000B44F File Offset: 0x0000984F
		public GameModeFirstWinOfDayBonusConfigInvalidValueException(string attributeName, string attributeContent, Exception innerException) : base(GameModeFirstWinOfDayBonusConfigInvalidValueException.GetExceptionMessage(attributeName, attributeContent), innerException)
		{
		}

		// Token: 0x060001DB RID: 475 RVA: 0x0000B45F File Offset: 0x0000985F
		private static string GetExceptionMessage(string attributeName, string attributeContent)
		{
			return string.Format("Attribute '{0}' in GameModeFirstWinOfDayBonus config in rewards_configuration.xml has incorrect value: '{1}'", attributeName, attributeContent);
		}
	}
}
