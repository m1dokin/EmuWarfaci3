using System;
using System.Collections;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.CustomRules.Rules;

namespace MasterServer.GameLogic.InGameEventSystem
{
	// Token: 0x02000309 RID: 777
	[QueryAttributes(TagName = "ingame_event", QoSClass = "ingame_event")]
	internal class InGameEventQuery : BaseQuery
	{
		// Token: 0x060011EF RID: 4591 RVA: 0x00046EC4 File Offset: 0x000452C4
		public InGameEventQuery(IInGameEventsService inGameEventsService)
		{
			this.m_inGameEventsService = inGameEventsService;
		}

		// Token: 0x060011F0 RID: 4592 RVA: 0x00046ED4 File Offset: 0x000452D4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "InGameEventQuery"))
			{
				string attribute = request.GetAttribute("event_name");
				string attribute2 = request.GetAttribute("session_id");
				ulong profileId = ulong.Parse(request.GetAttribute("profile_id"));
				InGameEventData inGameEventData = new InGameEventData();
				IEnumerator enumerator = request.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode xmlNode = (XmlNode)obj;
						if (!(xmlNode.Name != "data"))
						{
							XmlElement xmlElement = xmlNode as XmlElement;
							inGameEventData.Add(xmlElement.GetAttribute("key"), xmlElement.GetAttribute("value"));
						}
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
				try
				{
					this.m_inGameEventsService.FireInGameEvent(fromJid, attribute, attribute2, profileId, inGameEventData);
				}
				catch (InGameRewardRewardLimitReachedException)
				{
					return 1;
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x04000805 RID: 2053
		public const int REWARD_LIMIT_REACHED_FAIL = 1;

		// Token: 0x04000806 RID: 2054
		private readonly IInGameEventsService m_inGameEventsService;
	}
}
