using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000795 RID: 1941
	[QueryAttributes(TagName = "mission_load_failed")]
	internal class MissionLoadFailedQuery : BaseQuery
	{
		// Token: 0x06002845 RID: 10309 RVA: 0x000AD70C File Offset: 0x000ABB0C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			string serverId;
			if (!base.GetServerID(fromJid, out serverId))
			{
				return -3;
			}
			IGameRoomServer service = ServicesManager.GetService<IGameRoomServer>();
			service.FailedServer(serverId);
			return 0;
		}
	}
}
