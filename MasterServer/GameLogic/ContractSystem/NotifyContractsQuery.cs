using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x0200029B RID: 667
	[QueryAttributes(TagName = "notify_contracts")]
	internal class NotifyContractsQuery : BaseQuery
	{
		// Token: 0x06000E6A RID: 3690 RVA: 0x0003A1BC File Offset: 0x000385BC
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			ProfileContract profileContract = (ProfileContract)queryParams[0];
			request.AppendChild(profileContract.ToXml(request.OwnerDocument));
		}
	}
}
