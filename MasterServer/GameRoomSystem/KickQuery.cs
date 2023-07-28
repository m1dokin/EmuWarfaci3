using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200076E RID: 1902
	[QueryAttributes(TagName = "gameroom_kick")]
	internal class KickQuery : BaseQuery
	{
		// Token: 0x0600277C RID: 10108 RVA: 0x000A826E File Offset: 0x000A666E
		public KickQuery(IGameRoomManager gameRoomManager, IPunishmentService punishmentService)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x000A8284 File Offset: 0x000A6684
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "KickQuery"))
			{
				ulong profile_id;
				if (!base.GetClientProfileId(fromJid, out profile_id))
				{
					result = -3;
				}
				else
				{
					ulong targetId = ulong.Parse(request.GetAttribute("target_id"));
					IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(profile_id);
					if (room == null)
					{
						result = -1;
					}
					else
					{
						try
						{
							room.transaction(fromJid, response.OwnerDocument, AccessMode.ReadOnly, delegate(IGameRoom r)
							{
								Log.Info<ulong, ulong, ulong>("Player {0} is kicking player '{1}' from room '{2}'", profile_id, targetId, r.ID);
								RoomMasterExtension extension = room.GetExtension<RoomMasterExtension>();
								if (!extension.IsRoomMaster(profile_id))
								{
									throw new RoomGenericException(room.ID, "Only room master can kick players");
								}
								if (extension.IsRoomMaster(targetId))
								{
									throw new RoomGenericException(room.ID, "Can't kick room master");
								}
								if (r.GetPlayer(targetId, AccessMode.ReadOnly) == null)
								{
									throw new RoomGenericException(room.ID, "No such player");
								}
								SessionExtension extension2 = r.GetExtension<SessionExtension>();
								if (extension2.Started)
								{
									throw new RoomGenericException(room.ID, "Can't kick players from in-game");
								}
							});
							this.m_punishmentService.KickPlayerLocal(targetId, GameRoomPlayerRemoveReason.KickMaster);
							XmlElement newChild = room.FullStateSnapshot(RoomUpdate.Target.Client, response.OwnerDocument);
							response.AppendChild(newChild);
						}
						catch (RoomGenericException ex)
						{
							Log.Warning<string>("[KickQuery] Failed: {0}", ex.Message);
							return -1;
						}
						catch (RoomUnsupportedExtensionException)
						{
							Log.Warning<string>("[KickQuery] Restricted query for rooms which don't support RoomMasterExtension, jid {0}", fromJid);
							throw;
						}
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x0400147D RID: 5245
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x0400147E RID: 5246
		private IPunishmentService m_punishmentService;
	}
}
