using System;
using System.Xml;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x0200040D RID: 1037
	[QueryAttributes(TagName = "mute_user")]
	internal class MuteUserQuery : BaseQuery
	{
		// Token: 0x06001667 RID: 5735 RVA: 0x0005E2D0 File Offset: 0x0005C6D0
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			DateTime dateTime = (DateTime)queryParams[1];
			request.SetAttribute("nickname", value);
			request.SetAttribute("expiration_time", dateTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
		}

		// Token: 0x04000AE0 RID: 2784
		public const string Name = "mute_user";
	}
}
