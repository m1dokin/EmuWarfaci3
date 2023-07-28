using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.XMPP
{
	// Token: 0x02000814 RID: 2068
	[QueryAttributes(TagName = "masterserver_presence")]
	internal class ServerPresenceQuery : BaseQuery
	{
		// Token: 0x06002A6D RID: 10861 RVA: 0x000B7099 File Offset: 0x000B5499
		public ServerPresenceQuery(IRankSystem rankSystem, IOnlineClient onlineClient)
		{
			this.m_rankSystem = rankSystem;
			this.m_onlineClient = onlineClient;
		}

		// Token: 0x06002A6E RID: 10862 RVA: 0x000B70B0 File Offset: 0x000B54B0
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			request.SetAttribute("server_id", Resources.ServerID.ToString());
			request.SetAttribute("resource", Resources.XmppResource);
			request.SetAttribute("channel", Resources.ChannelName);
			request.SetAttribute("realm_id", Resources.RealmId.ToString());
			request.SetAttribute("rank_group", ((Resources.ChannelRankGroup)queryParams[0]).ToString().ToLower());
			request.SetAttribute("presence", queryParams[1].ToString());
			ServerLoadStats serverLoadStats = queryParams[2] as ServerLoadStats;
			request.SetAttribute("online_users", serverLoadStats.Online.ToString());
			request.SetAttribute("total_load", serverLoadStats.Load.ToString());
			request.SetAttribute("load_stats_types", string.Join(",", from stat in serverLoadStats.LoadStats
			select stat.Item1));
			request.SetAttribute("load_stats", string.Join<float>(",", from stat in serverLoadStats.LoadStats
			select stat.Item2));
			request.SetAttribute("online_users_regions", string.Join(",", from stat in serverLoadStats.OnlineStats
			select stat.Item1));
			request.SetAttribute("online_users_count", string.Join<int>(",", from stat in serverLoadStats.OnlineStats
			select stat.Item2));
			if (Resources.BootstrapMode)
			{
				request.SetAttribute("bootstrap", Resources.BootstrapName);
			}
			request.SetAttribute("min_rank", this.m_rankSystem.ChannelMinRank.ToString());
			request.SetAttribute("max_rank", this.m_rankSystem.ChannelMaxRank.ToString());
		}

		// Token: 0x06002A6F RID: 10863 RVA: 0x000B72F4 File Offset: 0x000B56F4
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
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
					if (string.Compare(item.Resource, Resources.XmppResource, true, CultureInfo.InvariantCulture) != 0)
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
			this.m_onlineClient.SetOnlineServers(list);
			return null;
		}

		// Token: 0x040016B2 RID: 5810
		public const string QueryName = "masterserver_presence";

		// Token: 0x040016B3 RID: 5811
		private readonly IRankSystem m_rankSystem;

		// Token: 0x040016B4 RID: 5812
		private readonly IOnlineClient m_onlineClient;
	}
}
