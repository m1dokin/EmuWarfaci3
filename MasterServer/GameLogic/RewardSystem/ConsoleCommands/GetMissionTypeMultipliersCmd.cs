using System;
using System.Text;
using MasterServer.Core;
using MasterServer.Core.Services.Configuration;

namespace MasterServer.GameLogic.RewardSystem.ConsoleCommands
{
	// Token: 0x020000CC RID: 204
	[DebugCommand]
	[ConsoleCmdAttributes(CmdName = "get_mission_type_multipliers", Help = "Get mission type reward multipliers")]
	internal class GetMissionTypeMultipliersCmd : ConsoleCommand<GetMissionTypeMultipliersCmdParams>
	{
		// Token: 0x0600034E RID: 846 RVA: 0x0000F302 File Offset: 0x0000D702
		public GetMissionTypeMultipliersCmd(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0000F314 File Offset: 0x0000D714
		protected override void Execute(GetMissionTypeMultipliersCmdParams param)
		{
			SRewardMultiplier missionTypeMultiplier = ApplyMultipliersCalculator.GetMissionTypeMultiplier(this.m_configurationService, param.MissionType);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("MissionTypeMultipliers:");
			stringBuilder.AppendLine(string.Format("Experience = {0}", missionTypeMultiplier.ExperienceMultiplier));
			stringBuilder.AppendLine(string.Format("GameMoney = {0}", missionTypeMultiplier.MoneyMultiplier));
			stringBuilder.AppendLine(string.Format("VendorPoints = {0}", missionTypeMultiplier.SponsorPointsMultiplier));
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x04000166 RID: 358
		private readonly IConfigurationService m_configurationService;
	}
}
