using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.LobbyChat
{
	// Token: 0x0200039B RID: 923
	[Service]
	[Singleton]
	internal class ChatConferences : IChatConferences
	{
		// Token: 0x06001481 RID: 5249 RVA: 0x00053177 File Offset: 0x00051577
		public ChatConferences(IOnlineClient onlineClient, IGameRoomManager gameRoomManager, IClanService clanService)
		{
			this.m_onlineClient = onlineClient;
			this.m_gameRoomManager = gameRoomManager;
			this.m_clanService = clanService;
		}

		// Token: 0x06001482 RID: 5250 RVA: 0x00053194 File Offset: 0x00051594
		public ChatChannelID GenerateChannelId(EChatChannel channel, ulong profileID)
		{
			ChatConferences.<GenerateChannelId>c__AnonStorey1 <GenerateChannelId>c__AnonStorey = new ChatConferences.<GenerateChannelId>c__AnonStorey1();
			<GenerateChannelId>c__AnonStorey.profileID = profileID;
			<GenerateChannelId>c__AnonStorey.chatID = new ChatChannelID
			{
				ConferenceID = string.Format("conference.{0}", this.m_onlineClient.XmppHost)
			};
			switch (channel)
			{
			case EChatChannel.GLOBAL:
				<GenerateChannelId>c__AnonStorey.chatID.ChannelID = "global." + Resources.ServerName;
				break;
			case EChatChannel.ROOM:
			{
				IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(<GenerateChannelId>c__AnonStorey.profileID);
				if (room != null)
				{
					<GenerateChannelId>c__AnonStorey.chatID.ChannelID = string.Format("room.{0}.{1}", Resources.ServerName, room.ID);
				}
				break;
			}
			case EChatChannel.TEAM:
			{
				IGameRoom room = this.m_gameRoomManager.GetRoomByPlayer(<GenerateChannelId>c__AnonStorey.profileID);
				if (room != null)
				{
					try
					{
						room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
						{
							if (room.Autobalance)
							{
								throw new RoomGenericException(room.ID, "Can not open team channel for auto-balanced room");
							}
							RoomPlayer player = room.GetPlayer(<GenerateChannelId>c__AnonStorey.profileID);
							if (player != null)
							{
								<GenerateChannelId>c__AnonStorey.chatID.ChannelID = string.Format("team.{0}.{1}.{2}", Resources.ServerName, room.ID, player.TeamID);
							}
						});
					}
					catch (RoomGenericException)
					{
					}
				}
				break;
			}
			case EChatChannel.CLAN:
			{
				ClanMember memberInfo = this.m_clanService.GetMemberInfo(<GenerateChannelId>c__AnonStorey.profileID);
				if (memberInfo != null)
				{
					<GenerateChannelId>c__AnonStorey.chatID.ChannelID = string.Format("clan.{0}", memberInfo.ClanID);
				}
				break;
			}
			default:
				throw new ApplicationException(string.Format("Unsupported channel type {0}", channel));
			}
			return <GenerateChannelId>c__AnonStorey.chatID;
		}

		// Token: 0x0400099E RID: 2462
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x0400099F RID: 2463
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x040009A0 RID: 2464
		private readonly IClanService m_clanService;
	}
}
