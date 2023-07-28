using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.Users
{
	// Token: 0x02000763 RID: 1891
	[QueryAttributes(TagName = "class_presence")]
	internal class ClassPresenceQuery : BaseQuery
	{
		// Token: 0x06002744 RID: 10052 RVA: 0x000A5D19 File Offset: 0x000A4119
		public ClassPresenceQuery(ISessionStorage sessionStorage, IQueryManager queryManager, IOnlineClient onlineClient, IClassPresenceService classPresenceService)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_queryManager = queryManager;
			this.m_onlineClient = onlineClient;
			this.m_classPresenceService = classPresenceService;
		}

		// Token: 0x06002745 RID: 10053 RVA: 0x000A5D40 File Offset: 0x000A4140
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ClassPresenceQuery"))
			{
				string attribute = request.GetAttribute("session_id");
				if (!this.m_sessionStorage.ValidateSession(fromJid, attribute))
				{
					Log.Warning<string, string>("Ignoring class presence from server {0} which has incorrect session id {1}", fromJid, attribute);
					result = -1;
				}
				else
				{
					ClassPresenceData classPresenceData = new ClassPresenceData
					{
						sessionId = attribute
					};
					Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>();
					IEnumerator enumerator = request.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlNode xmlNode = (XmlNode)obj;
							if (xmlNode.NodeType == XmlNodeType.Element)
							{
								XmlElement xmlElement = xmlNode as XmlElement;
								if (xmlElement.Name == "profile")
								{
									ulong key = ulong.Parse(xmlElement.GetAttribute("id"));
									float num = float.Parse(xmlElement.GetAttribute("total_playtime"));
									List<ClassPresenceData.PresenceData> list = new List<ClassPresenceData.PresenceData>();
									dictionary[key] = (int)num;
									IEnumerator enumerator2 = xmlElement.ChildNodes.GetEnumerator();
									try
									{
										while (enumerator2.MoveNext())
										{
											object obj2 = enumerator2.Current;
											XmlNode xmlNode2 = (XmlNode)obj2;
											if (xmlNode2.NodeType == XmlNodeType.Element)
											{
												XmlElement xmlElement2 = xmlNode2 as XmlElement;
												if (xmlElement2.Name == "presence")
												{
													int cid = int.Parse(xmlElement2.GetAttribute("class_id"));
													int v = Math.Max(0, int.Parse(xmlElement2.GetAttribute("value")));
													ClassPresenceData.PresenceData item = new ClassPresenceData.PresenceData(cid, v);
													list.Add(item);
												}
											}
										}
									}
									finally
									{
										IDisposable disposable;
										if ((disposable = (enumerator2 as IDisposable)) != null)
										{
											disposable.Dispose();
										}
									}
									if (list.Count > 0)
									{
										classPresenceData.presence.Add(key, list);
									}
								}
							}
						}
					}
					finally
					{
						IDisposable disposable2;
						if ((disposable2 = (enumerator as IDisposable)) != null)
						{
							disposable2.Dispose();
						}
					}
					this.m_queryManager.Request("player_ingame_time", this.m_onlineClient.TargetRoute, new object[]
					{
						dictionary
					});
					this.m_classPresenceService.ClassPresenceRecieved(classPresenceData);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x0400143D RID: 5181
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x0400143E RID: 5182
		private readonly IQueryManager m_queryManager;

		// Token: 0x0400143F RID: 5183
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x04001440 RID: 5184
		private readonly IClassPresenceService m_classPresenceService;
	}
}
