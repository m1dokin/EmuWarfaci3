using System;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000274 RID: 628
	public struct SClanMemberUpdate
	{
		// Token: 0x06000D8E RID: 3470 RVA: 0x000368BC File Offset: 0x00034CBC
		public SClanMemberUpdate(ClanMember member_info, EMembersListUpdate update_type)
		{
			this.update_type = update_type;
			this.member_info = member_info;
		}

		// Token: 0x04000652 RID: 1618
		public EMembersListUpdate update_type;

		// Token: 0x04000653 RID: 1619
		public ClanMember member_info;
	}
}
