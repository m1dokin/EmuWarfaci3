using System;
using Util.Common;

namespace MasterServer.Common
{
	// Token: 0x02000021 RID: 33
	public class scoped_write_lock : IDisposable
	{
		// Token: 0x06000072 RID: 114 RVA: 0x0000687D File Offset: 0x00004C7D
		public scoped_write_lock(IReaderWriterLockSlim l)
		{
			this.m_lock = l;
			this.m_lock.EnterWriteLock();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006897 File Offset: 0x00004C97
		public void Dispose()
		{
			this.m_lock.ExitWriteLock();
		}

		// Token: 0x04000045 RID: 69
		internal IReaderWriterLockSlim m_lock;
	}
}
