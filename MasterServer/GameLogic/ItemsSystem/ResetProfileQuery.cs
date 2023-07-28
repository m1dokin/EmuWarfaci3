using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.GameInterface;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000787 RID: 1927
	[DebugQuery]
	[QueryAttributes(TagName = "resetcharacter")]
	internal class ResetProfileQuery : BaseQuery
	{
		// Token: 0x060027ED RID: 10221 RVA: 0x000AB294 File Offset: 0x000A9694
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ResetProfileQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					bool full = request.GetAttribute("depth") == "full";
					IGameInterface service = ServicesManager.GetService<IGameInterface>();
					using (IGameInterfaceContext gameInterfaceContext = service.CreateContext(AccessLevel.Debug))
					{
						gameInterfaceContext.ResetProfile(user.UserID, full);
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
