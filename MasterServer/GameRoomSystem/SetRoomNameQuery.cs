using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000633 RID: 1587
	[QueryAttributes(TagName = "gameroom_setname")]
	internal class SetRoomNameQuery : BaseQuery
	{
		// Token: 0x06002212 RID: 8722 RVA: 0x0008E5F8 File Offset: 0x0008C9F8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRoomNameQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					ulong profile_id = user.ProfileID;
					string roomName = request.GetAttribute("room_name");
					IProfanityCheckService service = ServicesManager.GetService<IProfanityCheckService>();
					ProfanityCheckResult profanityCheckResult = service.CheckRoomName(user.UserID, user.Nickname, roomName);
					if (profanityCheckResult != ProfanityCheckResult.Reserved)
					{
						if (profanityCheckResult != ProfanityCheckResult.Failed)
						{
							IGameRoomManager service2 = ServicesManager.GetService<IGameRoomManager>();
							IGameRoom roomByPlayer = service2.GetRoomByPlayer(profile_id);
							if (roomByPlayer == null)
							{
								result = -1;
							}
							else if (roomByPlayer.IsAutoStartMode())
							{
								Log.Warning<string>("[SetRoomNameQuery] Restricted query for AS room, jid {0}", fromJid);
								result = -1;
							}
							else
							{
								XmlElement xmlElement = roomByPlayer.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
								{
									RoomMasterExtension extension = r.GetExtension<RoomMasterExtension>();
									if (!extension.IsRoomMaster(profile_id))
									{
										throw new ApplicationException("Only master can change room name");
									}
									r.RoomName = roomName;
								});
								if (xmlElement != null)
								{
									response.AppendChild(xmlElement);
								}
								result = 0;
							}
						}
						else
						{
							Log.Error<ulong>("Failed to set room name by user {0}: room name invalid", profile_id);
							result = 21;
						}
					}
					else
					{
						Log.Error<ulong>("Failed to set room name by user {0}: room name reserved", profile_id);
						result = 22;
					}
				}
			}
			return result;
		}
	}
}
