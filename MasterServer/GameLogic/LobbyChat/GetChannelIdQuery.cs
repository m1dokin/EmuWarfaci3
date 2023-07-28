using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.LobbyChat
{
	// Token: 0x02000678 RID: 1656
	[QueryAttributes(TagName = "lobbychat_getchannelid")]
	internal class GetChannelIdQuery : BaseQuery
	{
		// Token: 0x060022F7 RID: 8951 RVA: 0x00092396 File Offset: 0x00090796
		public GetChannelIdQuery(IChatConferences chatConferences)
		{
			this.m_chatConferences = chatConferences;
		}

		// Token: 0x060022F8 RID: 8952 RVA: 0x000923A8 File Offset: 0x000907A8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetChannelIdQuery.UpdateXml"))
			{
				ulong profileID;
				if (!base.GetClientProfileId(fromJid, out profileID))
				{
					result = -3;
				}
				else
				{
					EChatChannel echatChannel = (EChatChannel)int.Parse(request.GetAttribute("channel"));
					ChatChannelID chatChannelID = this.m_chatConferences.GenerateChannelId(echatChannel, profileID);
					if (!chatChannelID.IsEmpty())
					{
						string name = "channel";
						int num = (int)echatChannel;
						response.SetAttribute(name, num.ToString());
						response.SetAttribute("channel_id", chatChannelID.ChannelID);
						response.SetAttribute("service_id", chatChannelID.ConferenceID);
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

		// Token: 0x0400118E RID: 4494
		private IChatConferences m_chatConferences;
	}
}
