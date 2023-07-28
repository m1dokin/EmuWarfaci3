using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000820 RID: 2080
	[QueryAttributes(TagName = "gameroom_setplayer")]
	internal class SetRoomPlayerQuery : BaseQuery
	{
		// Token: 0x06002AD3 RID: 10963 RVA: 0x000B9490 File Offset: 0x000B7890
		public SetRoomPlayerQuery(IGameRoomManager gameRoomManager, IMissionAccessLimitationService accessLimitation)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_accessLimitation = accessLimitation;
		}

		// Token: 0x06002AD4 RID: 10964 RVA: 0x000B94A8 File Offset: 0x000B78A8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetRoomPlayerQuery"))
			{
				ulong profileID;
				if (!base.GetClientProfileId(fromJid, out profileID))
				{
					result = -3;
				}
				else
				{
					int teamId = int.Parse(request.GetAttribute("team_id"));
					RoomPlayer.EStatus status = (RoomPlayer.EStatus)int.Parse(request.GetAttribute("status"));
					int classId = int.Parse(request.GetAttribute("class_id"));
					IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(profileID);
					if (room == null)
					{
						result = -1;
					}
					else
					{
						int queryResult = -1;
						XmlElement xmlElement = room.transaction(fromJid, response.OwnerDocument, AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							RoomPlayer player = r.GetPlayer(profileID);
							if (player == null)
							{
								return;
							}
							if (!string.IsNullOrEmpty(r.MissionDifficulty) && r.IsPveMode() && (!player.ProfileProgression.IsMissionTypeUnlocked(r.MissionType.Name) || !player.CanJoinMission(this.m_accessLimitation, room.MissionType.Name)))
							{
								status = RoomPlayer.EStatus.CantBeReady;
							}
							if (r.IsAutoStartMode() && (status != RoomPlayer.EStatus.Ready || player.TeamID != teamId))
							{
								Log.Warning<string>("[SetRoomPlayerQuery] Status and team id changes are restricted in AS rooms, jid {0}", fromJid);
								return;
							}
							if (player.RoomStatus == RoomPlayer.EStatus.CantBeReady && player.RoomStatus != status)
							{
								return;
							}
							if (r.IsClanWarMode() && player.TeamID != teamId)
							{
								return;
							}
							if (r.IsClanWarMode() && !player.IsInClan())
							{
								r.RemovePlayer(profileID, GameRoomPlayerRemoveReason.KickClan);
								return;
							}
							CustomParams state = r.GetState<CustomParams>(AccessMode.ReadOnly);
							if (!state.IsClassSuported(classId) || !player.ProfileProgression.IsClassUnlocked(classId))
							{
								return;
							}
							player.RoomStatus = status;
							player.ClassID = classId;
							if (player.TeamID != teamId)
							{
								r.SwitchTeam(profileID, teamId);
							}
							r.SignalPlayersChanged();
							queryResult = 0;
						});
						if (xmlElement != null)
						{
							response.AppendChild(xmlElement);
						}
						result = queryResult;
					}
				}
			}
			return result;
		}

		// Token: 0x040016D9 RID: 5849
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040016DA RID: 5850
		private readonly IMissionAccessLimitationService m_accessLimitation;
	}
}
