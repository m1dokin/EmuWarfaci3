using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200037D RID: 893
	[QueryAttributes(TagName = "repair_item")]
	internal class RepairPermanentItemQuery : BaseQuery
	{
		// Token: 0x0600143D RID: 5181 RVA: 0x00052318 File Offset: 0x00050718
		public RepairPermanentItemQuery(IItemService itemService)
		{
			this.m_itemService = itemService;
		}

		// Token: 0x0600143E RID: 5182 RVA: 0x00052328 File Offset: 0x00050728
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "RepairPermanentItemQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					ulong itemId = ulong.Parse(request.GetAttribute("item_id"));
					bool flag = int.Parse(request.GetAttribute("accept_repair")) != 0;
					ulong repairCost = ulong.Parse(request.GetAttribute("repair_cost"));
					response.SetAttribute("accept_repair", (!flag) ? "0" : "1");
					result = ((!flag) ? this.RejectItem(user, itemId) : this.RepairItem(user, itemId, repairCost, response));
				}
			}
			return result;
		}

		// Token: 0x0600143F RID: 5183 RVA: 0x000523F8 File Offset: 0x000507F8
		private int RepairItem(UserInfo.User user, ulong itemId, ulong repairCost, XmlElement response)
		{
			int result = 0;
			try
			{
				RepairItemResponse repairItemResponse = this.m_itemService.RepairItem(user, itemId, repairCost);
				response.SetAttribute("total_durability", repairItemResponse.TotalDurability.ToString());
				response.SetAttribute("durability", repairItemResponse.Durability.ToString());
				response.SetAttribute("game_money", repairItemResponse.GameMoney.ToString());
				response.SetAttribute("repair_cost", repairItemResponse.RepairCost.ToString());
			}
			catch (ItemServiceNotEnoughtMoneyException)
			{
				result = 1;
			}
			catch (ItemServiceException e)
			{
				Log.Error(e);
				result = -1;
			}
			return result;
		}

		// Token: 0x06001440 RID: 5184 RVA: 0x000524D8 File Offset: 0x000508D8
		private int RejectItem(UserInfo.User user, ulong itemId)
		{
			this.m_itemService.DeleteItem(user, itemId, delegate(SProfileItem item)
			{
				if (item.OfferType != OfferType.Permanent)
				{
					throw new ItemServiceException(string.Format("Cannot reject not expired permanent item {0} of profile {1}", item.ProfileItemID, user.ProfileID));
				}
			});
			return 0;
		}

		// Token: 0x0400095E RID: 2398
		private const int REPAIR_NOT_ENOUGH_MONEY = 1;

		// Token: 0x0400095F RID: 2399
		private readonly IItemService m_itemService;
	}
}
