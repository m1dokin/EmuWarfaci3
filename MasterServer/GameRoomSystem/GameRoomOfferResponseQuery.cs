using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000526 RID: 1318
	[QueryAttributes(TagName = "gameroom_offer_response")]
	internal class GameRoomOfferResponseQuery : BaseQuery
	{
		// Token: 0x06001CA2 RID: 7330 RVA: 0x00072C01 File Offset: 0x00071001
		public GameRoomOfferResponseQuery(IGameRoomOfferService gameRoomOfferService)
		{
			this.m_gameRoomOfferService = gameRoomOfferService;
		}

		// Token: 0x06001CA3 RID: 7331 RVA: 0x00072C10 File Offset: 0x00071010
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GameRoomOfferResponseQuery"))
			{
				string attribute = request.GetAttribute("id");
				Guid token = new Guid(attribute);
				bool accepted = request.GetAttribute("result") == "1";
				this.m_gameRoomOfferService.OnResponse(token, accepted);
				result = 0;
			}
			return result;
		}

		// Token: 0x04000D9F RID: 3487
		private readonly IGameRoomOfferService m_gameRoomOfferService;
	}
}
