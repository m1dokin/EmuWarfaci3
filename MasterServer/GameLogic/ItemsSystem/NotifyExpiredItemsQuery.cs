using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200036E RID: 878
	[QueryAttributes(TagName = "notify_expired_items")]
	internal class NotifyExpiredItemsQuery : BaseQuery
	{
		// Token: 0x060013C0 RID: 5056 RVA: 0x00050DB0 File Offset: 0x0004F1B0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "NotifyExpiredItemsQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					IProfileItems service = ServicesManager.GetService<IProfileItems>();
					Dictionary<ulong, SProfileItem> expiredProfileItems = service.GetExpiredProfileItems(user.ProfileID);
					IEnumerator enumerator = request.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlElement xmlElement = (XmlElement)obj;
							if (xmlElement.Name == "item")
							{
								ulong num = ulong.Parse(xmlElement.GetAttribute("item_id"));
								if (expiredProfileItems.ContainsKey(num))
								{
									service.ExpireProfileItemConfirmed(user.ProfileID, num);
								}
							}
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
					result = 0;
				}
			}
			return result;
		}
	}
}
