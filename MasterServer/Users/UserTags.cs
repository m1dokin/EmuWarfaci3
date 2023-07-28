using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.Users
{
	// Token: 0x020007E0 RID: 2016
	public class UserTags : IEquatable<UserTags>
	{
		// Token: 0x06002946 RID: 10566 RVA: 0x000B3470 File Offset: 0x000B1870
		public UserTags(string tags = null)
		{
			HashSet<string> tags2;
			if (string.IsNullOrEmpty(tags))
			{
				tags2 = new HashSet<string>();
			}
			else
			{
				tags2 = new HashSet<string>(from s in tags.Split(new string[]
				{
					","
				}, StringSplitOptions.RemoveEmptyEntries)
				select s.Trim());
			}
			this.m_tags = tags2;
		}

		// Token: 0x170003D8 RID: 984
		// (get) Token: 0x06002947 RID: 10567 RVA: 0x000B34DA File Offset: 0x000B18DA
		public IEnumerable<string> List
		{
			get
			{
				return this.m_tags.ToList<string>();
			}
		}

		// Token: 0x06002948 RID: 10568 RVA: 0x000B34E7 File Offset: 0x000B18E7
		public void Add(UserTags userTags)
		{
			this.m_tags.UnionWith(userTags.List);
		}

		// Token: 0x06002949 RID: 10569 RVA: 0x000B34FA File Offset: 0x000B18FA
		public void Remove(UserTags userTags)
		{
			this.m_tags.ExceptWith(userTags.List);
		}

		// Token: 0x0600294A RID: 10570 RVA: 0x000B350D File Offset: 0x000B190D
		public bool HasAny()
		{
			return this.m_tags.Any<string>();
		}

		// Token: 0x0600294B RID: 10571 RVA: 0x000B351A File Offset: 0x000B191A
		public bool HasAnyTag(UserTags tags)
		{
			return this.m_tags.Overlaps(tags.List);
		}

		// Token: 0x0600294C RID: 10572 RVA: 0x000B352D File Offset: 0x000B192D
		public override string ToString()
		{
			return string.Join(",", this.List);
		}

		// Token: 0x0600294D RID: 10573 RVA: 0x000B353F File Offset: 0x000B193F
		public bool Equals(UserTags other)
		{
			return this.m_tags.SetEquals(other.m_tags);
		}

		// Token: 0x040015FB RID: 5627
		public const string TagDelimiter = ",";

		// Token: 0x040015FC RID: 5628
		private readonly HashSet<string> m_tags;
	}
}
