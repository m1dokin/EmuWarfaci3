using System;
using System.Collections.Generic;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem.Exceptions;
using NCrontab;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig
{
	// Token: 0x02000086 RID: 134
	public class GameModeFirstWinOfDayBonusConfigParser
	{
		// Token: 0x060001F5 RID: 501 RVA: 0x0000B720 File Offset: 0x00009B20
		public GameModeFirstWinOfDayBonusConfig Parse(ConfigSection gameModeFirstWinOfDayBonusConfigData)
		{
			if (gameModeFirstWinOfDayBonusConfigData == null)
			{
				throw new GameModeFirstWinOfDayBonusConfigMissingException();
			}
			List<ConfigSection> modesBonusConfigSections;
			gameModeFirstWinOfDayBonusConfigData.TryGetSections("Mode", out modesBonusConfigSections);
			bool enabled = this.ParseEnabled(gameModeFirstWinOfDayBonusConfigData);
			CrontabSchedule resetSchedule = this.ParseResetSchedule(gameModeFirstWinOfDayBonusConfigData);
			Dictionary<string, GameModeBonus> modesBonus = this.ParseModesBonus(modesBonusConfigSections);
			return new GameModeFirstWinOfDayBonusConfig
			{
				Enabled = enabled,
				ResetSchedule = resetSchedule,
				ModesBonus = modesBonus
			};
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000B784 File Offset: 0x00009B84
		private void CheckConfigAttributePresence(ConfigSection config, string attributeName)
		{
			if (!config.HasValue(attributeName))
			{
				throw new GameModeFirstWinOfDayBonusConfigAttributeMissingException(attributeName);
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000B79C File Offset: 0x00009B9C
		private bool ParseEnabled(ConfigSection config)
		{
			this.CheckConfigAttributePresence(config, "enabled");
			bool result;
			try
			{
				config.Get("enabled", out result);
			}
			catch (Exception innerException)
			{
				string attributeContent = config.Get("enabled");
				throw new GameModeFirstWinOfDayBonusConfigInvalidValueException("enabled", attributeContent, innerException);
			}
			return result;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000B7F4 File Offset: 0x00009BF4
		private CrontabSchedule ParseResetSchedule(ConfigSection config)
		{
			this.CheckConfigAttributePresence(config, "reset_schedule");
			string text = config.Get("reset_schedule");
			CrontabSchedule result;
			try
			{
				text = config.Get("reset_schedule");
				result = CrontabSchedule.Parse(text);
			}
			catch (Exception innerException)
			{
				throw new GameModeFirstWinOfDayBonusConfigInvalidValueException("reset_schedule", text, innerException);
			}
			return result;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000B850 File Offset: 0x00009C50
		private Dictionary<string, GameModeBonus> ParseModesBonus(IEnumerable<ConfigSection> modesBonusConfigSections)
		{
			Dictionary<string, GameModeBonus> dictionary = new Dictionary<string, GameModeBonus>();
			if (modesBonusConfigSections != null)
			{
				foreach (ConfigSection configSection in modesBonusConfigSections)
				{
					this.CheckConfigAttributePresence(configSection, "name");
					string key = configSection.Get("name");
					uint bonus = this.ParseBonusAmount(configSection, "bonus");
					GameModeBonus value = new GameModeBonus
					{
						Bonus = bonus
					};
					dictionary.Add(key, value);
				}
			}
			return dictionary;
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000B8F4 File Offset: 0x00009CF4
		private uint ParseBonusAmount(ConfigSection config, string attributeName)
		{
			this.CheckConfigAttributePresence(config, attributeName);
			string text = config.Get(attributeName);
			uint result;
			try
			{
				uint num = uint.Parse(text);
				result = num;
			}
			catch (Exception innerException)
			{
				throw new GameModeFirstWinOfDayBonusConfigInvalidValueException(attributeName, text, innerException);
			}
			return result;
		}
	}
}
