using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000221 RID: 545
	[DebugQuery]
	[QueryAttributes(TagName = "dbg_reset_player_stats")]
	internal class ResetPersonalStatsQuery : BaseQuery
	{
		// Token: 0x06000BC6 RID: 3014 RVA: 0x0002CC94 File Offset: 0x0002B094
		public ResetPersonalStatsQuery(ITelemetryDALService telemetryDal)
		{
			this.m_telemetryDal = telemetryDal;
		}

		// Token: 0x06000BC7 RID: 3015 RVA: 0x0002CCA4 File Offset: 0x0002B0A4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ResetPersonalStatsQuery"))
			{
				ulong profileId;
				if (!base.GetClientProfileId(fromJid, out profileId))
				{
					result = -3;
				}
				else
				{
					this.m_telemetryDal.TelemetrySystem.ResetPlayerStats(profileId);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0400057A RID: 1402
		private readonly ITelemetryDALService m_telemetryDal;
	}
}
