using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core.Services;

namespace MasterServer.Users
{
	// Token: 0x02000746 RID: 1862
	[Service]
	[Singleton]
	internal class ClassPresenceService : ServiceModule, IClassPresenceService
	{
		// Token: 0x140000A4 RID: 164
		// (add) Token: 0x0600265E RID: 9822 RVA: 0x000A2B1C File Offset: 0x000A0F1C
		// (remove) Token: 0x0600265F RID: 9823 RVA: 0x000A2B54 File Offset: 0x000A0F54
		public event ClassPresenceDeleg ClassPresenceReceived = delegate(ClassPresenceData A_0)
		{
		};

		// Token: 0x06002660 RID: 9824 RVA: 0x000A2B8C File Offset: 0x000A0F8C
		public void ClassPresenceRecieved(ClassPresenceData data)
		{
			this.ClassPresenceReceived.GetInvocationList().SafeForEach(delegate(Delegate d)
			{
				((ClassPresenceDeleg)d)(data);
			});
		}
	}
}
