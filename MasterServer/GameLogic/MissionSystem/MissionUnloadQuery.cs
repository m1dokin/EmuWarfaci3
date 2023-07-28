using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000797 RID: 1943
	[QueryAttributes(TagName = "mission_unload")]
	internal class MissionUnloadQuery : BaseQuery
	{
		// Token: 0x0600284B RID: 10315 RVA: 0x000AD964 File Offset: 0x000ABD64
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
		}
	}
}
