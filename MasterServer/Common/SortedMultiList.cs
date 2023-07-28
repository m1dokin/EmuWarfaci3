using System;
using System.Collections.Generic;

namespace MasterServer.Common
{
	// Token: 0x02000023 RID: 35
	public class SortedMultiList<TKey, TValue>
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000078 RID: 120 RVA: 0x000068EC File Offset: 0x00004CEC
		public List<KeyValuePair<TKey, TValue>> Data
		{
			get
			{
				return this.data;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000068F4 File Offset: 0x00004CF4
		private void Add(KeyValuePair<TKey, TValue> element)
		{
			int num = 0;
			int num2 = this.data.Count;
			if (this.data.Count != 0)
			{
				do
				{
					int num3 = (num2 + num) / 2;
					if (Comparer<TKey>.Default.Compare(this.data[num3].Key, element.Key) == 1)
					{
						num2 = num3;
					}
					else
					{
						num = num3;
					}
				}
				while (num2 - num > 1);
			}
			this.data.Insert(num2, element);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x0000696F File Offset: 0x00004D6F
		public void Add(TKey key, TValue value)
		{
			this.Add(new KeyValuePair<TKey, TValue>(key, value));
		}

		// Token: 0x04000047 RID: 71
		private List<KeyValuePair<TKey, TValue>> data = new List<KeyValuePair<TKey, TValue>>();
	}
}
