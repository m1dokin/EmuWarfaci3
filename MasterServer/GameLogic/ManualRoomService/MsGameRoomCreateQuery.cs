using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x0200052B RID: 1323
	[QueryAttributes(TagName = "ms_gameroom_create")]
	internal class MsGameRoomCreateQuery : ManualRoomQuery
	{
		// Token: 0x06001CBB RID: 7355 RVA: 0x000736DE File Offset: 0x00071ADE
		public MsGameRoomCreateQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x06001CBC RID: 7356 RVA: 0x000736E8 File Offset: 0x00071AE8
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			RoomReference roomReference = (RoomReference)queryParams[0];
			CreateRoomParam createRoomParam = (CreateRoomParam)queryParams[1];
			XmlElement xmlElement = request.OwnerDocument.CreateElement("room");
			request.AppendChild(xmlElement);
			xmlElement.SetAttribute("roomRef", roomReference.Reference);
			xmlElement.SetAttribute("mission_type", createRoomParam.Mission);
			xmlElement.SetAttribute("manual_start", (!createRoomParam.ManualStart) ? "0" : "1");
			xmlElement.SetAttribute("allow_manual_join", (!createRoomParam.AllowJoin) ? "0" : "1");
			xmlElement.SetAttribute("locked_spectator_camera", (!createRoomParam.LockedSpectatorCamera) ? "0" : "1");
			xmlElement.SetAttribute("prereound_limit", createRoomParam.PreRoundTime.ToString());
			xmlElement.SetAttribute("round_limit", createRoomParam.RoundLimit.ToString());
			xmlElement.SetAttribute("overtime_mode", (!createRoomParam.OvertimeMode) ? "0" : "1");
		}

		// Token: 0x06001CBD RID: 7357 RVA: 0x00073818 File Offset: 0x00071C18
		protected override string ActivateRequest(XmlElement request)
		{
			XmlNode xmlNode = request.SelectSingleNode("room");
			RoomReference roomRef = new RoomReference(xmlNode.Attributes["roomRef"].Value);
			CreateRoomParam param = new CreateRoomParam
			{
				Mission = xmlNode.Attributes["mission_type"].Value,
				ManualStart = (int.Parse(xmlNode.Attributes["manual_start"].Value) > 0),
				AllowJoin = (int.Parse(xmlNode.Attributes["allow_manual_join"].Value) > 0),
				LockedSpectatorCamera = (int.Parse(xmlNode.Attributes["locked_spectator_camera"].Value) > 0),
				RoundLimit = int.Parse(xmlNode.Attributes["round_limit"].Value),
				PreRoundTime = int.Parse(xmlNode.Attributes["prereound_limit"].Value),
				OvertimeMode = (int.Parse(xmlNode.Attributes["overtime_mode"].Value) > 0)
			};
			return this.m_manualRoomService.CreateRoom(Resources.ServerName, roomRef, param);
		}

		// Token: 0x06001CBE RID: 7358 RVA: 0x00073958 File Offset: 0x00071D58
		protected override string GetQueryName()
		{
			return "ms_gameroom_create";
		}

		// Token: 0x04000DB2 RID: 3506
		public const string QueryName = "ms_gameroom_create";
	}
}
