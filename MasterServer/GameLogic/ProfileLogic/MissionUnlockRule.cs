using System;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.CustomRules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SpecialProfileRewards;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000400 RID: 1024
	[ProfileProgressionRule(Name = "mission_unlock")]
	internal class MissionUnlockRule : ProfileProgressionRuleBase
	{
		// Token: 0x06001624 RID: 5668 RVA: 0x0005D8DC File Offset: 0x0005BCDC
		public MissionUnlockRule(IProfileProgressionService progressionService, ISpecialProfileRewardService specialRewards, IPlayerStatsService playerStatsService, IUserRepository userRepository, IDALService dalService, ICatalogService catalogService, IProfileItems profileItemsService, ICustomRulesService customRulesService, ICustomRulesStateStorage customRulesStateStorage) : base(progressionService, userRepository, specialRewards, playerStatsService, dalService, catalogService, profileItemsService, customRulesService, customRulesStateStorage)
		{
		}

		// Token: 0x06001625 RID: 5669 RVA: 0x0005D900 File Offset: 0x0005BD00
		public override void Init(ConfigSection section)
		{
			base.Init(section);
			string value = section.Get("type");
			string text = section.Get("pass_value");
			this.m_missionType = ((!string.IsNullOrEmpty(value)) ? Utils.ParseEnum<ProfileProgressionInfo.MissionType>(value) : ProfileProgressionInfo.MissionType.None);
			this.m_maxCounterValue = ((this.m_missionType != ProfileProgressionInfo.MissionType.None) ? int.Parse(section.Get("max_value")) : 0);
			this.m_passCounterValue = ((!string.IsNullOrEmpty(text)) ? int.Parse(text) : 0);
			string value2 = section.Get("unlock_type");
			this.m_unlockMissionType = ((!string.IsNullOrEmpty(value2)) ? Utils.ParseEnum<ProfileProgressionInfo.MissionType>(value2) : ProfileProgressionInfo.MissionType.None);
			this.m_branch = ProfileProgressionService.GetMissionUnlockBranch(this.m_missionType);
			this.ValidateProgression(this.m_missionType, this.m_unlockMissionType);
		}

		// Token: 0x06001626 RID: 5670 RVA: 0x0005D9DC File Offset: 0x0005BDDC
		public override ProfileProgressionInfo ProcessRewardData(MissionContext missionContext, RewardOutputData output, ILogGroup logGroup)
		{
			ProfileProgressionInfo profileProgressionInfo = new ProfileProgressionInfo(output.profileId, null);
			if (output.outcome != SessionOutcome.Won)
			{
				return profileProgressionInfo;
			}
			if (!this.m_missionType.ToString().Equals(missionContext.missionType.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				return profileProgressionInfo;
			}
			if (this.UserRepository.GetUser(output.profileId) == null)
			{
				return profileProgressionInfo;
			}
			int missionPassCounter = this.GetMissionPassCounter(profileProgressionInfo);
			if (missionPassCounter < this.m_maxCounterValue)
			{
				profileProgressionInfo = this.ProgressionService.IncrementMissionPassCounter(output.profileId, this.m_passCounterValue, this.m_maxCounterValue, this.m_branch);
			}
			return profileProgressionInfo;
		}

		// Token: 0x06001627 RID: 5671 RVA: 0x0005DA84 File Offset: 0x0005BE84
		public override ProfileProgressionInfo TrigerRule(ulong profileId, ProfileProgressionInfo progressionInfo, ILogGroup logGroup)
		{
			if (!progressionInfo.IsMissionTypeUnlocked(this.m_unlockMissionType) && this.Check(progressionInfo.ProfileId) && this.m_maxCounterValue <= this.GetMissionPassCounter(progressionInfo))
			{
				progressionInfo |= this.ProgressionService.UnlockMission(progressionInfo, this.m_unlockMissionType, this.Silent, logGroup);
				base.ProcessRewardEvent(profileId, logGroup);
			}
			return progressionInfo;
		}

		// Token: 0x06001628 RID: 5672 RVA: 0x0005DAF0 File Offset: 0x0005BEF0
		private int GetMissionPassCounter(ProfileProgressionInfo info)
		{
			return ProfileProgressionService.GetMissionPassCounter(info, this.m_branch);
		}

		// Token: 0x06001629 RID: 5673 RVA: 0x0005DB00 File Offset: 0x0005BF00
		private void ValidateProgression(ProfileProgressionInfo.MissionType type, ProfileProgressionInfo.MissionType unlockType)
		{
			if (type == ProfileProgressionInfo.MissionType.None || unlockType == ProfileProgressionInfo.MissionType.None)
			{
				return;
			}
			MissionUnlockBranch missionUnlockBranch = ProfileProgressionService.GetMissionUnlockBranch(type);
			if (ProfileProgressionService.GetMissionUnlockBranch(unlockType) != missionUnlockBranch)
			{
				throw new ProfileProgressionException(string.Format("Incompatible mission unlock type {0} for branch {1}", unlockType, missionUnlockBranch));
			}
		}

		// Token: 0x0600162A RID: 5674 RVA: 0x0005DB49 File Offset: 0x0005BF49
		public override string ToString()
		{
			return string.Format("<mission_unlock type={0} requiredValue={1} maxValue={2}/>", string.Empty, string.Empty, string.Empty);
		}

		// Token: 0x04000AB9 RID: 2745
		private ProfileProgressionInfo.MissionType m_missionType;

		// Token: 0x04000ABA RID: 2746
		private ProfileProgressionInfo.MissionType m_unlockMissionType;

		// Token: 0x04000ABB RID: 2747
		private MissionUnlockBranch m_branch;

		// Token: 0x04000ABC RID: 2748
		private int m_maxCounterValue;

		// Token: 0x04000ABD RID: 2749
		private int m_passCounterValue;
	}
}
