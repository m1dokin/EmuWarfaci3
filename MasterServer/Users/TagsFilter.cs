using System;

namespace MasterServer.Users
{
	// Token: 0x0200075B RID: 1883
	public class TagsFilter
	{
		// Token: 0x060026E7 RID: 9959 RVA: 0x000A471C File Offset: 0x000A2B1C
		public TagsFilter(string tags)
		{
			this.m_filterTags = new UserTags(tags);
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x000A4730 File Offset: 0x000A2B30
		public bool Check(UserTags tags)
		{
			return !this.m_filterTags.HasAny() || tags.HasAnyTag(this.m_filterTags);
		}

		// Token: 0x060026E9 RID: 9961 RVA: 0x000A4751 File Offset: 0x000A2B51
		public override string ToString()
		{
			return this.m_filterTags.ToString();
		}

		// Token: 0x04001402 RID: 5122
		private readonly UserTags m_filterTags;
	}
}
