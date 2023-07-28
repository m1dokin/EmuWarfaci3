using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x020006E7 RID: 1767
	[DebugQuery]
	[QueryAttributes(TagName = "debug_telemetry_stream")]
	internal class DebugTelemetryStreamQuery : BaseQuery
	{
		// Token: 0x0600251C RID: 9500 RVA: 0x0009ACAD File Offset: 0x000990AD
		public DebugTelemetryStreamQuery(ITelemetryService telemetryService, IDebugTelemetryStreamService telemetryStreamService, ISessionStorage sessionStorage)
		{
			this.m_telemetryService = telemetryService;
			this.m_telemetryStreamService = telemetryStreamService;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x0600251D RID: 9501 RVA: 0x0009ACCC File Offset: 0x000990CC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugTelemetryStreamQuery"))
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
						TelemetryStreamService.SessionData sessionData = new TelemetryStreamService.SessionData(this.m_telemetryService.StatsProcessor.GetRegistry(), attribute, string.Empty, string.Empty);
						List<DataUpdate> list = new List<DataUpdate>
						{
							this.GenerateSessionInfoUpdate()
						};
						IEnumerator enumerator = request.GetElementsByTagName("player").GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								object obj = enumerator.Current;
								XmlElement xmlElement = (XmlElement)obj;
								string attribute2 = xmlElement.GetAttribute("profile_id");
								string attribute3 = xmlElement.GetAttribute("team");
								string attribute4 = xmlElement.GetAttribute("kills");
								string attribute5 = xmlElement.GetAttribute("deaths");
								string attribute6 = xmlElement.GetAttribute("player_session_time");
								list.Add(this.GenerateSessionTimeUpdate(attribute2, attribute3, attribute6));
								list.Add(this.GenerateDataUpdate(attribute2, "player_kills_player", attribute4));
								list.Add(this.GenerateDataUpdate(attribute2, "player_deaths", attribute5));
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
						this.m_telemetryStreamService.SimulateTelemetryEvent(sessionData, list);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0600251E RID: 9502 RVA: 0x0009AEB4 File Offset: 0x000992B4
		private DataUpdate GenerateSessionInfoUpdate()
		{
			return new DataUpdate(new SortedList<string, string>
			{
				{
					"clanwar",
					"0"
				},
				{
					"difficulty",
					string.Empty
				},
				{
					"end_outcome",
					"aborted"
				},
				{
					"end_reason",
					"4"
				},
				{
					"end_winner",
					"1"
				},
				{
					"host",
					"test"
				},
				{
					"map_name",
					"@pvp_mission_display_name_tdm_aul"
				},
				{
					"map_path",
					"levels/pvp/tdm_aul"
				},
				{
					"map_uid",
					"bbb33b77-98db-463c-9a1a-d27655d90690"
				},
				{
					"masterserver",
					Resources.Jid
				},
				{
					"mode",
					"PVP"
				},
				{
					"stat",
					"_ss_info"
				},
				{
					"submode",
					"TDM"
				}
			}, "1");
		}

		// Token: 0x0600251F RID: 9503 RVA: 0x0009AFA4 File Offset: 0x000993A4
		private DataUpdate GenerateSessionTimeUpdate(string profileId, string team, string sessionTime)
		{
			return new DataUpdate(new SortedList<string, string>
			{
				{
					"class",
					"Rifleman"
				},
				{
					"nickname",
					"testbot"
				},
				{
					"profile",
					profileId
				},
				{
					"rank",
					"1"
				},
				{
					"stat",
					"_ss_player_playtime"
				},
				{
					"team",
					team
				}
			}, sessionTime);
		}

		// Token: 0x06002520 RID: 9504 RVA: 0x0009B018 File Offset: 0x00099418
		private DataUpdate GenerateDataUpdate(string profileId, string stat, string value)
		{
			return new DataUpdate(new SortedList<string, string>
			{
				{
					"clanwar",
					"0"
				},
				{
					"mode",
					"PVP"
				},
				{
					"profile",
					profileId
				},
				{
					"stat",
					stat
				},
				{
					"submode",
					"TDM"
				}
			}, value);
		}

		// Token: 0x040012BA RID: 4794
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x040012BB RID: 4795
		private readonly IDebugTelemetryStreamService m_telemetryStreamService;

		// Token: 0x040012BC RID: 4796
		private readonly ISessionStorage m_sessionStorage;
	}
}
