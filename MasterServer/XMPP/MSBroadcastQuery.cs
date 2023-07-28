using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.XMPP
{
	// Token: 0x0200080E RID: 2062
	[QueryAttributes(TagName = "master_server_bcast")]
	internal class MSBroadcastQuery : BaseQuery
	{
		// Token: 0x06002A3F RID: 10815 RVA: 0x000B6409 File Offset: 0x000B4809
		public MSBroadcastQuery(IClientVersionsManagementService clientVersionsManagementService, IAnnouncementService announcementService, IPunishmentService punishmentService)
		{
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_announcementService = announcementService;
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x06002A40 RID: 10816 RVA: 0x000B6428 File Offset: 0x000B4828
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			string value = (string)queryParams[0];
			request.SetAttribute("type", value);
			request.SetAttribute("sender", Resources.Jid);
			XmlElement xmlElement = request.OwnerDocument.CreateElement("params");
			int i = 1;
			int num = 1;
			while (i < queryParams.Length)
			{
				if (queryParams[i].ToString() == "no_self_send")
				{
					request.SetAttribute("bcast_no_self_send", "1");
				}
				else
				{
					xmlElement.SetAttribute(string.Format("param{0}", num), queryParams[i].ToString());
					num++;
				}
				i++;
			}
			request.AppendChild(xmlElement);
		}

		// Token: 0x06002A41 RID: 10817 RVA: 0x000B64D8 File Offset: 0x000B48D8
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			string attribute = request.GetAttribute("type");
			string attribute2 = request.GetAttribute("sender");
			XmlNodeList elementsByTagName = request.GetElementsByTagName("params");
			if (attribute != null)
			{
				if (MSBroadcastQuery.<>f__switch$map3 == null)
				{
					MSBroadcastQuery.<>f__switch$map3 = new Dictionary<string, int>(8)
					{
						{
							"set_global",
							0
						},
						{
							"set_user_tags_global",
							0
						},
						{
							"clear_user_tags_global",
							0
						},
						{
							"global_command",
							0
						},
						{
							"kick",
							1
						},
						{
							"announcement",
							2
						},
						{
							"remote_screenshot",
							3
						},
						{
							"reload_client_versions",
							4
						}
					};
				}
				int num;
				if (MSBroadcastQuery.<>f__switch$map3.TryGetValue(attribute, out num))
				{
					switch (num)
					{
					case 0:
						MSBroadcastQuery.ProccessGlobalCommand(elementsByTagName.Item(0));
						break;
					case 1:
						this.ProccessKickCommand(elementsByTagName.Item(0));
						break;
					case 2:
						this.ProccessAnnouncement(attribute2, elementsByTagName.Item(0));
						break;
					case 3:
						this.ProccessRemoteScreenshot(attribute2, elementsByTagName.Item(0));
						break;
					case 4:
						this.ProcessClientVersionsReloadCommand();
						break;
					case 5:
						goto IL_12B;
					default:
						goto IL_12B;
					}
					return 0;
				}
			}
			IL_12B:
			throw new ApplicationException(string.Format("Unknow type {0}", attribute));
		}

		// Token: 0x06002A42 RID: 10818 RVA: 0x000B6624 File Offset: 0x000B4A24
		private void ProccessAnnouncement(string sender, XmlNode element)
		{
			if (sender != Resources.Jid)
			{
				ulong deleteId = ulong.Parse(element.Attributes["param1"].Value);
				this.m_announcementService.UpdateCache(deleteId);
			}
		}

		// Token: 0x06002A43 RID: 10819 RVA: 0x000B6668 File Offset: 0x000B4A68
		private void ProccessKickCommand(XmlNode element)
		{
			string value = element.Attributes["param1"].Value;
			string[] array = value.Split(new char[]
			{
				' '
			});
			ulong profileId = ulong.Parse(array[1]);
			this.m_punishmentService.KickPlayerLocal(profileId, GameRoomPlayerRemoveReason.KickAdmin);
		}

		// Token: 0x06002A44 RID: 10820 RVA: 0x000B66B4 File Offset: 0x000B4AB4
		private static void ProccessGlobalCommand(XmlNode element)
		{
			string value = element.Attributes["param1"].Value;
			ConsoleCmdManager.ExecuteCmd(value);
		}

		// Token: 0x06002A45 RID: 10821 RVA: 0x000B66E0 File Offset: 0x000B4AE0
		private void ProccessRemoteScreenshot(string initiator, XmlNode element)
		{
			ulong profileId = ulong.Parse(element.Attributes["param1"].Value);
			bool frontBuffer = bool.Parse(element.Attributes["param2"].Value);
			int count = int.Parse(element.Attributes["param3"].Value);
			float scaleWidth = float.Parse(element.Attributes["param4"].Value);
			float scaleHeight = float.Parse(element.Attributes["param5"].Value);
			long screenShotKey = long.Parse(element.Attributes["param6"].Value);
			this.m_punishmentService.MakeScreenShot(profileId, frontBuffer, count, scaleWidth, scaleHeight, screenShotKey, initiator);
		}

		// Token: 0x06002A46 RID: 10822 RVA: 0x000B67A6 File Offset: 0x000B4BA6
		private void ProcessClientVersionsReloadCommand()
		{
			this.m_clientVersionsManagementService.SyncClientVersions();
		}

		// Token: 0x04001687 RID: 5767
		public const string QueryName = "master_server_bcast";

		// Token: 0x04001688 RID: 5768
		public const string SetGlobalRequest = "set_global";

		// Token: 0x04001689 RID: 5769
		public const string SetUserTagsRequest = "set_user_tags_global";

		// Token: 0x0400168A RID: 5770
		public const string ClearUserRequest = "clear_user_tags_global";

		// Token: 0x0400168B RID: 5771
		public const string GlobalCommandRequest = "global_command";

		// Token: 0x0400168C RID: 5772
		public const string KickRequest = "kick";

		// Token: 0x0400168D RID: 5773
		public const string AnouncementRequest = "announcement";

		// Token: 0x0400168E RID: 5774
		public const string RemoteScreensotRequest = "remote_screenshot";

		// Token: 0x0400168F RID: 5775
		public const string ReloadClientVersionsRequest = "reload_client_versions";

		// Token: 0x04001690 RID: 5776
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x04001691 RID: 5777
		private readonly IAnnouncementService m_announcementService;

		// Token: 0x04001692 RID: 5778
		private readonly IPunishmentService m_punishmentService;
	}
}
