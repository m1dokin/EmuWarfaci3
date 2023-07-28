using System;
using Util.Common;

namespace MasterServer.Common
{
	// Token: 0x02000020 RID: 32
	public class scoped_read_lock : IDisposable
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00006856 File Offset: 0x00004C56
		public scoped_read_lock(IReaderWriterLockSlim l)
		{
			this.m_lock = l;
			this.m_lock.EnterReadLock();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00006870 File Offset: 0x00004C70
		public void Dispose()
		{
			this.m_lock.ExitReadLock();
		}

		// Token: 0x04000044 RID: 68
		internal IReaderWriterLockSlim m_lock;
	}
}
