using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.Users
{
	// Token: 0x0200074A RID: 1866
	[Contract]
	internal interface IClientVersionsManagementService
	{
		// Token: 0x0600267F RID: 9855
		void SyncClientVersions();

		// Token: 0x06002680 RID: 9856
		void SetClientVersions(params string[] versionsToSet);

		// Token: 0x06002681 RID: 9857
		void AddClientVersions(params string[] versionsToAdd);

		// Token: 0x06002682 RID: 9858
		void RemoveClientVersions(params string[] versionsToRemove);

		// Token: 0x06002683 RID: 9859
		IEnumerable<string> GetClientVersions();

		// Token: 0x06002684 RID: 9860
		bool Validate(ClientVersion version);

		// Token: 0x140000A5 RID: 165
		// (add) Token: 0x06002685 RID: 9861
		// (remove) Token: 0x06002686 RID: 9862
		event Action ClientVersionsChanged;
	}
}
