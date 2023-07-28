using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000632 RID: 1586
	[QueryAttributes(TagName = "gameroom_update_pvp")]
	internal class UpdatePvpRoomQuery : BaseQuery
	{
		// Token: 0x0600220F RID: 8719 RVA: 0x0008E3A3 File Offset: 0x0008C7A3
		public UpdatePvpRoomQuery(IGameRoomManager gameRoomManager, IMissionSystem missionSystem)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x06002210 RID: 8720 RVA: 0x0008E3BC File Offset: 0x0008C7BC
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRoomCustomParamsQuery"))
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
						Log.Warning<string>("[UpdatePvpRoomQuery] Restricted query for AS room, jid {0}", fromJid);
						result = -1;
					}
					else
					{
						bool priv = int.Parse(request.GetAttribute("private")) != 0;
						bool flag = int.Parse(request.GetAttribute("by_mission_key")) != 0;
						string missionKey = request.GetAttribute("mission_key");
						string attribute = request.GetAttribute("data");
						if (!flag)
						{
							if (!Resources.DebugQueriesEnabled)
							{
								Log.Error<string>("Set room mission data is disabled, jid {0}", fromJid);
								return -1;
							}
							this.m_missionSystem.AddMission(missionKey, attribute);
						}
						GameRoomRetCode ret = GameRoomRetCode.OK;
						XmlElement xmlElement = roomByPlayer.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							RoomMasterExtension extension = r.GetExtension<RoomMasterExtension>();
							if (!extension.IsRoomMaster(profile_id))
							{
								throw new ApplicationException("Only master can change pvp room settings");
							}
							MissionExtension extension2 = r.GetExtension<MissionExtension>();
							if (extension2.MissionKey != missionKey)
							{
								ret = extension2.SetMission(missionKey);
							}
							if (ret != GameRoomRetCode.OK)
							{
								return;
							}
							r.Private = priv;
							CustomParamsExtension extension3 = r.GetExtension<CustomParamsExtension>();
							extension3.CheckAndSetRestrictions(request);
						});
						if (xmlElement != null)
						{
							response.AppendChild(xmlElement);
						}
						result = (int)ret;
					}
				}
			}
			return result;
		}

		// Token: 0x040010C7 RID: 4295
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040010C8 RID: 4296
		private readonly IMissionSystem m_missionSystem;
	}
}
