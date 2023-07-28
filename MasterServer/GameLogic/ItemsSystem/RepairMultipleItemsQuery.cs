using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200037C RID: 892
	[QueryAttributes(TagName = "repair_multiple_items")]
	internal class RepairMultipleItemsQuery : BaseQuery
	{
		// Token: 0x06001439 RID: 5177 RVA: 0x000521DC File Offset: 0x000505DC
		public RepairMultipleItemsQuery(IItemService itemService, ICatalogService catalogService)
		{
			this.m_itemService = itemService;
			this.m_catalogService = catalogService;
		}

		// Token: 0x0600143A RID: 5178 RVA: 0x000521F4 File Offset: 0x000505F4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "RepairMultipleItemsQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					List<ulong> profileItems = (from XmlElement itemEl in request.ChildNodes
					where itemEl.Name == "item"
					select ulong.Parse(itemEl.GetAttribute("item_id"))).ToList<ulong>();
					RepairEquipmentOperationResult repairEquipmentOperationResult = this.m_itemService.RepairMultipleItems(user, profileItems);
					response.AppendChild(repairEquipmentOperationResult.ToXml(response.OwnerDocument));
					response.SetAttribute("game_money", this.m_catalogService.GetCustomerAccount(user.UserID, Currency.GameMoney).Money.ToString());
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0400095A RID: 2394
		private readonly IItemService m_itemService;

		// Token: 0x0400095B RID: 2395
		private readonly ICatalogService m_catalogService;
	}
}
