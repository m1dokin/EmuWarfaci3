using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x0200021D RID: 541
	[DebugQuery]
	[QueryAttributes(TagName = "debug_expiration_time_subtract")]
	internal class ExpirationTimeSubtractQuery : BaseQuery
	{
		// Token: 0x06000BBE RID: 3006 RVA: 0x0002C8CC File Offset: 0x0002ACCC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ExpirationTimeSubtractQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					ulong inventoryID = ulong.Parse(request.GetAttribute("item_id"));
					uint seconds = uint.Parse(request.GetAttribute("seconds"));
					ICatalogService service = ServicesManager.GetService<ICatalogService>();
					IProfileItems service2 = ServicesManager.GetService<IProfileItems>();
					SProfileItem profileItem = service2.GetProfileItem(user.ProfileID, inventoryID);
					IDALService service3 = ServicesManager.GetService<IDALService>();
					bool flag = true;
					if (profileItem.ExpirationTimeUTC != 0UL && profileItem.TotalDurabilityPoints == 0)
					{
						flag &= service3.ECatalog.DebugExpireItem(user.UserID, profileItem.CatalogID, seconds);
					}
					if (flag)
					{
						response.SetAttribute("item_id", inventoryID.ToString());
						profileItem = service2.GetProfileItem(user.ProfileID, inventoryID);
						response.SetAttribute("expiration_time", profileItem.ExpirationTimeUTC.ToString());
						response.SetAttribute("seconds_left", Math.Max(profileItem.SecondsLeft, 0).ToString());
					}
					result = ((!flag) ? -1 : 0);
				}
			}
			return result;
		}
	}
}
