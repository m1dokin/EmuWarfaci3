using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000222 RID: 546
	[DebugQuery]
	[QueryAttributes(TagName = "route_test", QoSClass = "generic_telemetry")]
	internal class RouteTestQuery : BaseQuery
	{
		// Token: 0x06000BC9 RID: 3017 RVA: 0x0002CD14 File Offset: 0x0002B114
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			Random random = new Random();
			int maxValue = 50;
			int val = 0;
			if (request.HasAttribute("max_delay_msec"))
			{
				maxValue = int.Parse(request.GetAttribute("max_delay_msec"));
			}
			if (request.HasAttribute("min_delay_msec"))
			{
				val = int.Parse(request.GetAttribute("min_delay_msec"));
			}
			int num = Math.Max(val, random.Next(maxValue));
			Log.Info<int>("route_test sleep for {0} msec", num);
			DateTime utcNow = DateTime.UtcNow;
			DateTime utcNow2;
			do
			{
				utcNow2 = DateTime.UtcNow;
			}
			while ((utcNow2 - utcNow).TotalMilliseconds <= (double)num);
			return 0;
		}
	}
}
