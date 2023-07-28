using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x020000FE RID: 254
	internal class InvalidChannelSkillConfigException : SkillServiceException
	{
		// Token: 0x0600042B RID: 1067 RVA: 0x00012295 File Offset: 0x00010695
		public InvalidChannelSkillConfigException(SkillType skillType, string channelName, Exception innerException) : base(string.Format("Error during read {0} channel skill config for {1} skill", channelName, skillType), innerException)
		{
		}

		// Token: 0x0600042C RID: 1068 RVA: 0x000122AF File Offset: 0x000106AF
		public InvalidChannelSkillConfigException(string variableName, double variableValue) : base(string.Format("Config variable {0} should be greater than zero. Actual: {1}", variableName, variableValue))
		{
		}

		// Token: 0x0600042D RID: 1069 RVA: 0x000122C8 File Offset: 0x000106C8
		public InvalidChannelSkillConfigException(string message) : base(message)
		{
		}
	}
}
