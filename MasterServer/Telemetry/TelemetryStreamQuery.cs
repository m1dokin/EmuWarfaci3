using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D7 RID: 2007
	[QueryAttributes(TagName = "telemetry_stream", QoSClass = "telemetry_stream", CompressionType = ECompressType.eCS_SmartCompress)]
	internal class TelemetryStreamQuery : BaseQuery
	{
		// Token: 0x0600290D RID: 10509 RVA: 0x000B1EAA File Offset: 0x000B02AA
		public TelemetryStreamQuery(ITelemetryService telemetryService, ITelemetryStreamService telemetryStreamService, ISessionStorage sessionStorage)
		{
			this.m_telemetryService = telemetryService;
			this.m_telemetryStreamService = telemetryStreamService;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x0600290E RID: 10510 RVA: 0x000B1EC8 File Offset: 0x000B02C8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "TelemetryStreamQuery"))
			{
				string text;
				if (!this.m_telemetryService.CheckMode(TelemetryMode.Session))
				{
					result = 0;
				}
				else if (!base.GetServerID(fromJid, out text))
				{
					Log.Warning<string>("Ignoring telemetry stream from unregistered server {0}", fromJid);
					result = -1;
				}
				else
				{
					string attribute = request.GetAttribute("session_id");
					if (!this.m_sessionStorage.ValidateSession(fromJid, attribute))
					{
						Log.Warning<string, string>("Ignoring telemetry stream from server {0} which has incorrect session id {1}", fromJid, attribute);
						result = -1;
					}
					else
					{
						TelemetryStreamService.StreamPacket packet = new TelemetryStreamService.StreamPacket
						{
							SessionID = attribute,
							PacketID = int.Parse(request.GetAttribute("packet_id")),
							IsFinal = (request.GetAttribute("finalize") == "1"),
							Content = request.InnerText
						};
						result = ((!this.m_telemetryStreamService.Process(packet)) ? -1 : 0);
					}
				}
			}
			return result;
		}

		// Token: 0x040015E2 RID: 5602
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x040015E3 RID: 5603
		private readonly ITelemetryStreamService m_telemetryStreamService;

		// Token: 0x040015E4 RID: 5604
		private readonly ISessionStorage m_sessionStorage;
	}
}
