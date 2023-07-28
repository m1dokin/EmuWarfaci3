using System;
using System.Reflection;
using MasterServer.DAL;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x0200020C RID: 524
	internal class ProfileProgressionSystemClient : DALCacheProxy<IDALService>, IProfileProgressionSystemClient
	{
		// Token: 0x06000B24 RID: 2852 RVA: 0x00029B33 File Offset: 0x00027F33
		internal void Reset(IProfileProgressionSystem profileProgressionSystem)
		{
			this.m_profileProgressionSystem = profileProgressionSystem;
		}

		// Token: 0x06000B25 RID: 2853 RVA: 0x00029B3C File Offset: 0x00027F3C
		public SProfileProgression GetProfileProgression(ulong profileId)
		{
			DALCacheProxy<IDALService>.Options<SProfileProgression> options = new DALCacheProxy<IDALService>.Options<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				get_data = (() => this.m_profileProgressionSystem.GetProfileProgression(profileId))
			};
			return base.GetData<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x00029BA4 File Offset: 0x00027FA4
		public SProfileProgression SetProfileProgression(ulong profileId, ProfileProgressionInfo profileProgress)
		{
			SProfileProgression progress = new SProfileProgression
			{
				ProfileId = profileProgress.ProfileId,
				ClassUnlocked = (int)profileProgress.ClassUnlocked,
				MissionUnlocked = (int)profileProgress.MissionUnlocked,
				MissionPassCounter = profileProgress.MissionPassCounter,
				ZombieMissionPassCounter = profileProgress.ZombieMissionPassCounter,
				VolcanoCampaignPasCounter = profileProgress.VolcanoCampaignPassCounter,
				CampaignPassCounter = profileProgress.CampaignPassCounter,
				TutorialPassed = (int)profileProgress.TutorialPassed,
				TutorialUnlocked = (int)profileProgress.TutorialUnlocked
			};
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.SetProfileProgression(progress))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x00029C84 File Offset: 0x00028084
		public SProfileProgression IncrementMissionPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementMissionPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00029CF8 File Offset: 0x000280F8
		public SProfileProgression IncrementZombieMissionPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementZombieMissionPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x00029D6C File Offset: 0x0002816C
		public SProfileProgression IncrementCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementCampaignPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x00029DE0 File Offset: 0x000281E0
		public SProfileProgression IncrementVolcanoCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementVolcanoCampaignPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x00029E54 File Offset: 0x00028254
		public SProfileProgression IncrementAnubisCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementAnubisCampaignPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x00029EC8 File Offset: 0x000282C8
		public SProfileProgression IncrementZombieTowerCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementZombieTowerCampaignPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x00029F3C File Offset: 0x0002833C
		public SProfileProgression IncrementIceBreakerCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.IncrementIceBreakerCampaignPassCounter(profileId, value, maxValue))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x00029FB0 File Offset: 0x000283B0
		public SProfileProgression UnlockTutorial(ulong profileId, ProfileProgressionInfo.Tutorial tutorialUnlocked)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.UnlockTutorial(profileId, (int)tutorialUnlocked))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0002A01C File Offset: 0x0002841C
		public SProfileProgression PassTutorial(ulong profileId, ProfileProgressionInfo.Tutorial tutorialPassed)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.PassTutorial(profileId, (int)tutorialPassed))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0002A088 File Offset: 0x00028488
		public SProfileProgression UnlockClass(ulong profileId, ProfileProgressionInfo.PlayerClass classUnlocked)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.UnlockClass(profileId, (int)classUnlocked))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0002A0F4 File Offset: 0x000284F4
		public SProfileProgression UnlockMission(ulong profileId, ProfileProgressionInfo.MissionType unlockedMissionType)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression> options = new DALCacheProxy<IDALService>.SetOptionsScalar<SProfileProgression>
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.UnlockMission(profileId, (int)unlockedMissionType))
			};
			return base.SetAndStore<SProfileProgression>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0002A160 File Offset: 0x00028560
		public void DeleteProgression(ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].profile_progression,
				set_func = (() => this.m_profileProgressionSystem.DeleteProgression(profileId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400055D RID: 1373
		private IProfileProgressionSystem m_profileProgressionSystem;
	}
}
