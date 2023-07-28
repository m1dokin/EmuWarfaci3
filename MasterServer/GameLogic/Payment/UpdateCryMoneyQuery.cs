using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.Payment
{
	// Token: 0x020003E6 RID: 998
	[QueryAttributes(TagName = "update_cry_money")]
	internal class UpdateCryMoneyQuery : BaseQuery
	{
		// Token: 0x060015B2 RID: 5554 RVA: 0x0005A779 File Offset: 0x00058B79
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			request.SetAttribute("cry_money", args[0].ToString());
		}

		// Token: 0x04000A58 RID: 2648
		public const string Name = "update_cry_money";
	}
}
