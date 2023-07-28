using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000223 RID: 547
	[DebugQuery]
	[QueryAttributes(TagName = "route_test_db")]
	internal class RouteTestQueryDB : BaseQuery
	{
		// Token: 0x06000BCA RID: 3018 RVA: 0x0002CDB9 File Offset: 0x0002B1B9
		public RouteTestQueryDB(IDALService dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000BCB RID: 3019 RVA: 0x0002CDC8 File Offset: 0x0002B1C8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			this.m_dal.CommonSystem.GetTotalDataVersionStamp();
			return 0;
		}

		// Token: 0x0400057B RID: 1403
		private readonly IDALService m_dal;
	}
}
