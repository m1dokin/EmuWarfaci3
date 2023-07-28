using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x0200021B RID: 539
	[DebugQuery]
	[QueryAttributes(TagName = "debug_session_close")]
	internal class DebugSessionClose : BaseQuery
	{
		// Token: 0x06000BB5 RID: 2997 RVA: 0x0002C488 File Offset: 0x0002A888
		public DebugSessionClose(ISessionStorage sessionStorage, IGameRoomManager gameRoomManager)
		{
			this.m_sessionStorage = sessionStorage;
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06000BB6 RID: 2998 RVA: 0x0002C4A0 File Offset: 0x0002A8A0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "DebugSessionClose"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					base.ServerRepository.RemoveServer(user.OnlineID);
					string sessionId = string.Empty;
					IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(user.ProfileID);
					if (roomByPlayer != null)
					{
						roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							sessionId = r.SessionID;
							r.RemoveAllPlayers();
						});
						this.m_sessionStorage.RemoveAllData(sessionId);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000571 RID: 1393
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000572 RID: 1394
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
