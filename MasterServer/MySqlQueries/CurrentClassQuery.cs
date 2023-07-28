using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.Users;

namespace MasterServer.MySqlQueries
{
	// Token: 0x020007C3 RID: 1987
	[QueryAttributes(TagName = "setcurrentclass")]
	internal class CurrentClassQuery : BaseQuery
	{
		// Token: 0x060028BF RID: 10431 RVA: 0x000B08C3 File Offset: 0x000AECC3
		public CurrentClassQuery(IClassChangingService classChanging)
		{
			this.m_classChanging = classChanging;
		}

		// Token: 0x060028C0 RID: 10432 RVA: 0x000B08D4 File Offset: 0x000AECD4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "CurrentClassQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					uint classId = uint.Parse(request.GetAttribute("current_class"));
					if (this.m_classChanging.ChangePlayersClass(user, classId) == ClassChangeStatus.Ok)
					{
						result = 0;
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}

		// Token: 0x040015A3 RID: 5539
		private readonly IClassChangingService m_classChanging;
	}
}
