using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x020000FF RID: 255
	internal class InvalidSkillConfigException : SkillServiceException
	{
		// Token: 0x0600042E RID: 1070 RVA: 0x000122D1 File Offset: 0x000106D1
		public InvalidSkillConfigException(string exceptionDetails) : base("Skill configuration in module_configuration.xml is invalid. Details: " + exceptionDetails)
		{
		}

		// Token: 0x0600042F RID: 1071 RVA: 0x000122E4 File Offset: 0x000106E4
		public InvalidSkillConfigException(Exception innerException) : base("Skill configuration in module_configuration.xml is invalid", innerException)
		{
		}
	}
}
