using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.Matchmaking.Queries
{
	// Token: 0x020005F0 RID: 1520
	[QueryAttributes(TagName = "gameroom_quickplay_cancel")]
	internal class QuickPlayCancelQuery : BaseQuery
	{
		// Token: 0x0600204C RID: 8268 RVA: 0x00082BFC File Offset: 0x00080FFC
		public QuickPlayCancelQuery(IMatchmakingSystem matchmakingSystem)
		{
			this.m_matchmakingSystem = matchmakingSystem;
		}

		// Token: 0x0600204D RID: 8269 RVA: 0x00082C0C File Offset: 0x0008100C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "QuickPlayCancelQuery"))
			{
				ulong initiatorProfileId;
				if (!base.GetClientProfileId(fromJid, out initiatorProfileId))
				{
					result = -3;
				}
				else
				{
					this.m_matchmakingSystem.UnQueueEntity(initiatorProfileId, EUnQueueReason.Cancelled);
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000FD0 RID: 4048
		private readonly IMatchmakingSystem m_matchmakingSystem;
	}
}
