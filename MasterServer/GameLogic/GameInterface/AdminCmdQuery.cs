using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002D7 RID: 727
	[QueryAttributes(TagName = "admin_cmd")]
	internal class AdminCmdQuery : BaseQuery
	{
		// Token: 0x06000F8B RID: 3979 RVA: 0x0003EBD0 File Offset: 0x0003CFD0
		public AdminCmdQuery(IGameInterface gameInterface, ILogService logService, IGameRoomManager gameRoomManager, IPunishmentService punishmentService)
		{
			this.m_gameInterface = gameInterface;
			this.m_logService = logService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x06000F8C RID: 3980 RVA: 0x0003EBF8 File Offset: 0x0003CFF8
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			string attribute = request.GetAttribute("command");
			string attribute2 = request.GetAttribute("args");
			string value = "ERROR";
			string text = string.Empty;
			Log.Info<string, string, string>("Executing admin command '{0} {1}' sent by {2}", attribute, attribute2, fromJid);
			Dictionary<string, Func<UserInfo.User, string, string>> dictionary = new Dictionary<string, Func<UserInfo.User, string, string>>
			{
				{
					"ban",
					new Func<UserInfo.User, string, string>(this.BanPlayer)
				},
				{
					"unban",
					new Func<UserInfo.User, string, string>(this.CancelBanPlayer)
				},
				{
					"mute",
					new Func<UserInfo.User, string, string>(this.MutePlayer)
				},
				{
					"unmute",
					new Func<UserInfo.User, string, string>(this.CancelMutePlayer)
				},
				{
					"kick",
					new Func<UserInfo.User, string, string>(this.KickPlayer)
				},
				{
					"observer",
					new Func<UserInfo.User, string, string>(this.SetObserver)
				},
				{
					"start",
					new Func<UserInfo.User, string, string>(this.StartRoom)
				},
				{
					"stop",
					(UserInfo.User userInfo, string args) => this.StopSession(userInfo)
				},
				{
					"pause",
					(UserInfo.User userInfo, string args) => this.PauseSession(userInfo)
				},
				{
					"resume",
					(UserInfo.User userInfo, string args) => this.ResumeSession(userInfo)
				}
			};
			try
			{
				Func<UserInfo.User, string, string> func;
				text = ((!dictionary.TryGetValue(attribute, out func)) ? string.Format("{0} {1}", attribute, attribute2) : func(user, attribute2));
				value = GameInterfaceCmd.Execute(user.AccessLvl, text);
			}
			catch (ProfileNotFoundException)
			{
				return -1;
			}
			catch (RoomNotFoundException)
			{
				return -1;
			}
			catch (AccessLevelException)
			{
				this.ForceLogout(fromJid, user);
			}
			catch (Exception ex)
			{
				if (ex.InnerException is AccessLevelException)
				{
					this.ForceLogout(fromJid, user);
				}
				Log.Error(ex);
			}
			this.m_logService.Event.AdminCommandLog(user.UserID, user.IP, text);
			response.SetAttribute("command", attribute);
			response.SetAttribute("result", value);
			return 0;
		}

		// Token: 0x06000F8D RID: 3981 RVA: 0x0003EE38 File Offset: 0x0003D238
		private string BanPlayer(UserInfo.User userInfo, string sargs)
		{
			return this.CreatePlayerRelatedCommand("gi_ban_player", userInfo, sargs);
		}

		// Token: 0x06000F8E RID: 3982 RVA: 0x0003EE47 File Offset: 0x0003D247
		private string CancelBanPlayer(UserInfo.User userInfo, string sargs)
		{
			return this.CreatePlayerRelatedCommand("gi_cancel_ban_player", userInfo, sargs);
		}

		// Token: 0x06000F8F RID: 3983 RVA: 0x0003EE56 File Offset: 0x0003D256
		private string MutePlayer(UserInfo.User userInfo, string sargs)
		{
			return this.CreatePlayerRelatedCommand("gi_mute_player", userInfo, sargs);
		}

		// Token: 0x06000F90 RID: 3984 RVA: 0x0003EE65 File Offset: 0x0003D265
		private string CancelMutePlayer(UserInfo.User userInfo, string sargs)
		{
			return this.CreatePlayerRelatedCommand("gi_cancel_mute_player", userInfo, sargs);
		}

		// Token: 0x06000F91 RID: 3985 RVA: 0x0003EE74 File Offset: 0x0003D274
		private string KickPlayer(UserInfo.User userInfo, string sargs)
		{
			return this.CreatePlayerRelatedCommand("gi_kick_player", userInfo, sargs);
		}

		// Token: 0x06000F92 RID: 3986 RVA: 0x0003EE83 File Offset: 0x0003D283
		private string SetObserver(UserInfo.User userInfo, string sargs)
		{
			return this.CreatePlayerRelatedCommand("gi_set_observer", userInfo, sargs);
		}

		// Token: 0x06000F93 RID: 3987 RVA: 0x0003EE94 File Offset: 0x0003D294
		private string StartRoom(UserInfo.User userInfo, string sargs)
		{
			IGameRoom gameRoom = this.GetGameRoom(userInfo);
			return string.Format("gi_start_room {0} {1} {2}", Resources.ServerName, gameRoom.Reference.Reference, sargs);
		}

		// Token: 0x06000F94 RID: 3988 RVA: 0x0003EEC4 File Offset: 0x0003D2C4
		private string StopSession(UserInfo.User userInfo)
		{
			IGameRoom gameRoom = this.GetGameRoom(userInfo);
			return string.Format("gi_stop_session {0} {1}", Resources.ServerName, gameRoom.Reference.Reference);
		}

		// Token: 0x06000F95 RID: 3989 RVA: 0x0003EEF4 File Offset: 0x0003D2F4
		private string PauseSession(UserInfo.User userInfo)
		{
			IGameRoom gameRoom = this.GetGameRoom(userInfo);
			return string.Format("gi_pause_session {0} {1}", Resources.ServerName, gameRoom.Reference.Reference);
		}

		// Token: 0x06000F96 RID: 3990 RVA: 0x0003EF24 File Offset: 0x0003D324
		private string ResumeSession(UserInfo.User userInfo)
		{
			IGameRoom gameRoom = this.GetGameRoom(userInfo);
			return string.Format("gi_resume_session {0} {1}", Resources.ServerName, gameRoom.Reference.Reference);
		}

		// Token: 0x06000F97 RID: 3991 RVA: 0x0003EF54 File Offset: 0x0003D354
		private string CreatePlayerRelatedCommand(string alias, UserInfo.User userInfo, string sargs)
		{
			IGameInterfaceContext ctx = this.m_gameInterface.CreateContext(userInfo.AccessLvl);
			string[] array = sargs.Split(new char[]
			{
				' '
			});
			ulong num = GameInterfaceCmd.gi_get_profile_id_by_nick(ctx, array);
			if (num == 0UL)
			{
				throw new ProfileNotFoundException(array[0]);
			}
			return string.Format("{0} {1} {2}", alias, num, sargs.Remove(0, array[0].Length));
		}

		// Token: 0x06000F98 RID: 3992 RVA: 0x0003EFC0 File Offset: 0x0003D3C0
		private IGameRoom GetGameRoom(UserInfo.User userInfo)
		{
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(userInfo.ProfileID);
			if (roomByPlayer == null)
			{
				throw new RoomNotFoundException(string.Format("Room for player {0} not found", userInfo.ProfileID));
			}
			return roomByPlayer;
		}

		// Token: 0x06000F99 RID: 3993 RVA: 0x0003F001 File Offset: 0x0003D401
		private void ForceLogout(string fromJid, UserInfo.User userInfo)
		{
			Log.Error<string, AccessLevel>("User {0} tried to execute command of inaccessible level {1}", fromJid, userInfo.AccessLvl);
			this.m_punishmentService.ForceLogout(userInfo.ProfileID);
		}

		// Token: 0x0400074B RID: 1867
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x0400074C RID: 1868
		private readonly IPunishmentService m_punishmentService;

		// Token: 0x0400074D RID: 1869
		private readonly IGameInterface m_gameInterface;

		// Token: 0x0400074E RID: 1870
		private readonly ILogService m_logService;
	}
}
