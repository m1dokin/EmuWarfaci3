using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000635 RID: 1589
	[QueryAttributes(TagName = "gameroom_switchteams")]
	internal class SwitchTeamsQuery : BaseQuery
	{
		// Token: 0x06002215 RID: 8725 RVA: 0x0008E8D0 File Offset: 0x0008CCD0
		public SwitchTeamsQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06002216 RID: 8726 RVA: 0x0008E8E0 File Offset: 0x0008CCE0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SwitchTeamsQuery"))
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
					else if (roomByPlayer.IsAutoStartMode())
					{
						Log.Warning<string>("[SwitchTeamsQuery] Restricted query for AS room, jid {0}", fromJid);
						result = -1;
					}
					else
					{
						XmlElement xmlElement = roomByPlayer.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							RoomMasterExtension extension = r.GetExtension<RoomMasterExtension>();
							if (!extension.IsRoomMaster(profile_id))
							{
								throw new ApplicationException("Only master can change room settings");
							}
							r.SwapTeams();
						});
						if (xmlElement != null)
						{
							response.AppendChild(xmlElement);
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x040010CA RID: 4298
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
