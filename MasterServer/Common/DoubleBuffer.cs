using System;
using System.Collections.Generic;
using System.Threading;
using MasterServer.Core;
using Util.Common;

namespace MasterServer.Common
{
	// Token: 0x02000017 RID: 23
	internal class DoubleBuffer<KT, VT> where VT : ICloneable
	{
		// Token: 0x06000057 RID: 87 RVA: 0x00005ACE File Offset: 0x00003ECE
		public DoubleBuffer(Dictionary<KT, VT> front)
		{
			this.m_front = (front ?? new Dictionary<KT, VT>());
			this.m_back = new Dictionary<KT, VT>();
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000058 RID: 88 RVA: 0x00005B0C File Offset: 0x00003F0C
		public AccessMode mode
		{
			get
			{
				if (Resources.UseMonoRWLock)
				{
					if (this.m_rw_lock.IsUpgradeableReadLockHeld || this.m_rw_lock.IsWriteLockHeld)
					{
						return AccessMode.ReadWrite;
					}
					if (this.m_rw_lock.IsReadLockHeld)
					{
						return AccessMode.ReadOnly;
					}
					return AccessMode.None;
				}
				else
				{
					if (this.m_rw_lock_custom.IsUpgradeableReadLockHeld || this.m_rw_lock_custom.IsWriteLockHeld)
					{
						return AccessMode.ReadWrite;
					}
					if (this.m_rw_lock_custom.IsReadLockHeld)
					{
						return AccessMode.ReadOnly;
					}
					return AccessMode.None;
				}
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00005B90 File Offset: 0x00003F90
		public void enter(AccessMode acc_mode)
		{
			if (acc_mode == AccessMode.None)
			{
				throw new InvalidOperationException("Invalid mode");
			}
			if (this.mode == AccessMode.ReadOnly && acc_mode == AccessMode.ReadWrite)
			{
				throw new Exception("Upgrading the state access level is not supported");
			}
			if (Resources.UseMonoRWLock)
			{
				if (acc_mode == AccessMode.ReadOnly)
				{
					this.m_rw_lock.EnterReadLock();
				}
				else if (acc_mode == AccessMode.ReadWrite)
				{
					this.m_rw_lock.EnterUpgradeableReadLock();
				}
			}
			else if (acc_mode == AccessMode.ReadOnly)
			{
				this.m_rw_lock_custom.EnterReadLock();
			}
			else if (acc_mode == AccessMode.ReadWrite)
			{
				this.m_rw_lock_custom.EnterUpgradeableReadLock();
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00005C30 File Offset: 0x00004030
		public DoubleBuffer<KT, VT>.Snapshot commit()
		{
			if (this.mode != AccessMode.ReadWrite)
			{
				throw new InvalidOperationException("commit valid only in R/W mode");
			}
			if (this.m_back.Count == 0)
			{
				return null;
			}
			if (Resources.UseMonoRWLock)
			{
				this.m_rw_lock.EnterWriteLock();
			}
			else
			{
				this.m_rw_lock_custom.EnterWriteLock();
			}
			DoubleBuffer<KT, VT>.Snapshot result;
			try
			{
				DoubleBuffer<KT, VT>.Snapshot snapshot = new DoubleBuffer<KT, VT>.Snapshot();
				foreach (KeyValuePair<KT, VT> keyValuePair in this.m_front)
				{
					VT new_value;
					this.m_back.TryGetValue(keyValuePair.Key, out new_value);
					snapshot.Add(new DoubleBuffer<KT, VT>.Flip
					{
						key = keyValuePair.Key,
						old_value = keyValuePair.Value,
						new_value = new_value
					});
				}
				foreach (KeyValuePair<KT, VT> keyValuePair2 in this.m_back)
				{
					this.m_front[keyValuePair2.Key] = keyValuePair2.Value;
				}
				this.m_back.Clear();
				result = snapshot;
			}
			finally
			{
				if (Resources.UseMonoRWLock)
				{
					this.m_rw_lock.ExitWriteLock();
				}
				else
				{
					this.m_rw_lock_custom.ExitWriteLock();
				}
			}
			return result;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005DC8 File Offset: 0x000041C8
		public void discard()
		{
			if (this.mode != AccessMode.ReadWrite)
			{
				throw new InvalidOperationException("discard valid only in R/W mode");
			}
			this.m_back.Clear();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00005DEC File Offset: 0x000041EC
		public void exit()
		{
			if (this.mode == AccessMode.None)
			{
				throw new InvalidOperationException("exiting when lock is not held");
			}
			if (Resources.UseMonoRWLock)
			{
				if (this.m_rw_lock.RecursiveReadCount > 0)
				{
					this.m_rw_lock.ExitReadLock();
				}
				else
				{
					this.m_rw_lock.ExitUpgradeableReadLock();
				}
			}
			else if (this.m_rw_lock_custom.RecursiveReadCount > 0)
			{
				this.m_rw_lock_custom.ExitReadLock();
			}
			else
			{
				this.m_rw_lock_custom.ExitUpgradeableReadLock();
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005E76 File Offset: 0x00004276
		public bool check_access(AccessMode check_mode)
		{
			return check_mode <= this.mode;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00005E84 File Offset: 0x00004284
		public VT get(KT key, AccessMode acc_mode)
		{
			if (!this.check_access(acc_mode))
			{
				throw new Exception("Invalid access mode");
			}
			if (this.mode == AccessMode.ReadOnly)
			{
				return this.m_front[key];
			}
			VT vt;
			if (!this.m_back.TryGetValue(key, out vt))
			{
				vt = this.m_front[key];
				if (this.mode == AccessMode.ReadWrite)
				{
					vt = (VT)((object)vt.Clone());
					this.m_back.Add(key, vt);
				}
			}
			return vt;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600005F RID: 95 RVA: 0x00005F10 File Offset: 0x00004310
		public IEnumerable<KeyValuePair<KT, VT>> items
		{
			get
			{
				if (!this.check_access(AccessMode.ReadOnly))
				{
					throw new Exception("Invalid access mode");
				}
				foreach (KT i in this.m_front.Keys)
				{
					VT val;
					if (this.mode != AccessMode.ReadWrite || !this.m_back.TryGetValue(i, out val))
					{
						val = this.m_front[i];
					}
					yield return new KeyValuePair<KT, VT>(i, val);
				}
				yield break;
			}
		}

		// Token: 0x04000036 RID: 54
		private readonly Dictionary<KT, VT> m_front;

		// Token: 0x04000037 RID: 55
		private readonly Dictionary<KT, VT> m_back;

		// Token: 0x04000038 RID: 56
		private readonly System.Threading.ReaderWriterLockSlim m_rw_lock = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);

		// Token: 0x04000039 RID: 57
		private readonly Util.Common.ReaderWriterLockSlim m_rw_lock_custom = new Util.Common.ReaderWriterLockSlim(Util.Common.LockRecursionPolicy.SupportsRecursion);

		// Token: 0x02000018 RID: 24
		public class Snapshot : List<DoubleBuffer<KT, VT>.Flip>
		{
		}

		// Token: 0x02000019 RID: 25
		public struct Flip
		{
			// Token: 0x0400003A RID: 58
			public KT key;

			// Token: 0x0400003B RID: 59
			public VT old_value;

			// Token: 0x0400003C RID: 60
			public VT new_value;
		}
	}
}
