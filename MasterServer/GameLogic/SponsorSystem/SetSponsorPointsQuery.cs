using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.GameLogic.SponsorSystem
{
	// Token: 0x02000226 RID: 550
	[DebugQuery]
	[QueryAttributes(TagName = "debug_set_sponsor_points")]
	internal class SetSponsorPointsQuery : BaseQuery
	{
		// Token: 0x06000BD1 RID: 3025 RVA: 0x0002CF64 File Offset: 0x0002B364
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			ulong profileId;
			if (!base.GetClientProfileId(fromJid, out profileId))
			{
				return -3;
			}
			uint sponsorId = uint.Parse(request.GetAttribute("sponsor"));
			ulong sponsorPts = (ulong)uint.Parse(request.GetAttribute("points"));
			IDALService service = ServicesManager.GetService<IDALService>();
			service.RewardsSystem.SetSponsorPoints(profileId, sponsorId, sponsorPts);
			return 0;
		}
	}
}
