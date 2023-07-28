using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200049D RID: 1181
	[QueryAttributes(TagName = "map_vote_finished")]
	internal class MapVotingFinishedQuery : BaseQuery
	{
		// Token: 0x0600192F RID: 6447 RVA: 0x00066BF8 File Offset: 0x00064FF8
		public override void SendRequest(string online_id, XmlElement request, params object[] args)
		{
			string value = args[0].ToString();
			request.SetAttribute("bcast_receivers", value);
			string value2 = args[1].ToString();
			request.SetAttribute("mission_uid", value2);
		}

		// Token: 0x04000C0A RID: 3082
		internal const string QueryName = "map_vote_finished";
	}
}
