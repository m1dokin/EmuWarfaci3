using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000369 RID: 873
	public class MetaGameDesc
	{
		// Token: 0x0600137C RID: 4988 RVA: 0x0004F984 File Offset: 0x0004DD84
		public MetaGameDesc(string name)
		{
			this.Name = name;
		}

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x0600137D RID: 4989 RVA: 0x0004F99E File Offset: 0x0004DD9E
		// (set) Token: 0x0600137E RID: 4990 RVA: 0x0004F9A6 File Offset: 0x0004DDA6
		public string Name { get; private set; }

		// Token: 0x0600137F RID: 4991 RVA: 0x0004F9B0 File Offset: 0x0004DDB0
		public IEnumerable<string> Get(string key)
		{
			List<string> list;
			return (!this.m_desc.TryGetValue(key, out list)) ? new List<string>() : list;
		}

		// Token: 0x06001380 RID: 4992 RVA: 0x0004F9DC File Offset: 0x0004DDDC
		public void Add(string key, string value)
		{
			List<string> list;
			if (this.m_desc.TryGetValue(key, out list))
			{
				list.Add(value);
			}
			else
			{
				this.m_desc.Add(key, new List<string>
				{
					value
				});
			}
		}

		// Token: 0x0400091D RID: 2333
		private readonly Dictionary<string, List<string>> m_desc = new Dictionary<string, List<string>>();
	}
}
