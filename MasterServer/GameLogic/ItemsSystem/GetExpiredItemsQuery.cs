using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000342 RID: 834
	[QueryAttributes(TagName = "get_expired_items")]
	internal class GetExpiredItemsQuery : BaseQuery
	{
		// Token: 0x060012AE RID: 4782 RVA: 0x0004B398 File Offset: 0x00049798
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetExpiredItemsQuery"))
			{
				ulong profileId;
				if (!base.GetClientProfileId(fromJid, out profileId))
				{
					result = -3;
				}
				else
				{
					ProfileProxy profile = new ProfileProxy(profileId);
					ProfileReader profileReader = new ProfileReader(profile);
					profileReader.ReadPendingNotifications(response);
					if (profileReader.ReadExpiredItems(response))
					{
						profileReader.ReadProfileItems(response);
						profileReader.ReadUnlockedItems(response);
					}
					else
					{
						profileReader.ReadDurableAndConsumableItems(response);
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
