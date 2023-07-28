using System;
using MasterServer.Common;
using MasterServer.CryOnlineNET;

namespace MasterServer.Core
{
	// Token: 0x020007AA RID: 1962
	internal static class DebugCommandsJidHelper
	{
		// Token: 0x0600287E RID: 10366 RVA: 0x000AE394 File Offset: 0x000AC794
		public static Jid GetFakeJid(ulong pid)
		{
			return (!Resources.BootstrapMode) ? new Jid("fake_" + pid, "warface", "GameClient") : Utils.MakeJid("fake_" + pid, Resources.BootstrapName, "warface", "GameClient");
		}
	}
}
