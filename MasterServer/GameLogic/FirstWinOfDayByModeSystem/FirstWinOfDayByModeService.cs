using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL.FirstWinOfDayByMode;
using MasterServer.Database;
using MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig;
using NCrontab;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem
{
	// Token: 0x02000084 RID: 132
	[Service]
	[Singleton]
	internal class FirstWinOfDayByModeService : ServiceModule, IFirstWinOfDayByModeService, IDebugFirstWinOfDayByModeService
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x0000B4B0 File Offset: 0x000098B0
		public FirstWinOfDayByModeService(IDALService dalService, IConfigProvider<GameModeFirstWinOfDayBonusConfig> gameModeFirstWinOfDayBonusConfigProvider)
		{
			this.m_dalService = dalService;
			this.m_gameModeFirstWinOfDayBonusConfigProvider = gameModeFirstWinOfDayBonusConfigProvider;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000B4C8 File Offset: 0x000098C8
		public bool SetPvpModeFirstWin(ulong profileId, string mode)
		{
			GameModeFirstWinOfDayBonusConfig gameModeFirstWinOfDayBonusConfig = this.m_gameModeFirstWinOfDayBonusConfigProvider.Get();
			if (gameModeFirstWinOfDayBonusConfig.Enabled)
			{
				DateTime nextWinsResetOccurrence = this.GetNextWinsResetOccurrence();
				return this.m_dalService.FirstWinOfDayByModeSystem.SetPvpModeFirstWin(profileId, mode, nextWinsResetOccurrence);
			}
			return false;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000B50C File Offset: 0x0000990C
		public IEnumerable<string> GetPvpModesWithBonusAvailable(ulong profileId)
		{
			GameModeFirstWinOfDayBonusConfig gameModeFirstWinOfDayBonusConfig = this.m_gameModeFirstWinOfDayBonusConfigProvider.Get();
			ICollection<string> keys = gameModeFirstWinOfDayBonusConfig.ModesBonus.Keys;
			DateTime nextOccurrence = this.GetNextWinsResetOccurrence();
			IEnumerable<string> result = Enumerable.Empty<string>();
			if (gameModeFirstWinOfDayBonusConfig.Enabled && keys.Any<string>())
			{
				IEnumerable<PvpModeWinNextOccurrence> pvpModesWinNextOccurrence = this.m_dalService.FirstWinOfDayByModeSystem.GetPvpModesWinNextOccurrence(profileId);
				IEnumerable<string> second = from x in pvpModesWinNextOccurrence
				where x.NextOccurence >= nextOccurrence
				select x.Mode;
				result = keys.Except(second);
			}
			return result;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000B5B3 File Offset: 0x000099B3
		public void ResetPvpModesFirstWin(ulong profileId)
		{
			this.m_dalService.FirstWinOfDayByModeSystem.ResetPvpModesFirstWin(profileId);
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000B5C8 File Offset: 0x000099C8
		private DateTime GetNextWinsResetOccurrence()
		{
			GameModeFirstWinOfDayBonusConfig gameModeFirstWinOfDayBonusConfig = this.m_gameModeFirstWinOfDayBonusConfigProvider.Get();
			CrontabSchedule resetSchedule = gameModeFirstWinOfDayBonusConfig.ResetSchedule;
			return resetSchedule.GetNextOccurrence(DateTime.Now);
		}

		// Token: 0x040000DD RID: 221
		private readonly IDALService m_dalService;

		// Token: 0x040000DE RID: 222
		private readonly IConfigProvider<GameModeFirstWinOfDayBonusConfig> m_gameModeFirstWinOfDayBonusConfigProvider;
	}
}
