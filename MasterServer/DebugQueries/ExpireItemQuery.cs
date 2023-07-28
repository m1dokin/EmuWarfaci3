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
	// Token: 0x0200021E RID: 542
	[DebugQuery]
	[QueryAttributes(TagName = "debug_expire_item")]
	internal class ExpireItemQuery : BaseQuery
	{
		// Token: 0x06000BC0 RID: 3008 RVA: 0x0002CA40 File Offset: 0x0002AE40
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ExpireItemQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("item_id");
					ulong num = ulong.Parse(attribute);
					ICatalogService service = ServicesManager.GetService<ICatalogService>();
					IProfileItems service2 = ServicesManager.GetService<IProfileItems>();
					SProfileItem profileItem = service2.GetProfileItem(user.ProfileID, num);
					bool flag = true;
					if (profileItem.ExpirationTimeUTC != 0UL && profileItem.TotalDurabilityPoints == 0)
					{
						IDALService service3 = ServicesManager.GetService<IDALService>();
						flag &= service3.ECatalog.DebugExpireItem(user.UserID, profileItem.CatalogID, 0U);
					}
					IItemsExpiration service4 = ServicesManager.GetService<IItemsExpiration>();
					flag &= service4.ExpireItem(user.UserID, user.ProfileID, num);
					result = ((!flag) ? -1 : 0);
				}
			}
			return result;
		}
	}
}
