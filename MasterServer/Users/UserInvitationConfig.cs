using System;

namespace MasterServer.Users
{
	// Token: 0x020007FF RID: 2047
	public class UserInvitationConfig
	{
		// Token: 0x060029F9 RID: 10745 RVA: 0x000B588F File Offset: 0x000B3C8F
		public UserInvitationConfig(bool useGroups)
		{
			this.UseGroups = useGroups;
		}

		// Token: 0x170003DE RID: 990
		// (get) Token: 0x060029FA RID: 10746 RVA: 0x000B589E File Offset: 0x000B3C9E
		// (set) Token: 0x060029FB RID: 10747 RVA: 0x000B58A6 File Offset: 0x000B3CA6
		public bool UseGroups { get; private set; }
	}
}
