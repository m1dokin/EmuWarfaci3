using System;
using System.Xml;
using DedicatedPoolServer.Model;
using MasterServer.CryOnlineNET;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C2 RID: 1730
	[QueryAttributes(TagName = "lock_server")]
	internal class LockServerQuery : BaseQuery
	{
		// Token: 0x06002444 RID: 9284 RVA: 0x00097538 File Offset: 0x00095938
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			request.SetAttribute("build_type", (string)args[0]);
			request.SetAttribute("mode", ((DedicatedMode)args[1] != DedicatedMode.PurePVP) ? "pvp_pve" : "pure_pvp");
			request.SetAttribute("region_id", (string)args[2]);
			string value = (string)args[3];
			if (!string.IsNullOrEmpty(value))
			{
				request.SetAttribute("server_id", value);
			}
		}

		// Token: 0x06002445 RID: 9285 RVA: 0x000975B4 File Offset: 0x000959B4
		public override object HandleResponse(SOnlineQuery query, XmlElement response)
		{
			string attribute = response.GetAttribute("server_id");
			string attribute2 = response.GetAttribute("server_jid");
			if (string.IsNullOrEmpty(attribute) || string.IsNullOrEmpty(attribute2))
			{
				return string.Empty;
			}
			return attribute;
		}

		// Token: 0x0400122B RID: 4651
		public const string QueryName = "lock_server";

		// Token: 0x0400122C RID: 4652
		private const string BuildTypeAttributeName = "build_type";

		// Token: 0x0400122D RID: 4653
		private const string ModeAttributeName = "mode";

		// Token: 0x0400122E RID: 4654
		private const string ServerIdAttributeName = "server_id";

		// Token: 0x0400122F RID: 4655
		private const string ServerJidAttributeName = "server_jid";

		// Token: 0x04001230 RID: 4656
		private const string RegionIdAttibuteName = "region_id";
	}
}
