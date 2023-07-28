using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x020000FA RID: 250
	internal class SkillTypeParseException : SkillServiceException
	{
		// Token: 0x06000416 RID: 1046 RVA: 0x00011CCA File Offset: 0x000100CA
		public SkillTypeParseException(string skillType) : base(string.Format("Unable to parse skill type {0}", skillType))
		{
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00011CDD File Offset: 0x000100DD
		public SkillTypeParseException(string skillType, Exception innerException) : base(string.Format("Unable to parse skill type {0}", skillType), innerException)
		{
		}
	}
}
