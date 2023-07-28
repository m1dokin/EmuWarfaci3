using System;
using System.Collections.Generic;

namespace MasterServer.Common
{
	// Token: 0x0200001B RID: 27
	internal class Range<T> where T : IComparable<T>
	{
		// Token: 0x06000064 RID: 100 RVA: 0x000062B5 File Offset: 0x000046B5
		public Range(IEnumerable<T> stops)
		{
			this.m_stops = new List<T>(stops);
			if (this.m_stops.Count < 2)
			{
				throw new Exception("Range should have at least two interval stops");
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000065 RID: 101 RVA: 0x000062E5 File Offset: 0x000046E5
		public T Min
		{
			get
			{
				return this.m_stops[0];
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000066 RID: 102 RVA: 0x000062F3 File Offset: 0x000046F3
		public T Max
		{
			get
			{
				return this.m_stops[this.m_stops.Count - 1];
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00006310 File Offset: 0x00004710
		public int IntervalIndex(T val)
		{
			for (int num = 0; num != this.m_stops.Count - 1; num++)
			{
				T t = this.m_stops[num];
				int num2 = t.CompareTo(val);
				T t2 = this.m_stops[num + 1];
				int num3 = t2.CompareTo(val);
				if (num2 == 0 || num3 == 0 || (num2 < 0 && num3 > 0))
				{
					return num;
				}
			}
			return -1;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00006394 File Offset: 0x00004794
		public KeyValuePair<T, T> Interval(T val)
		{
			int num = this.IntervalIndex(val);
			if (num < 0)
			{
				throw new ArgumentException("Value is outside of range intervals");
			}
			return new KeyValuePair<T, T>(this.m_stops[num], this.m_stops[num + 1]);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x000063DC File Offset: 0x000047DC
		public KeyValuePair<T, T> Interval(int index)
		{
			if (index < 0 || index >= this.m_stops.Count - 1)
			{
				throw new ArgumentException("Invalid range interval");
			}
			return new KeyValuePair<T, T>(this.m_stops[index], this.m_stops[index + 1]);
		}

		// Token: 0x0400003E RID: 62
		private readonly List<T> m_stops;
	}
}
