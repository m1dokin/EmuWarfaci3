using System;
using MasterServer.GameLogic.SkillSystem.Exceptions;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x02000100 RID: 256
	internal class SkillConfigValidator
	{
		// Token: 0x06000431 RID: 1073 RVA: 0x000122FC File Offset: 0x000106FC
		public void Validate(SkillConfig skillConfig)
		{
			this.ShouldBeGreaterZeroValidation("max_skill", skillConfig.MaxChannelValue);
			this.ShouldBeGreaterZeroValidation("exponential_coefficient", skillConfig.ExponentialCoefficient);
			this.ShouldBeGreaterZeroValidation("min_skill_points_add", skillConfig.MinPointsToAdd);
			if (skillConfig.MinPointsToAdd > skillConfig.MaxPointsToAdd)
			{
				throw new InvalidChannelSkillConfigException(string.Format("{0} can't be greater than {1}. {2} > {3}", new object[]
				{
					"min_skill_points_add",
					"max_skill_points_add",
					skillConfig.MinPointsToAdd,
					skillConfig.MaxPointsToAdd
				}));
			}
		}

		// Token: 0x06000432 RID: 1074 RVA: 0x0001238F File Offset: 0x0001078F
		private void ShouldBeGreaterZeroValidation(string variableName, double actualValue)
		{
			if (actualValue <= 5E-324)
			{
				throw new InvalidChannelSkillConfigException(variableName, actualValue);
			}
		}
	}
}
