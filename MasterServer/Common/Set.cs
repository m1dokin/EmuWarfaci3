using System;
using System.Collections;
using System.Collections.Generic;

namespace MasterServer.Common
{
	// Token: 0x02000154 RID: 340
	public class Set<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		// Token: 0x060005E2 RID: 1506 RVA: 0x000178A1 File Offset: 0x00015CA1
		public Set()
		{
		}

		// Token: 0x060005E3 RID: 1507 RVA: 0x000178B4 File Offset: 0x00015CB4
		public Set(Set<T> other)
		{
			this.m_list.AddRange(other.m_list);
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x000178D8 File Offset: 0x00015CD8
		public Set(IEnumerable<T> elements)
		{
			this.Add(elements);
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x000178F4 File Offset: 0x00015CF4
		public void Add(T el)
		{
			if (this.m_list.Count == 0)
			{
				this.m_list.Add(el);
			}
			int num = this.find(el);
			int num2 = Comparer<T>.Default.Compare(this.m_list[num], el);
			if (num2 < 0)
			{
				this.m_list.Insert(num + 1, el);
			}
			else if (num2 > 0)
			{
				this.m_list.Insert(num, el);
			}
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x0001796C File Offset: 0x00015D6C
		public void Add(IEnumerable<T> elements)
		{
			foreach (T el in elements)
			{
				this.Add(el);
			}
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x000179C0 File Offset: 0x00015DC0
		public Set<T> Union(Set<T> other)
		{
			return new Set<T>(this)
			{
				other
			};
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x000179DC File Offset: 0x00015DDC
		public bool Remove(T el)
		{
			int num = this.find(el);
			if (num >= 0 && EqualityComparer<T>.Default.Equals(this.m_list[num], el))
			{
				this.m_list.RemoveAt(num);
				return true;
			}
			return false;
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00017A24 File Offset: 0x00015E24
		public void Remove(IEnumerable<T> elements)
		{
			foreach (T el in elements)
			{
				this.Remove(el);
			}
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00017A7C File Offset: 0x00015E7C
		public Set<T> Subtract(Set<T> other)
		{
			Set<T> set = new Set<T>(this);
			set.Remove(other);
			return set;
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00017A98 File Offset: 0x00015E98
		public Set<T> Intersect(Set<T> other)
		{
			Set<T> set = new Set<T>();
			foreach (T item in this.m_list)
			{
				if (other.Contains(item))
				{
					set.m_list.Add(item);
				}
			}
			return set;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00017B0C File Offset: 0x00015F0C
		public Set<T> SymetricDifference(Set<T> other)
		{
			Set<T> set = new Set<T>();
			foreach (T t in this.m_list)
			{
				if (!other.Contains(t))
				{
					set.Add(t);
				}
			}
			foreach (T t2 in other.m_list)
			{
				if (!this.Contains(t2))
				{
					set.Add(t2);
				}
			}
			return set;
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00017BD4 File Offset: 0x00015FD4
		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj == null || !(obj is Set<T>))
			{
				return false;
			}
			Set<T> set = (Set<T>)obj;
			if (set.m_list.Count != this.m_list.Count)
			{
				return false;
			}
			for (int num = 0; num != this.m_list.Count; num++)
			{
				if (!EqualityComparer<T>.Default.Equals(this.m_list[num], set.m_list[num]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00017C6C File Offset: 0x0001606C
		public override int GetHashCode()
		{
			return this.m_list.GetHashCode();
		}

		// Token: 0x170000A1 RID: 161
		public T this[int index]
		{
			get
			{
				return this.m_list[index];
			}
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x00017C87 File Offset: 0x00016087
		public void Clear()
		{
			this.m_list.Clear();
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00017C94 File Offset: 0x00016094
		public bool Contains(T item)
		{
			int num = this.find(item);
			return num >= 0 && EqualityComparer<T>.Default.Equals(this.m_list[num], item);
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00017CCA File Offset: 0x000160CA
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.m_list.CopyTo(array, arrayIndex);
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060005F3 RID: 1523 RVA: 0x00017CD9 File Offset: 0x000160D9
		public int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060005F4 RID: 1524 RVA: 0x00017CE6 File Offset: 0x000160E6
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x00017CE9 File Offset: 0x000160E9
		IEnumerator<T> IEnumerable<!0>.GetEnumerator()
		{
			return this.m_list.GetEnumerator();
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x00017CFB File Offset: 0x000160FB
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.m_list.GetEnumerator();
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x00017D0D File Offset: 0x0001610D
		private int find(T value)
		{
			return this.find(value, -1, this.m_list.Count - 1);
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x00017D24 File Offset: 0x00016124
		private int find(T value, int low, int high)
		{
			while (high - low > 1)
			{
				int num = (high + low) / 2;
				int num2 = Comparer<T>.Default.Compare(value, this.m_list[num]);
				if (num2 < 0)
				{
					high = num;
				}
				else
				{
					if (num2 <= 0)
					{
						return num;
					}
					low = num;
				}
			}
			return high;
		}

		// Token: 0x040003D1 RID: 977
		private List<T> m_list = new List<T>();
	}
}
