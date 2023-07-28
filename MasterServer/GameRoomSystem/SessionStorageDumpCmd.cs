using System;
using MasterServer.Core;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000622 RID: 1570
	[ConsoleCmdAttributes(CmdName = "session_storage_dump", ArgsSize = 0)]
	internal class SessionStorageDumpCmd : IConsoleCmd
	{
		// Token: 0x060021BC RID: 8636 RVA: 0x0008AB58 File Offset: 0x00088F58
		public SessionStorageDumpCmd(ISessionStorageDebug sessionStorage)
		{
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x060021BD RID: 8637 RVA: 0x0008AB67 File Offset: 0x00088F67
		public void ExecuteCmd(string[] args)
		{
			this.m_sessionStorage.DbgDumpStorage();
		}

		// Token: 0x04001062 RID: 4194
		private readonly ISessionStorageDebug m_sessionStorage;
	}
}
