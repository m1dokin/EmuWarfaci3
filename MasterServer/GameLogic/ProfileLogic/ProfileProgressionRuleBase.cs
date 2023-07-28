using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;
using OLAPHypervisor;
using Util.Common;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000402 RID: 1026
	internal abstract class ProfileProgressionRuleBase : IProfileProgressionRule, IDisposable
	{
		// Token: 0x0600162C RID: 5676 RVA: 0x0005D1F4 File Offset: 0x0005B5F4
		protected ProfileProgressionRuleBase(IProfileProgressionService progressionService, IUserRepository userRepository, ISpecialProfileRewardService specialRewards, IPlayerStatsService playerStatsService, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage)
		{
			this.ProgressionService = progressionService;
			this.UserRepository = userRepository;
			this.DalService = dalService;
			this.m_specialRewards = specialRewards;
			this.m_playerStatsService = playerStatsService;
			this.m_catalogService = catalogService;
			this.m_profileItemsService = profileItemsService;
			this.m_customRulesService = customRulesService;
			this.m_customRulesStateStorage = customRulesStateStorage;
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x0600162D RID: 5677 RVA: 0x0005D24C File Offset: 0x0005B64C
		protected bool Enabled
		{
			get
			{
				return this.ProgressionService.IsEnabled;
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x0600162F RID: 5679 RVA: 0x0005D262 File Offset: 0x0005B662
		// (set) Token: 0x0600162E RID: 5678 RVA: 0x0005D259 File Offset: 0x0005B659
		public TimeSpan PlayTime { protected get; set; }

		// Token: 0x06001630 RID: 5680 RVA: 0x0005D26C File Offset: 0x0005B66C
		public virtual void Init(ConfigSection section)
		{
			this.Event = section.Get("special_reward");
			if (section.HasValue("silent"))
			{
				section.Get("silent", out this.Silent);
			}
			if (section.HasValue("playtime"))
			{
				int num;
				section.Get("playtime", out num);
				this.PlayTime = TimeSpan.FromMinutes((double)num);
			}
			if (section.HasValue("rank_reached"))
			{
				section.Get("rank_reached", out this.RankReached);
			}
		}

		// Token: 0x06001631 RID: 5681 RVA: 0x0005D2F6 File Offset: 0x0005B6F6
		public virtual ProfileProgressionInfo TrigerRule(ulong profileId, ProfileProgressionInfo info, ILogGroup logGroup)
		{
			return info;
		}

		// Token: 0x06001632 RID: 5682 RVA: 0x0005D2F9 File Offset: 0x0005B6F9
		public virtual ProfileProgressionInfo ProcessRewardData(MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			return aggRewardData.progression;
		}

		// Token: 0x06001633 RID: 5683 RVA: 0x0005D302 File Offset: 0x0005B702
		public virtual void Dispose()
		{
		}

		// Token: 0x06001634 RID: 5684 RVA: 0x0005D304 File Offset: 0x0005B704
		protected IEnumerable<SNotification> ProcessRewardEvent(ulong profileId, ILogGroup logGroup)
		{
			return (!string.IsNullOrEmpty(this.Event)) ? this.m_specialRewards.ProcessEvent(this.Event, profileId, logGroup) : new List<SNotification>();
		}

		// Token: 0x06001635 RID: 5685 RVA: 0x0005D333 File Offset: 0x0005B733
		protected virtual bool Check(ulong profileId)
		{
			return this.Enabled && this.CheckRank(profileId);
		}

		// Token: 0x06001636 RID: 5686 RVA: 0x0005D34A File Offset: 0x0005B74A
		protected virtual bool Check(ulong profileId, long sessionTime)
		{
			return this.Enabled && this.CheckRank(profileId) && this.CheckPlaytime(profileId, sessionTime);
		}

		// Token: 0x06001637 RID: 5687 RVA: 0x0005D370 File Offset: 0x0005B770
		protected bool CheckRank(ulong profileId)
		{
			if (this.RankReached != 0)
			{
				ProfileProxy profileProxy = new ProfileProxy(profileId, this.UserRepository, this.DalService, this.m_catalogService, this.m_profileItemsService, this.m_customRulesService, this.m_customRulesStateStorage);
				return profileProxy.ProfileInfo.RankInfo.RankId >= this.RankReached;
			}
			return true;
		}

		// Token: 0x06001638 RID: 5688 RVA: 0x0005D3D4 File Offset: 0x0005B7D4
		private bool CheckPlaytime(ulong profileId, long sessionTime)
		{
			if (this.PlayTime.TotalSeconds > 0.0)
			{
				string stat;
				IEnumerable<long> source = from measure in this.m_playerStatsService.GetPlayerStatsAggregated(profileId)
				where measure.Dimensions.TryGetValue("stat", out stat) && stat == "player_playtime"
				select measure.Value;
				if (TimeUtils.StatsPlayTimeToTimeSpan(source.Sum()) + TimeSpan.FromSeconds((double)sessionTime) < this.PlayTime)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04000ABF RID: 2751
		protected readonly IProfileProgressionService ProgressionService;

		// Token: 0x04000AC0 RID: 2752
		protected readonly IUserRepository UserRepository;

		// Token: 0x04000AC1 RID: 2753
		protected readonly IDALService DalService;

		// Token: 0x04000AC2 RID: 2754
		private readonly ISpecialProfileRewardService m_specialRewards;

		// Token: 0x04000AC3 RID: 2755
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x04000AC4 RID: 2756
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000AC5 RID: 2757
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x04000AC6 RID: 2758
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x04000AC7 RID: 2759
		private readonly ICustomRulesStateStorage m_customRulesStateStorage;

		// Token: 0x04000AC9 RID: 2761
		protected int RankReached;

		// Token: 0x04000ACA RID: 2762
		protected bool Silent;

		// Token: 0x04000ACB RID: 2763
		protected string Event;
	}
}
