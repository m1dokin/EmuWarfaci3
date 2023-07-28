using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000225 RID: 549
	[DebugQuery]
	[QueryAttributes(TagName = "debug_unlock_item")]
	internal class UnlockItemQuery : BaseQuery
	{
		// Token: 0x06000BCF RID: 3023 RVA: 0x0002CEA4 File Offset: 0x0002B2A4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "UnlockItemQuery"))
			{
				ulong num;
				if (!base.GetClientProfileId(fromJid, out num))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("item_name");
					if (string.IsNullOrEmpty(attribute))
					{
						IDALService service = ServicesManager.GetService<IDALService>();
						service.ItemSystem.DebugUnlockAllItems(num);
					}
					else
					{
						IItemCache service2 = ServicesManager.GetService<IItemCache>();
						IProfileItems service3 = ServicesManager.GetService<IProfileItems>();
						service3.UnlockItem(num, service2.GetAllItemsByName()[attribute].ID);
					}
					response.SetAttribute("item_name", attribute);
					result = 0;
				}
			}
			return result;
		}
	}
}
