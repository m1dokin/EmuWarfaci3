using System;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.GameModes;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig
{
	// Token: 0x02000087 RID: 135
	[Service]
	[Singleton]
	internal class GameModeFirstWinOfDayBonusConfigProvider : ServiceModule, IConfigProvider<GameModeFirstWinOfDayBonusConfig>
	{
		// Token: 0x060001FB RID: 507 RVA: 0x0000B93C File Offset: 0x00009D3C
		public GameModeFirstWinOfDayBonusConfigProvider(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x060001FC RID: 508 RVA: 0x0000B94C File Offset: 0x00009D4C
		// (remove) Token: 0x060001FD RID: 509 RVA: 0x0000B984 File Offset: 0x00009D84
		public event Action<GameModeFirstWinOfDayBonusConfig> Changed;

		// Token: 0x060001FE RID: 510 RVA: 0x0000B9BC File Offset: 0x00009DBC
		public override void Init()
		{
			this.InitConfig();
			ConfigSection configData = this.GetConfigData();
			configData.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000B9E8 File Offset: 0x00009DE8
		public override void Stop()
		{
			ConfigSection configData = this.GetConfigData();
			configData.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000BA0E File Offset: 0x00009E0E
		public GameModeFirstWinOfDayBonusConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000BA18 File Offset: 0x00009E18
		private void InitConfig()
		{
			ConfigSection configData = this.GetConfigData();
			GameModeFirstWinOfDayBonusConfigParser gameModeFirstWinOfDayBonusConfigParser = new GameModeFirstWinOfDayBonusConfigParser();
			GameModeFirstWinOfDayBonusConfig gameModeFirstWinOfDayBonusConfig = gameModeFirstWinOfDayBonusConfigParser.Parse(configData);
			GameModeFirstWinOfDayBonusConfigValidator gameModeFirstWinOfDayBonusConfigValidator = new GameModeFirstWinOfDayBonusConfigValidator(this.m_gameModesSystem);
			gameModeFirstWinOfDayBonusConfigValidator.Validate(gameModeFirstWinOfDayBonusConfig);
			Interlocked.Exchange<GameModeFirstWinOfDayBonusConfig>(ref this.m_config, gameModeFirstWinOfDayBonusConfig);
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000BA5C File Offset: 0x00009E5C
		private ConfigSection GetConfigData()
		{
			return Resources.Rewards.GetSection("GameModeFirstWinOfDayBonus");
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000BA7A File Offset: 0x00009E7A
		private void OnConfigChanged(ConfigEventArgs args)
		{
			this.InitConfig();
		}

		// Token: 0x040000E3 RID: 227
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x040000E4 RID: 228
		private GameModeFirstWinOfDayBonusConfig m_config;
	}
}
