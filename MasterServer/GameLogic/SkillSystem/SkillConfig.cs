using System;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.SkillSystem.Exceptions;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x02000438 RID: 1080
	internal class SkillConfig
	{
		// Token: 0x0600170E RID: 5902 RVA: 0x00060092 File Offset: 0x0005E492
		public SkillConfig(SkillType skillType, ConfigSection channelSkillConfigSection)
		{
			this.ChannelSkillType = skillType;
			this.ParseChannelConfig(channelSkillConfigSection);
		}

		// Token: 0x17000220 RID: 544
		// (get) Token: 0x0600170F RID: 5903 RVA: 0x000600A8 File Offset: 0x0005E4A8
		// (set) Token: 0x06001710 RID: 5904 RVA: 0x000600B0 File Offset: 0x0005E4B0
		public SkillType ChannelSkillType { get; private set; }

		// Token: 0x17000221 RID: 545
		// (get) Token: 0x06001711 RID: 5905 RVA: 0x000600B9 File Offset: 0x0005E4B9
		// (set) Token: 0x06001712 RID: 5906 RVA: 0x000600C1 File Offset: 0x0005E4C1
		public double DefaultChannelValue { get; private set; }

		// Token: 0x17000222 RID: 546
		// (get) Token: 0x06001713 RID: 5907 RVA: 0x000600CA File Offset: 0x0005E4CA
		// (set) Token: 0x06001714 RID: 5908 RVA: 0x000600D2 File Offset: 0x0005E4D2
		public double MaxChannelValue { get; private set; }

		// Token: 0x17000223 RID: 547
		// (get) Token: 0x06001715 RID: 5909 RVA: 0x000600DB File Offset: 0x0005E4DB
		// (set) Token: 0x06001716 RID: 5910 RVA: 0x000600E3 File Offset: 0x0005E4E3
		public double ExponentialCoefficient { get; private set; }

		// Token: 0x17000224 RID: 548
		// (get) Token: 0x06001717 RID: 5911 RVA: 0x000600EC File Offset: 0x0005E4EC
		// (set) Token: 0x06001718 RID: 5912 RVA: 0x000600F4 File Offset: 0x0005E4F4
		public double MinPointsToAdd { get; private set; }

		// Token: 0x17000225 RID: 549
		// (get) Token: 0x06001719 RID: 5913 RVA: 0x000600FD File Offset: 0x0005E4FD
		// (set) Token: 0x0600171A RID: 5914 RVA: 0x00060105 File Offset: 0x0005E505
		public double MaxPointsToAdd { get; private set; }

		// Token: 0x0600171B RID: 5915 RVA: 0x00060110 File Offset: 0x0005E510
		private void ParseChannelConfig(ConfigSection channelSkillConfigSection)
		{
			try
			{
				this.MaxChannelValue = Convert.ToDouble(channelSkillConfigSection.Get("max_skill"));
				this.ExponentialCoefficient = Convert.ToDouble(channelSkillConfigSection.Get("exponential_coefficient"));
				this.DefaultChannelValue = Convert.ToDouble(channelSkillConfigSection.Get("default_skill_value"));
				this.MinPointsToAdd = Convert.ToDouble(channelSkillConfigSection.Get("min_skill_points_add"));
				this.MaxPointsToAdd = Convert.ToDouble(channelSkillConfigSection.Get("max_skill_points_add"));
			}
			catch (Exception innerException)
			{
				throw new InvalidChannelSkillConfigException(this.ChannelSkillType, Resources.ChannelName, innerException);
			}
		}

		// Token: 0x04000B20 RID: 2848
		public const double MaxSkillPoints = 100000.0;

		// Token: 0x04000B21 RID: 2849
		public const string MaxSkillName = "max_skill";

		// Token: 0x04000B22 RID: 2850
		public const string DefaultSkillValueName = "default_skill_value";

		// Token: 0x04000B23 RID: 2851
		public const string ExponentialCoefficientName = "exponential_coefficient";

		// Token: 0x04000B24 RID: 2852
		public const string MinSkillPointsAddName = "min_skill_points_add";

		// Token: 0x04000B25 RID: 2853
		public const string MaxSkillPointsAddName = "max_skill_points_add";
	}
}
