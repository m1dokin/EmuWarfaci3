using System;
using System.Globalization;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200076D RID: 1901
	[QueryAttributes(TagName = "gameroom_join")]
	internal class JoinRoomQuery : BaseQuery
	{
		// Token: 0x06002779 RID: 10105 RVA: 0x000A7E2B File Offset: 0x000A622B
		public JoinRoomQuery(IGameRoomManager gameRoomManager, IRankSystem rankSystem, IDALService dalService)
		{
			this.m_gameRoomManager = gameRoomManager;
			this.m_rankSystem = rankSystem;
			this.m_dalService = dalService;
		}

		// Token: 0x0600277A RID: 10106 RVA: 0x000A7E48 File Offset: 0x000A6248
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "JoinRoomQuery"))
			{
				UserInfo.User user_info;
				if (!base.GetClientInfo(fromJid, out user_info))
				{
					result = -3;
				}
				else
				{
					ulong num = this.ParseRequestAttribute<ulong>(request, "room_id");
					int teamId = this.ParseRequestAttribute<int>(request, "team_id");
					string groupId = request.GetAttribute("group_id");
					int classId = this.ParseRequestAttribute<int>(request, "class_id");
					RoomPlayer.EStatus status = (RoomPlayer.EStatus)this.ParseRequestAttribute<int>(request, "status");
					GameRoomPlayerAddReason joinReason = (GameRoomPlayerAddReason)this.ParseRequestAttribute<int>(request, "join_reason");
					TimeSpan quickplayWaitTime = TimeSpan.FromSeconds((double)this.ParseRequestAttribute<float>(request, "wait_time_to_join"));
					response.SetAttribute("room_id", num.ToString(CultureInfo.InvariantCulture));
					SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(user_info.ProfileID);
					if (profileInfo.UserID != user_info.UserID)
					{
						Log.Warning<ulong>("Can't find ProfileInfo '{0}' for joining room.", user_info.ProfileID);
						result = -1;
					}
					else if (!this.m_rankSystem.CanJoinChannel(profileInfo.RankInfo.RankId))
					{
						Log.Warning<ulong, ulong, int>("Failed to join room '{0}' user '{1}' with incorrect rank {2}", num, profileInfo.UserID, profileInfo.RankInfo.RankId);
						response.SetAttribute("code", 11.ToString(CultureInfo.InvariantCulture));
						result = 0;
					}
					else
					{
						IGameRoom room = this.m_gameRoomManager.GetRoom(num);
						if (room == null)
						{
							response.SetAttribute("code", 10.ToString(CultureInfo.InvariantCulture));
							result = 0;
						}
						else
						{
							GameRoomRetCode code = GameRoomRetCode.CLOSED;
							try
							{
								room.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
								{
									ReservationsExtension extension = r.GetExtension<ReservationsExtension>();
									if ((!r.Private && !r.Locked) || extension.HasReservationFor(user_info.ProfileID))
									{
										code = r.AddPlayer(user_info.ProfileID, groupId, teamId, classId, quickplayWaitTime, status, joinReason);
									}
									if (code != GameRoomRetCode.OK)
									{
										XmlElement xmlElement = GetRoomsQuery.SerializeRoomDesc(r, response.OwnerDocument);
										if (xmlElement != null)
										{
											response.AppendChild(xmlElement);
										}
										else
										{
											code = GameRoomRetCode.CLOSED;
										}
									}
								});
							}
							catch (RoomClosedException)
							{
							}
							catch (RoomGenericException ex)
							{
								Log.Warning<string>("[JoinRoomQuery] Failed: {0}", ex.Message);
								return -1;
							}
							if (code == GameRoomRetCode.OK)
							{
								XmlElement newChild = room.FullStateSnapshot(RoomUpdate.Target.Client, response.OwnerDocument, user_info.ProfileID);
								response.AppendChild(newChild);
							}
							XmlElement response2 = response;
							string name = "code";
							int code2 = (int)code;
							response2.SetAttribute(name, code2.ToString(CultureInfo.InvariantCulture));
							result = 0;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x000A8120 File Offset: 0x000A6520
		private T ParseRequestAttribute<T>(XmlElement request, string name) where T : IConvertible
		{
			T result = default(T);
			string attribute = request.GetAttribute(name);
			if (string.IsNullOrEmpty(attribute))
			{
				throw new Exception(string.Format("There is no attribute '{0}' in join room request", name));
			}
			try
			{
				result = (T)((object)Convert.ChangeType(attribute, typeof(T)));
			}
			catch (Exception innerException)
			{
				throw new Exception(string.Format("Error while parsing request attribute {0}={1} in join room request", name, attribute), innerException);
			}
			return result;
		}

		// Token: 0x0400147A RID: 5242
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x0400147B RID: 5243
		private readonly IRankSystem m_rankSystem;

		// Token: 0x0400147C RID: 5244
		private readonly IDALService m_dalService;
	}
}
