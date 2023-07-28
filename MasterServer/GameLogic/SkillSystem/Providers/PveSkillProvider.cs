using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.SkillSystem.Providers
{
	// Token: 0x02000432 RID: 1074
	[Service]
	[Singleton]
	internal class PveSkillProvider : ISkillProvider
	{
		// Token: 0x060016F6 RID: 5878 RVA: 0x0005FD49 File Offset: 0x0005E149
		public PveSkillProvider(IDALService dalService, IRankSystem rankSystem)
		{
			this.m_dalService = dalService;
			this.m_rankSystem = rankSystem;
			this.SkillType = SkillType.Pve;
		}

		// Token: 0x1700021B RID: 539
		// (get) Token: 0x060016F7 RID: 5879 RVA: 0x0005FD66 File Offset: 0x0005E166
		// (set) Token: 0x060016F8 RID: 5880 RVA: 0x0005FD6E File Offset: 0x0005E16E
		public SkillType SkillType { get; private set; }

		// Token: 0x060016F9 RID: 5881 RVA: 0x0005FD78 File Offset: 0x0005E178
		public Skill GetSkill(ulong profileId)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			return new Skill(this.SkillType, (double)profileInfo.RankInfo.RankId, 0.0);
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x0005FDB8 File Offset: 0x0005E1B8
		public double GetMaxSkillValue()
		{
			return this.m_rankSystem.ChannelMaxRank;
		}

		// Token: 0x04000B13 RID: 2835
		private readonly IDALService m_dalService;

		// Token: 0x04000B14 RID: 2836
		private readonly IRankSystem m_rankSystem;
	}
}
