using System;
using System.Collections.Generic;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions;
using MasterServer.GameLogic.GameModes;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig
{
	// Token: 0x02000088 RID: 136
	internal class GameModeFirstWinOfDayBonusConfigValidator
	{
		// Token: 0x06000204 RID: 516 RVA: 0x0000BA82 File Offset: 0x00009E82
		public GameModeFirstWinOfDayBonusConfigValidator(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000BA91 File Offset: 0x00009E91
		public void Validate(GameModeFirstWinOfDayBonusConfig firstWinBonusConfig)
		{
			this.ValidateAtLeastOneModeIsConfigured(firstWinBonusConfig);
			this.ValidateModeExistence(firstWinBonusConfig);
			this.ValidateSomeRewardConfigured(firstWinBonusConfig);
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000BAA8 File Offset: 0x00009EA8
		private void ValidateAtLeastOneModeIsConfigured(GameModeFirstWinOfDayBonusConfig firstWinBonusConfig)
		{
			if (firstWinBonusConfig.ModesBonus.Count == 0 && firstWinBonusConfig.Enabled)
			{
				string message = string.Format("At least one mode should be configured in GameModeFirstWinOfDayBonus config in rewards_configuration.xml", new object[0]);
				throw new GameModeFirstWinOfDayBonusInvalidConfigException(message);
			}
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000BAE8 File Offset: 0x00009EE8
		private void ValidateModeExistence(GameModeFirstWinOfDayBonusConfig firstWinBonusConfig)
		{
			foreach (string mode in firstWinBonusConfig.ModesBonus.Keys)
			{
				if (this.m_gameModesSystem.GetGameModeSetting(mode) == null)
				{
					throw new UnknownGameModeException(mode);
				}
			}
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000BB5C File Offset: 0x00009F5C
		private void ValidateSomeRewardConfigured(GameModeFirstWinOfDayBonusConfig firstWinBonusConfig)
		{
			foreach (KeyValuePair<string, GameModeBonus> keyValuePair in firstWinBonusConfig.ModesBonus)
			{
				string key = keyValuePair.Key;
				if (keyValuePair.Value.Bonus == 0U)
				{
					string message = string.Format("Minimal reward should be configured for '{0}' mode in GameModeFirstWinOfDayBonus config in rewards_configuration.xml", key);
					throw new GameModeFirstWinOfDayBonusInvalidConfigException(message);
				}
			}
		}

		// Token: 0x040000E6 RID: 230
		private readonly IGameModesSystem m_gameModesSystem;
	}
}
