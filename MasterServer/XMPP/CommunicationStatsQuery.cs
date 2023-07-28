using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.XMPP
{
	// Token: 0x0200080A RID: 2058
	[QueryAttributes(TagName = "communication_stats")]
	internal class CommunicationStatsQuery : BaseQuery
	{
		// Token: 0x06002A2E RID: 10798 RVA: 0x000B6027 File Offset: 0x000B4427
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			if (Resources.BootstrapMode)
			{
				request.SetAttribute("bootstrap", Resources.BootstrapName);
			}
		}

		// Token: 0x06002A2F RID: 10799 RVA: 0x000B6044 File Offset: 0x000B4444
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			int total_online = int.Parse(response.GetAttribute("online"));
			List<SOnlineServer> list = new List<SOnlineServer>();
			IEnumerator enumerator = response.FirstChild.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					SOnlineServer item = new SOnlineServer
					{
						Resource = xmlElement.GetAttribute("resource"),
						Users = int.Parse(xmlElement.GetAttribute("online")),
						Channel = xmlElement.GetAttribute("channel")
					};
					if (string.Compare(item.Resource, Resources.XmppResource, true) != 0)
					{
						list.Add(item);
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
			OnlineClient onlineClient = (OnlineClient)ServicesManager.GetService<IOnlineClient>();
			onlineClient.SetOnlineServers(list);
			ICommunicationStatsService service = ServicesManager.GetService<ICommunicationStatsService>();
			service.StatsUpdate(total_online);
			return null;
		}
	}
}
