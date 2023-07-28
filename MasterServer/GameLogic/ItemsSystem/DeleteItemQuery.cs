using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.ElectronicCatalog;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000340 RID: 832
	[QueryAttributes(TagName = "delete_item")]
	internal class DeleteItemQuery : BaseQuery
	{
		// Token: 0x060012A9 RID: 4777 RVA: 0x0004AFEC File Offset: 0x000493EC
		public DeleteItemQuery(ItemService itemService)
		{
			this.m_itemService = itemService;
		}

		// Token: 0x060012AA RID: 4778 RVA: 0x0004AFFC File Offset: 0x000493FC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DeleteItemQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					ulong profileItemId = ulong.Parse(request.GetAttribute("item_id"));
					this.m_itemService.DeleteItem(user, profileItemId);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0400089E RID: 2206
		private readonly ItemService m_itemService;
	}
}
