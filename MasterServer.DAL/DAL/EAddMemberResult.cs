using System;

namespace MasterServer.DAL
{
	// Token: 0x02000013 RID: 19
	public enum EAddMemberResult
	{
		// Token: 0x0400002B RID: 43
		Succeed,
		// Token: 0x0400002C RID: 44
		Duplicate,
		// Token: 0x0400002D RID: 45
		LimitReached,
		// Token: 0x0400002E RID: 46
		ProfileDoesNotExist
	}
}
