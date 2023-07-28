using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000218 RID: 536
	[DebugQuery]
	[QueryAttributes(TagName = "debug_get_item_status")]
	internal class DebugGetItemStatus : BaseQuery
	{
		// Token: 0x06000BAC RID: 2988 RVA: 0x0002BFF8 File Offset: 0x0002A3F8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugGetItemStatus"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					ulong inventoryID = ulong.Parse(request.GetAttribute("item_id"));
					IProfileItems service = ServicesManager.GetService<IProfileItems>();
					SProfileItem profileItem = service.GetProfileItem(user.ProfileID, inventoryID);
					if (profileItem == null)
					{
						result = -1;
					}
					else
					{
						response.SetAttribute("status", profileItem.Status.ToString());
						response.SetAttribute("item_id", inventoryID.ToString());
						result = 0;
					}
				}
			}
			return result;
		}
	}
}
