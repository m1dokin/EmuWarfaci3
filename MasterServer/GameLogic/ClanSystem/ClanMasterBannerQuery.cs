using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000272 RID: 626
	[QueryAttributes(TagName = "clan_masterbanner_update")]
	internal class ClanMasterBannerQuery : BaseQuery
	{
		// Token: 0x06000D8D RID: 3469 RVA: 0x00036818 File Offset: 0x00034C18
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = queryParams[0] as string;
			ulong profileId = (ulong)queryParams[1];
			request.SetAttribute("bcast_receivers", value);
			IDALService service = ServicesManager.GetService<IDALService>();
			SProfileInfo profileInfo = service.ProfileSystem.GetProfileInfo(profileId);
			request.SetAttribute("master_badge", profileInfo.Banner.Badge.ToString());
			request.SetAttribute("master_stripe", profileInfo.Banner.Stripe.ToString());
			request.SetAttribute("master_mark", profileInfo.Banner.Mark.ToString());
		}
	}
}
