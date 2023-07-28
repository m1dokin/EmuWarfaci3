using System;
using Util.Common;

namespace MasterServer.Common
{
	// Token: 0x02000022 RID: 34
	public class scoped_upgradable_lock : IDisposable
	{
		// Token: 0x06000074 RID: 116 RVA: 0x000068A4 File Offset: 0x00004CA4
		public scoped_upgradable_lock(scoped_write_lock l) : this(l.m_lock)
		{
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000068B2 File Offset: 0x00004CB2
		public scoped_upgradable_lock(IReaderWriterLockSlim l)
		{
			this.m_lock = l;
			this.m_lock.EnterUpgradeableReadLock();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x000068CC File Offset: 0x00004CCC
		public void Dispose()
		{
			this.m_lock.ExitUpgradeableReadLock();
		}

		// Token: 0x04000046 RID: 70
		internal IReaderWriterLockSlim m_lock;
	}
}
