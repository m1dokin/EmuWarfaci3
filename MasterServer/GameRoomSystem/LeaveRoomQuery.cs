using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200081E RID: 2078
	[QueryAttributes(TagName = "gameroom_leave")]
	internal class LeaveRoomQuery : BaseQuery
	{
		// Token: 0x06002ACF RID: 10959 RVA: 0x000B8F1C File Offset: 0x000B731C
		public LeaveRoomQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06002AD0 RID: 10960 RVA: 0x000B8F2C File Offset: 0x000B732C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "LeaveRoomQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profile_id);
					if (roomByPlayer == null)
					{
						result = -1;
					}
					else
					{
						roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.RemovePlayer(profile_id, GameRoomPlayerRemoveReason.Left);
						});
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x040016D3 RID: 5843
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
