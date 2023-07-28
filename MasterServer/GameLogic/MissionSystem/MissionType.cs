using System;
using MasterServer.Common;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x0200078B RID: 1931
	internal class MissionType
	{
		// Token: 0x060027F7 RID: 10231 RVA: 0x000ABC84 File Offset: 0x000AA084
		public MissionType(string name = null)
		{
			this.Name = (name ?? string.Empty);
			try
			{
				this.m_missionType = ((!string.IsNullOrEmpty(name)) ? Utils.ParseEnum<ProfileProgressionInfo.MissionType>(name) : ProfileProgressionInfo.MissionType.None);
			}
			catch (ArgumentException innerException)
			{
				throw new MissionParseException(string.Format("Can't parse missiontype '{0}'", name), innerException);
			}
		}

		// Token: 0x060027F8 RID: 10232 RVA: 0x000ABCF0 File Offset: 0x000AA0F0
		public MissionType(ProfileProgressionInfo.MissionType missionType)
		{
			this.Name = missionType.ToString().ToLower();
			this.m_missionType = missionType;
		}

		// Token: 0x170003B5 RID: 949
		// (get) Token: 0x060027F9 RID: 10233 RVA: 0x000ABD17 File Offset: 0x000AA117
		// (set) Token: 0x060027FA RID: 10234 RVA: 0x000ABD1F File Offset: 0x000AA11F
		public string Name { get; private set; }

		// Token: 0x060027FB RID: 10235 RVA: 0x000ABD28 File Offset: 0x000AA128
		public bool IsSurvival()
		{
			return this.IsSurvivalMission() | this.IsCampaign() | this.IsVolcanoCampaign() | this.IsAnubisCampaign() | this.IsZombie() | this.IsZombieTowerCampaign() | this.IsIceBreakerCampaign();
		}

		// Token: 0x060027FC RID: 10236 RVA: 0x000ABD5A File Offset: 0x000AA15A
		public bool IsVolcanoCampaign()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.VolcanoEasy | ProfileProgressionInfo.MissionType.VolcanoNormal | ProfileProgressionInfo.MissionType.VolcanoHard | ProfileProgressionInfo.MissionType.VolcanoSurvival);
		}

		// Token: 0x060027FD RID: 10237 RVA: 0x000ABD76 File Offset: 0x000AA176
		public bool IsAnubisCampaign()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.AnubisEasy | ProfileProgressionInfo.MissionType.AnubisNormal | ProfileProgressionInfo.MissionType.AnubisHard);
		}

		// Token: 0x060027FE RID: 10238 RVA: 0x000ABD92 File Offset: 0x000AA192
		public bool IsZombieTowerCampaign()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.ZombieTowerEasy | ProfileProgressionInfo.MissionType.ZombieTowerNormal | ProfileProgressionInfo.MissionType.ZombieTowerHard);
		}

		// Token: 0x060027FF RID: 10239 RVA: 0x000ABDAE File Offset: 0x000AA1AE
		public bool IsIceBreakerCampaign()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.IceBreakerEasy | ProfileProgressionInfo.MissionType.IceBreakerNormal | ProfileProgressionInfo.MissionType.IceBreakerHard);
		}

		// Token: 0x06002800 RID: 10240 RVA: 0x000ABDCA File Offset: 0x000AA1CA
		public bool IsCampaign()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.CampaignSections | ProfileProgressionInfo.MissionType.CampaignSection1 | ProfileProgressionInfo.MissionType.CampaignSection2 | ProfileProgressionInfo.MissionType.CampaignSection3);
		}

		// Token: 0x06002801 RID: 10241 RVA: 0x000ABDE6 File Offset: 0x000AA1E6
		public bool IsSurvivalMission()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.SurvivalMission);
		}

		// Token: 0x06002802 RID: 10242 RVA: 0x000ABDFF File Offset: 0x000AA1FF
		public bool IsZombie()
		{
			return this.m_missionType.HasAnyFlag(ProfileProgressionInfo.MissionType.ZombieEasy | ProfileProgressionInfo.MissionType.ZombieNormal | ProfileProgressionInfo.MissionType.ZombieHard);
		}

		// Token: 0x06002803 RID: 10243 RVA: 0x000ABE1C File Offset: 0x000AA21C
		public bool IsTraining()
		{
			return this.m_missionType.HasFlag(ProfileProgressionInfo.MissionType.TrainingMission);
		}

		// Token: 0x06002804 RID: 10244 RVA: 0x000ABE43 File Offset: 0x000AA243
		public override string ToString()
		{
			return this.Name;
		}

		// Token: 0x040014E2 RID: 5346
		private readonly ProfileProgressionInfo.MissionType m_missionType;
	}
}
