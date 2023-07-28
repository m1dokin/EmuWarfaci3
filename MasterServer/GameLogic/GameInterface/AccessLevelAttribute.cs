using System;
using System.Reflection;
using MasterServer.Common;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E0 RID: 736
	public class AccessLevelAttribute : Attribute, Aspect
	{
		// Token: 0x06001029 RID: 4137 RVA: 0x0003F3CC File Offset: 0x0003D7CC
		public AccessLevelAttribute(AccessLevel level)
		{
			this.Level = level;
		}

		// Token: 0x0600102A RID: 4138 RVA: 0x0003F3DC File Offset: 0x0003D7DC
		public void PreCall(object thiz, MethodInfo method)
		{
			GameInterfaceContext gameInterfaceContext = (GameInterfaceContext)thiz;
			if (!gameInterfaceContext.AccessLvl.HasAnyFlag(this.Level))
			{
				throw new AccessLevelException();
			}
		}

		// Token: 0x0400076C RID: 1900
		public readonly AccessLevel Level;
	}
}
