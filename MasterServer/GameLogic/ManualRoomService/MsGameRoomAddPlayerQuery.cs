using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x020004B5 RID: 1205
	[QueryAttributes(TagName = "ms_gameroom_add_player")]
	internal class MsGameRoomAddPlayerQuery : ManualRoomQuery
	{
		// Token: 0x060019B2 RID: 6578 RVA: 0x00068AD0 File Offset: 0x00066ED0
		public MsGameRoomAddPlayerQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x00068ADC File Offset: 0x00066EDC
		public override void SendRequest(string online_id, XmlElement request, params object[] args)
		{
			XmlDocument ownerDocument = request.OwnerDocument;
			RoomReference roomReference = (RoomReference)args[0];
			request.SetAttribute("room_ref", roomReference.Reference);
			List<PlayerInfoForRoomOffer> list = (List<PlayerInfoForRoomOffer>)args[1];
			foreach (PlayerInfoForRoomOffer playerInfoForRoomOffer in list)
			{
				XmlElement xmlElement = ownerDocument.CreateElement("player");
				xmlElement.SetAttribute("nickname", playerInfoForRoomOffer.Nickname);
				xmlElement.SetAttribute("profile_id", playerInfoForRoomOffer.ProfileId.ToString(CultureInfo.InvariantCulture));
				xmlElement.SetAttribute("group_id", playerInfoForRoomOffer.GroupId.ToString(CultureInfo.InvariantCulture));
				request.AppendChild(xmlElement);
			}
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x00068BBC File Offset: 0x00066FBC
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			XmlNodeList players = request.SelectNodes("player");
			IEnumerable<PlayerInfoForRoomOffer> playersInfos = this.ParseXml(players);
			return this.m_manualRoomService.AddPlayer(Resources.ServerName, roomRef, playersInfos);
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x00068C00 File Offset: 0x00067000
		protected override string GetQueryName()
		{
			return "ms_gameroom_add_player";
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x00068C08 File Offset: 0x00067008
		private IEnumerable<PlayerInfoForRoomOffer> ParseXml(XmlNodeList players)
		{
			List<PlayerInfoForRoomOffer> list = new List<PlayerInfoForRoomOffer>();
			for (int i = 0; i < players.Count; i++)
			{
				list.Add(new PlayerInfoForRoomOffer
				{
					ProfileId = ulong.Parse(players[i].Attributes["profile_id"].Value),
					Nickname = players[i].Attributes["nickname"].Value,
					GroupId = players[i].Attributes["group_id"].Value
				});
			}
			return list;
		}

		// Token: 0x04000C4D RID: 3149
		public const string QueryName = "ms_gameroom_add_player";
	}
}
