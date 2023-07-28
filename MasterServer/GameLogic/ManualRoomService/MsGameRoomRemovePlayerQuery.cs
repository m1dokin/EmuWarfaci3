using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000522 RID: 1314
	[QueryAttributes(TagName = "ms_gameroom_remove_player")]
	internal class MsGameRoomRemovePlayerQuery : ManualRoomQuery
	{
		// Token: 0x06001C97 RID: 7319 RVA: 0x0007288B File Offset: 0x00070C8B
		public MsGameRoomRemovePlayerQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x06001C98 RID: 7320 RVA: 0x00072894 File Offset: 0x00070C94
		public override void SendRequest(string online_id, XmlElement request, params object[] args)
		{
			XmlDocument ownerDocument = request.OwnerDocument;
			RoomReference roomReference = (RoomReference)args[0];
			request.SetAttribute("room_ref", roomReference.Reference);
			List<PlayerInfoForRoomOffer> list = (List<PlayerInfoForRoomOffer>)args[1];
			foreach (PlayerInfoForRoomOffer playerInfoForRoomOffer in list)
			{
				XmlElement xmlElement = ownerDocument.CreateElement("player");
				if (playerInfoForRoomOffer.IsNicknameUsed)
				{
					xmlElement.SetAttribute("nickname", playerInfoForRoomOffer.Nickname);
				}
				else
				{
					xmlElement.SetAttribute("profile_id", playerInfoForRoomOffer.ProfileId.ToString(CultureInfo.InvariantCulture));
				}
				request.AppendChild(xmlElement);
			}
		}

		// Token: 0x06001C99 RID: 7321 RVA: 0x00072968 File Offset: 0x00070D68
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			XmlNodeList players = request.SelectNodes("player");
			IEnumerable<PlayerInfoForRoomOffer> players2 = this.ParseXml(players);
			return this.m_manualRoomService.RemovePlayer(Resources.ServerName, roomRef, players2);
		}

		// Token: 0x06001C9A RID: 7322 RVA: 0x000729AC File Offset: 0x00070DAC
		protected override string GetQueryName()
		{
			return "ms_gameroom_remove_player";
		}

		// Token: 0x06001C9B RID: 7323 RVA: 0x000729B4 File Offset: 0x00070DB4
		private IEnumerable<PlayerInfoForRoomOffer> ParseXml(XmlNodeList players)
		{
			List<PlayerInfoForRoomOffer> list = new List<PlayerInfoForRoomOffer>();
			for (int i = 0; i < players.Count; i++)
			{
				PlayerInfoForRoomOffer item = default(PlayerInfoForRoomOffer);
				XmlAttributeCollection attributes = players[i].Attributes;
				if (attributes[0].Name == "profile_id")
				{
					item.ProfileId = ulong.Parse(attributes[0].Value);
				}
				else
				{
					item.Nickname = attributes[0].Value;
				}
				list.Add(item);
			}
			return list;
		}

		// Token: 0x04000D98 RID: 3480
		public const string QueryName = "ms_gameroom_remove_player";
	}
}
