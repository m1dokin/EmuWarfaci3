using System;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002BF RID: 703
	public class ConsecutiveLoginBonusUnknownAttributeException : Exception
	{
		// Token: 0x06000F18 RID: 3864 RVA: 0x0003C922 File Offset: 0x0003AD22
		public ConsecutiveLoginBonusUnknownAttributeException(string attrName) : base(string.Format("Unknown attribute '{0}'", attrName))
		{
		}
	}
}
