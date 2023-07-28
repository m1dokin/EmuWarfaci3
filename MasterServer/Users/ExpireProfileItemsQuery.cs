using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.Users
{
	// Token: 0x02000750 RID: 1872
	[DebugQuery]
	[QueryAttributes(TagName = "expire_profile_items")]
	internal class ExpireProfileItemsQuery : BaseQuery
	{
		// Token: 0x060026A9 RID: 9897 RVA: 0x000A3D6B File Offset: 0x000A216B
		public ExpireProfileItemsQuery(IItemsExpiration itemsExpiration)
		{
			this.m_itemsExpiration = itemsExpiration;
		}

		// Token: 0x060026AA RID: 9898 RVA: 0x000A3D7C File Offset: 0x000A217C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "expire_profile_items"))
			{
				ClassPresenceData classPresenceData = new ClassPresenceData();
				ulong key = ulong.Parse(request.GetAttribute("profile_id"));
				int classId = int.Parse(request.GetAttribute("class_id"));
				int playedTimeSec = int.Parse(request.GetAttribute("time_played"));
				classPresenceData.presence.Add(key, new List<ClassPresenceData.PresenceData>
				{
					new ClassPresenceData.PresenceData
					{
						ClassId = classId,
						PlayedTimeSec = playedTimeSec
					}
				});
				classPresenceData.sessionId = "expire_profile_items";
				this.m_itemsExpiration.ExpireItems(classPresenceData);
				result = 0;
			}
			return result;
		}

		// Token: 0x040013EC RID: 5100
		public const string QUERY_NAME = "expire_profile_items";

		// Token: 0x040013ED RID: 5101
		private readonly IItemsExpiration m_itemsExpiration;
	}
}
