using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000634 RID: 1588
	[QueryAttributes(TagName = "gameroom_setprivatestatus")]
	internal class SetRoomPrivateStatusQuery : BaseQuery
	{
		// Token: 0x06002213 RID: 8723 RVA: 0x0008E798 File Offset: 0x0008CB98
		public SetRoomPrivateStatusQuery(IGameRoomManager gameRoomManager)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06002214 RID: 8724 RVA: 0x0008E7A8 File Offset: 0x0008CBA8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRoomPrivateStatusQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					bool priv = int.Parse(request.GetAttribute("private")) != 0;
					IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profile_id);
					if (roomByPlayer == null)
					{
						result = -1;
					}
					else if (roomByPlayer.IsAutoStartMode())
					{
						Log.Warning<string>("[SetRoomPrivateStatusQuery] Restricted query for AS room, jid {0}", fromJid);
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
							r.Private = priv;
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

		// Token: 0x040010C9 RID: 4297
		private readonly IGameRoomManager m_gameRoomManager;
	}
}
