using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SkillSystem.Exceptions;
using MasterServer.GameLogic.SkillSystem.Providers;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x0200043D RID: 1085
	[Service]
	[Singleton]
	internal class SkillService : ServiceModule, ISkillService, IDebugSkillService, IRewardProcessor
	{
		// Token: 0x0600172D RID: 5933 RVA: 0x00060692 File Offset: 0x0005EA92
		public SkillService(IDALService dalService, ISkillConverterService skillConverterService, ISkillCalculator skillCalculator, IUserRepository userRepository, ISkillConfigProvider skillConfigProvider, IEnumerable<ISkillProvider> skillProviders)
		{
			this.m_dalService = dalService;
			this.m_skillConverterService = skillConverterService;
			this.m_skillCalculator = skillCalculator;
			this.m_userRepository = userRepository;
			this.m_skillConfigProvider = skillConfigProvider;
			this.m_skillProviders = skillProviders;
		}

		// Token: 0x1400004C RID: 76
		// (add) Token: 0x0600172E RID: 5934 RVA: 0x000606C8 File Offset: 0x0005EAC8
		// (remove) Token: 0x0600172F RID: 5935 RVA: 0x00060700 File Offset: 0x0005EB00
		public event Action<ulong, Skill> SkillChanged;

		// Token: 0x06001730 RID: 5936 RVA: 0x00060738 File Offset: 0x0005EB38
		public RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			if (state != RewardProcessorState.Process)
			{
				return aggRewardData;
			}
			if (aggRewardData.skillType != SkillType.None && Math.Abs(aggRewardData.skillDiff) > 5E-324)
			{
				this.AddSkillPoints(aggRewardData.profileId, aggRewardData.skillType, aggRewardData.skillDiff, logGroup);
			}
			return aggRewardData;
		}

		// Token: 0x06001731 RID: 5937 RVA: 0x00060794 File Offset: 0x0005EB94
		public override void Start()
		{
			base.Start();
			if (this.m_skillConfigProvider.HasAnyConfigs)
			{
				this.m_userRepository.UserLoggedIn += this.InitOrConvertSkill;
			}
		}

		// Token: 0x06001732 RID: 5938 RVA: 0x000607C3 File Offset: 0x0005EBC3
		public override void Stop()
		{
			this.m_userRepository.UserLoggedIn -= this.InitOrConvertSkill;
			base.Stop();
		}

		// Token: 0x06001733 RID: 5939 RVA: 0x000607E4 File Offset: 0x0005EBE4
		public Skill GetSkill(ulong profileId, SkillType skillType)
		{
			if (this.m_skillConfigProvider.HasConfig(skillType))
			{
				SkillInfo skill = this.m_dalService.SkillSystem.GetSkill(profileId, skillType);
				return (!skill.IsEmptySkill) ? this.ParseSkillInfo(skill) : new Skill(skillType, 0.0, 0.0);
			}
			ISkillProvider skillProvider = this.m_skillProviders.FirstOrDefault((ISkillProvider p) => p.SkillType == skillType);
			if (skillProvider != null)
			{
				return skillProvider.GetSkill(profileId);
			}
			throw new SkillNotSupportedException(skillType);
		}

		// Token: 0x06001734 RID: 5940 RVA: 0x00060894 File Offset: 0x0005EC94
		public double GetMaxChannelSkillByType(SkillType skillType)
		{
			if (this.m_skillConfigProvider.HasConfig(skillType))
			{
				SkillConfig skillConfig = this.m_skillConfigProvider.GetSkillConfig(skillType);
				return skillConfig.MaxChannelValue;
			}
			ISkillProvider skillProvider = this.m_skillProviders.FirstOrDefault((ISkillProvider p) => p.SkillType == skillType);
			if (skillProvider != null)
			{
				return skillProvider.GetMaxSkillValue();
			}
			throw new SkillNotSupportedException(skillType);
		}

		// Token: 0x06001735 RID: 5941 RVA: 0x00060910 File Offset: 0x0005ED10
		public double GetSkillPoints(ulong profileId, SkillType skillType)
		{
			if (!this.m_skillConfigProvider.HasConfig(skillType))
			{
				throw new SkillNotHandledException(skillType);
			}
			return this.m_dalService.SkillSystem.GetSkill(profileId, skillType).Points;
		}

		// Token: 0x06001736 RID: 5942 RVA: 0x0006094F File Offset: 0x0005ED4F
		public void DeleteSkillPoints(ulong profileId, SkillType skillType)
		{
			if (!this.m_skillConfigProvider.HasConfig(skillType))
			{
				throw new SkillNotHandledException(skillType);
			}
			this.m_dalService.SkillSystem.DeleteSkill(profileId, skillType);
		}

		// Token: 0x06001737 RID: 5943 RVA: 0x0006097B File Offset: 0x0005ED7B
		public void AddSkillPoints(ulong profileId, SkillType skillType, double skillToAdd)
		{
			this.AddSkillPoints(profileId, skillType, skillToAdd, null);
		}

		// Token: 0x06001738 RID: 5944 RVA: 0x00060988 File Offset: 0x0005ED88
		public void SetSkillPoints(ulong profileId, SkillType skillType, double skillPoints)
		{
			double skillPoints2 = this.GetSkillPoints(profileId, skillType);
			this.AddSkillPoints(profileId, skillType, skillPoints - skillPoints2, null);
		}

		// Token: 0x06001739 RID: 5945 RVA: 0x000609AC File Offset: 0x0005EDAC
		private void AddSkillPoints(ulong profileId, SkillType skillType, double skillToAdd, ILogGroup logGroup)
		{
			if (!this.m_skillConfigProvider.HasConfig(skillType))
			{
				throw new SkillNotHandledException(skillType);
			}
			SkillInfo skillInfo = this.m_dalService.SkillSystem.AddSkill(profileId, skillType, skillToAdd, -1.0);
			Skill skill = this.ParseSkillInfo(skillInfo);
			if (logGroup != null)
			{
				uint skill2 = (uint)skill.Value;
				uint skill_points = (uint)skillInfo.Points;
				uint curveCoef = (uint)skill.CurveCoef;
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
				ulong userID = profileInfo.UserID;
				SRankInfo rankInfo = profileInfo.RankInfo;
				logGroup.SkillChangedLog(userID, profileId, rankInfo.RankId, skillType, skill2, skill_points, curveCoef);
			}
			this.SkillChanged.SafeInvokeEach(profileId, skill);
		}

		// Token: 0x0600173A RID: 5946 RVA: 0x00060A60 File Offset: 0x0005EE60
		private Skill ParseSkillInfo(SkillInfo skillInfo)
		{
			SkillType type;
			if (!Enum.TryParse<SkillType>(skillInfo.Type, true, out type))
			{
				throw new SkillTypeParseException(skillInfo.Type);
			}
			double value = this.m_skillCalculator.CalculateSkillFromSkillPoints(skillInfo.Points, skillInfo.CurveCoef);
			return new Skill(type, value, skillInfo.CurveCoef);
		}

		// Token: 0x0600173B RID: 5947 RVA: 0x00060AB8 File Offset: 0x0005EEB8
		private void InitOrConvertSkill(UserInfo.User user, ELoginType type)
		{
			List<SkillConfig> list = this.m_skillConfigProvider.GetChannelSkillConfigs().ToList<SkillConfig>();
			foreach (SkillConfig skillConfig in list)
			{
				SkillInfo skill = this.m_dalService.SkillSystem.GetSkill(user.ProfileID, skillConfig.ChannelSkillType);
				if (skill.IsEmptySkill)
				{
					this.InitSkill(user.ProfileID, skillConfig);
				}
				else
				{
					this.ConvertSkill(user.ProfileID, skill, skillConfig);
				}
			}
		}

		// Token: 0x0600173C RID: 5948 RVA: 0x00060B64 File Offset: 0x0005EF64
		private void InitSkill(ulong profileId, SkillConfig config)
		{
			double value = this.m_skillConverterService.ConvertSkillToSkillPoints(config.DefaultChannelValue, config.MaxChannelValue);
			this.m_dalService.SkillSystem.AddSkill(profileId, config.ChannelSkillType, value, config.MaxChannelValue);
		}

		// Token: 0x0600173D RID: 5949 RVA: 0x00060BA8 File Offset: 0x0005EFA8
		private void ConvertSkill(ulong profileId, SkillInfo skillInfo, SkillConfig config)
		{
			double maxChannelValue = config.MaxChannelValue;
			if (Math.Abs(skillInfo.CurveCoef - maxChannelValue) < 5E-324)
			{
				return;
			}
			double num = this.m_skillConverterService.ConvertSkillPointsToNewCurve(skillInfo.Points, skillInfo.CurveCoef, maxChannelValue);
			this.m_dalService.SkillSystem.AddSkill(profileId, config.ChannelSkillType, num - skillInfo.Points, maxChannelValue);
		}

		// Token: 0x04000B2D RID: 2861
		private readonly IDALService m_dalService;

		// Token: 0x04000B2E RID: 2862
		private readonly ISkillConverterService m_skillConverterService;

		// Token: 0x04000B2F RID: 2863
		private readonly ISkillCalculator m_skillCalculator;

		// Token: 0x04000B30 RID: 2864
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000B31 RID: 2865
		private readonly ISkillConfigProvider m_skillConfigProvider;

		// Token: 0x04000B32 RID: 2866
		private readonly IEnumerable<ISkillProvider> m_skillProviders;
	}
}
