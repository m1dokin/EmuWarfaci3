using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000022 RID: 34
	internal class SkillSystem : MarshalByRefObject, ISkillSystem
	{
		// Token: 0x06000179 RID: 377 RVA: 0x0000E4A0 File Offset: 0x0000C6A0
		public SkillSystem(DAL dal)
		{
			this.m_dal = dal;
			this.m_skillInfoSerializer = new SkillInfoSerializer();
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000E4BA File Offset: 0x0000C6BA
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000E4C0 File Offset: 0x0000C6C0
		public DALResult<SkillInfo> GetSkill(ulong profileId, string skillType)
		{
			CacheProxy.Options<SkillInfo> options = new CacheProxy.Options<SkillInfo>
			{
				db_serializer = this.m_skillInfoSerializer
			};
			options.query("CALL GetSkill(?pid, ?skill_type)", new object[]
			{
				"?pid",
				profileId,
				"?skill_type",
				skillType
			});
			return this.m_dal.CacheProxy.Get<SkillInfo>(options);
		}

		// Token: 0x0600017C RID: 380 RVA: 0x0000E520 File Offset: 0x0000C720
		public DALResultVoid DeleteSkill(ulong profileId, string skillType)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteSkill(?pid, ?skill_type)", new object[]
			{
				"?pid",
				profileId,
				"?skill_type",
				skillType
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600017D RID: 381 RVA: 0x0000E574 File Offset: 0x0000C774
		public DALResult<SkillInfo> AddSkill(ulong profileId, string skillType, double value, double curveCoef)
		{
			CacheProxy.Options<SkillInfo> options = new CacheProxy.Options<SkillInfo>
			{
				db_serializer = this.m_skillInfoSerializer,
				db_transaction = true
			};
			options.query("CALL AddSkill(?pid, ?skill_type, ?skill_change, ?current_curve_coef)", new object[]
			{
				"?pid",
				profileId,
				"?skill_type",
				skillType,
				"?skill_change",
				value,
				"?current_curve_coef",
				curveCoef
			});
			return this.m_dal.CacheProxy.Get<SkillInfo>(options);
		}

		// Token: 0x04000074 RID: 116
		private readonly DAL m_dal;

		// Token: 0x04000075 RID: 117
		private readonly SkillInfoSerializer m_skillInfoSerializer;
	}
}
