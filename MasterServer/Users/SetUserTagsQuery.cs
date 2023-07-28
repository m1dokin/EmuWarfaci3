using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x0200075A RID: 1882
	[QueryAttributes(TagName = "set_user_tags")]
	internal class SetUserTagsQuery : BaseQuery
	{
		// Token: 0x060026E4 RID: 9956 RVA: 0x000A4689 File Offset: 0x000A2A89
		public SetUserTagsQuery(ITagService tagService)
		{
			this.m_tagService = tagService;
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x000A4698 File Offset: 0x000A2A98
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			ulong num = ulong.Parse(queryParams[0].ToString());
			string value = queryParams[1].ToString();
			request.SetAttribute("user_id", num.ToString());
			request.SetAttribute("tags", value);
		}

		// Token: 0x060026E6 RID: 9958 RVA: 0x000A46E0 File Offset: 0x000A2AE0
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			ulong userId = ulong.Parse(request.GetAttribute("user_id"));
			UserTags userTags = new UserTags(request.GetAttribute("tags"));
			this.m_tagService.SyncUserTags(userId);
			return 0;
		}

		// Token: 0x04001400 RID: 5120
		public const string QueryName = "set_user_tags";

		// Token: 0x04001401 RID: 5121
		private readonly ITagService m_tagService;
	}
}
