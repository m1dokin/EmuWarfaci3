using System;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000368 RID: 872
	public class TaggedItemDesc
	{
		// Token: 0x0600137A RID: 4986 RVA: 0x0004F963 File Offset: 0x0004DD63
		public TaggedItemDesc(UserTags tags)
		{
			this.m_tagsFilter = new TagsFilter(tags.ToString());
		}

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x0600137B RID: 4987 RVA: 0x0004F97C File Offset: 0x0004DD7C
		public TagsFilter Filter
		{
			get
			{
				return this.m_tagsFilter;
			}
		}

		// Token: 0x0400091B RID: 2331
		private readonly TagsFilter m_tagsFilter;
	}
}
