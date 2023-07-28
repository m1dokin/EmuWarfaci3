using System;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000273 RID: 627
	public enum EClanCreationStatus
	{
		// Token: 0x0400064A RID: 1610
		Created,
		// Token: 0x0400064B RID: 1611
		NeedBuyItem,
		// Token: 0x0400064C RID: 1612
		InvalidName,
		// Token: 0x0400064D RID: 1613
		CensoredName,
		// Token: 0x0400064E RID: 1614
		DuplicateName,
		// Token: 0x0400064F RID: 1615
		AlreadyClanMember,
		// Token: 0x04000650 RID: 1616
		ServiceError,
		// Token: 0x04000651 RID: 1617
		NameReserved
	}
}
