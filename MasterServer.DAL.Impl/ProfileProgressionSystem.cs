using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200001D RID: 29
	internal class ProfileProgressionSystem : IProfileProgressionSystem
	{
		// Token: 0x06000141 RID: 321 RVA: 0x0000CBA8 File Offset: 0x0000ADA8
		public ProfileProgressionSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000142 RID: 322 RVA: 0x0000CBC4 File Offset: 0x0000ADC4
		public DALResult<SProfileProgression> GetProfileProgression(ulong profileId)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL GetProfileProgression(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x0000CC18 File Offset: 0x0000AE18
		public DALResult<SProfileProgression> SetProfileProgression(SProfileProgression progress)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL SetProfileProgression(?pid, ?m_unlc, ?m_pass, ?zm_unlc, ?c_unlc, ?vc_unlc, ?ab_pass, ?zt_pass, ?ib_pass, ?tut_unlc, ?tut_psd, ?cl_unlc)", new object[]
			{
				"?pid",
				progress.ProfileId,
				"?m_unlc",
				progress.MissionUnlocked,
				"?m_pass",
				progress.MissionPassCounter,
				"?zm_unlc",
				progress.ZombieMissionPassCounter,
				"?c_unlc",
				progress.CampaignPassCounter,
				"?vc_unlc",
				progress.VolcanoCampaignPasCounter,
				"?ab_pass",
				progress.AnubisCampaignPassCounter,
				"?zt_pass",
				progress.ZombieTowerCampaignPassCounter,
				"?ib_pass",
				progress.IceBreakerCampaignPassCounter,
				"?tut_unlc",
				progress.TutorialUnlocked,
				"?tut_psd",
				progress.TutorialPassed,
				"?cl_unlc",
				progress.ClassUnlocked
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000144 RID: 324 RVA: 0x0000CD80 File Offset: 0x0000AF80
		public DALResult<SProfileProgression> IncrementMissionPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateMissionPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0000CDF8 File Offset: 0x0000AFF8
		public DALResult<SProfileProgression> IncrementZombieMissionPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateZombieMissionPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000CE70 File Offset: 0x0000B070
		public DALResult<SProfileProgression> IncrementCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateCampaignPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000147 RID: 327 RVA: 0x0000CEE8 File Offset: 0x0000B0E8
		public DALResult<SProfileProgression> IncrementVolcanoCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateVolcanoCampaignPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000148 RID: 328 RVA: 0x0000CF60 File Offset: 0x0000B160
		public DALResult<SProfileProgression> IncrementAnubisCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateAnubisCampaignPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000CFD8 File Offset: 0x0000B1D8
		public DALResult<SProfileProgression> IncrementZombieTowerCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateZombieTowerCampaignPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000D050 File Offset: 0x0000B250
		public DALResult<SProfileProgression> IncrementIceBreakerCampaignPassCounter(ulong profileId, int value, int maxValue)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UpdateIceBreakerCampaignPassCounter(?pid, ?val, ?max_val)", new object[]
			{
				"?pid",
				profileId,
				"?val",
				value,
				"?max_val",
				maxValue
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000D0C8 File Offset: 0x0000B2C8
		public DALResult<SProfileProgression> UnlockTutorial(ulong profileId, int tutorialUnlocked)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UnlockTutorial(?pid, ?tut_unlc)", new object[]
			{
				"?pid",
				profileId,
				"?tut_unlc",
				tutorialUnlocked
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x0600014C RID: 332 RVA: 0x0000D130 File Offset: 0x0000B330
		public DALResult<SProfileProgression> PassTutorial(ulong profileId, int tutorialPassed)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL PassTutorial(?pid, ?tut_psd)", new object[]
			{
				"?pid",
				profileId,
				"?tut_psd",
				tutorialPassed
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000D198 File Offset: 0x0000B398
		public DALResult<SProfileProgression> UnlockClass(ulong profileId, int classUnlocked)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UnlockClass(?pid, ?cl_unlc)", new object[]
			{
				"?pid",
				profileId,
				"?cl_unlc",
				classUnlocked
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000D200 File Offset: 0x0000B400
		public DALResult<SProfileProgression> UnlockMission(ulong profileId, int unlockedMissionType)
		{
			CacheProxy.Options<SProfileProgression> options = new CacheProxy.Options<SProfileProgression>
			{
				db_serializer = this.m_profileProgressionSerializer
			};
			options.query("CALL UnlockMission(?pid, ?unlc)", new object[]
			{
				"?pid",
				profileId,
				"?unlc",
				unlockedMissionType
			});
			return this.m_dal.CacheProxy.Get<SProfileProgression>(options);
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000D268 File Offset: 0x0000B468
		public DALResultVoid DeleteProgression(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteProgression(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000063 RID: 99
		private readonly DAL m_dal;

		// Token: 0x04000064 RID: 100
		private readonly ProfileProgressionSerializer m_profileProgressionSerializer = new ProfileProgressionSerializer();
	}
}
