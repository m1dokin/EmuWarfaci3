using System;
using MasterServer.Core.Configs;

namespace MasterServer.Core
{
	// Token: 0x02000036 RID: 54
	public class MsConfigInfo : ConfigInfo
	{
		// Token: 0x060000CD RID: 205 RVA: 0x00007E08 File Offset: 0x00006208
		private MsConfigInfo(string name, string path, Resources.ResFiles resource, bool optional = false) : base(name, path, optional)
		{
			this.Resource = resource;
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000CE RID: 206 RVA: 0x00007E1B File Offset: 0x0000621B
		// (set) Token: 0x060000CF RID: 207 RVA: 0x00007E23 File Offset: 0x00006223
		public Resources.ResFiles Resource { get; private set; }

		// Token: 0x04000060 RID: 96
		public static readonly ConfigInfo CommonConfiguration = new MsConfigInfo("configuration", ConfigInfo.GetRelatedFilePath("configuration.xml"), Resources.ResFiles.COMMON_CONFIG, false);

		// Token: 0x04000061 RID: 97
		public static readonly ConfigInfo ECatalogConfiguration = new MsConfigInfo("ecat_configuration", ConfigInfo.GetRelatedFilePath("ecat_configuration.xml"), Resources.ResFiles.ECAT_CONFIG, false);

		// Token: 0x04000062 RID: 98
		public static readonly ConfigInfo XmppConfiguration = new MsConfigInfo("xmpp_configuration", ConfigInfo.GetRelatedFilePath("xmpp_configuration.xml"), Resources.ResFiles.XMPP_CONFIG, false);

		// Token: 0x04000063 RID: 99
		public static readonly ConfigInfo DbMasterConfiguration = new MsConfigInfo("db_configuration", ConfigInfo.GetRelatedFilePath("db_configuration.xml"), Resources.ResFiles.DB_MASTER_CONFIG, false);

		// Token: 0x04000064 RID: 100
		public static readonly ConfigInfo DbSlaveConfiguration = new MsConfigInfo("slave_configuration", ConfigInfo.GetRelatedFilePath("slave_configuration.xml"), Resources.ResFiles.DB_SLAVE_CONFIG, true);

		// Token: 0x04000065 RID: 101
		public static readonly ConfigInfo OnlineVariablesConfiguration = new MsConfigInfo("online_variables", ConfigInfo.GetRelatedFilePath("online_variables.xml"), Resources.ResFiles.OV_CONFIG, false);

		// Token: 0x04000066 RID: 102
		public static readonly ConfigInfo RewardsConfiguration = new MsConfigInfo("rewards_configuration", ConfigInfo.GetRelatedFilePath("rewards_configuration.xml"), Resources.ResFiles.REWARDS_CONFIG, false);

		// Token: 0x04000067 RID: 103
		public static readonly ConfigInfo QosConfiguration = new MsConfigInfo("qos_configuration", ConfigInfo.GetRelatedFilePath("qos_configuration.xml"), Resources.ResFiles.QOS_CONFIG, false);

		// Token: 0x04000068 RID: 104
		public static readonly ConfigInfo LbConfiguration = new MsConfigInfo("lb_configuration", ConfigInfo.GetRelatedFilePath("lb_configuration.xml"), Resources.ResFiles.LB_CONFIG, false);

		// Token: 0x04000069 RID: 105
		public static readonly ConfigInfo AnticheatConfiguration = new MsConfigInfo("anticheat_configuration", ConfigInfo.GetRelatedFilePath("anticheat_configuration.xml"), Resources.ResFiles.ANTICHEAT_CONFIG, false);

		// Token: 0x0400006A RID: 106
		public static readonly ConfigInfo AbuseConfiguration = new MsConfigInfo("abuse_manager_config", ConfigInfo.GetRelatedFilePath("abuse_manager_config.xml"), Resources.ResFiles.ABUSE_CONFIG, false);

		// Token: 0x0400006B RID: 107
		public static readonly ConfigInfo ProgressionConfiguration = new MsConfigInfo("profile_progression_config", ConfigInfo.GetRelatedFilePath("profile_progression_config.xml"), Resources.ResFiles.PROF_PROGRESSION_CONFIG, false);

		// Token: 0x0400006C RID: 108
		public static readonly ConfigInfo SpecialRewardConfiguration = new MsConfigInfo("special_reward_configuration", ConfigInfo.GetRelatedFilePath("special_reward_configuration.xml"), Resources.ResFiles.SPECIAL_REWARD_CONFIG, false);

		// Token: 0x0400006D RID: 109
		public static readonly ConfigInfo QuickplayConfiguration = new MsConfigInfo("quickplay_maps", ConfigInfo.GetRelatedFilePath("quickplay_maps.xml"), Resources.ResFiles.QUICKPLAY_CONFIG, true);

		// Token: 0x0400006E RID: 110
		public static readonly ConfigInfo CustomRulesConfiguration = new MsConfigInfo("custom_rules", ConfigInfo.GetRelatedFilePath("custom_rules.xml"), Resources.ResFiles.CUSTOM_RULES, true);

		// Token: 0x0400006F RID: 111
		public static readonly ConfigInfo GFaceConfiguration = new MsConfigInfo("gface_configuration", ConfigInfo.GetRelatedFilePath("gface_configuration.xml"), Resources.ResFiles.GFACE_CONFIG, false);

		// Token: 0x04000070 RID: 112
		public static readonly ConfigInfo BanRequestsConfiguration = new MsConfigInfo("ban_requests_config", ConfigInfo.GetRelatedFilePath("ban_requests_config.xml"), Resources.ResFiles.BAN_REQUESTS_CONFIG, true);

		// Token: 0x04000071 RID: 113
		public static readonly ConfigInfo RatingCurveConfiguration = new MsConfigInfo("rating_curve", ConfigInfo.GetRelatedFilePath("rating_curve.xml"), Resources.ResFiles.RATING_CURVE_CONFIG, false);

		// Token: 0x04000072 RID: 114
		public static readonly ConfigInfo MissionGenerationConfiguration = new MsConfigInfo("mission_generation_configuration", ConfigInfo.GetRelatedFilePath("mission_generation_configuration.xml"), Resources.ResFiles.MISSION_GENERATION_CONFIG, false);
	}
}
