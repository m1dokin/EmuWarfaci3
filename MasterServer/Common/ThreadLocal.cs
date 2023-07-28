using System;
using System.Collections.Generic;
using System.Threading;

namespace MasterServer.Common
{
	// Token: 0x02000024 RID: 36
	public class ThreadLocal<T>
	{
		// Token: 0x0600007B RID: 123 RVA: 0x0000697E File Offset: 0x00004D7E
		public ThreadLocal()
		{
			this.m_id = Interlocked.Increment(ref ThreadLocal<T>.s_id);
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00006996 File Offset: 0x00004D96
		public static Dictionary<int, T> Data
		{
			get
			{
				if (ThreadLocal<T>._data == null)
				{
					ThreadLocal<T>._data = new Dictionary<int, T>();
				}
				return ThreadLocal<T>._data;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000069B4 File Offset: 0x00004DB4
		// (set) Token: 0x0600007E RID: 126 RVA: 0x000069E3 File Offset: 0x00004DE3
		public T Value
		{
			get
			{
				T result;
				if (!ThreadLocal<T>.Data.TryGetValue(this.m_id, out result))
				{
					return default(T);
				}
				return result;
			}
			set
			{
				ThreadLocal<T>.Data[this.m_id] = value;
			}
		}

		// Token: 0x04000048 RID: 72
		private static int s_id;

		// Token: 0x04000049 RID: 73
		[ThreadStatic]
		private static Dictionary<int, T> _data;

		// Token: 0x0400004A RID: 74
		private int m_id;
	}
}
