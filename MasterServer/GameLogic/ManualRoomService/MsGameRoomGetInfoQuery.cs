using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000005 RID: 5
	[QueryAttributes(TagName = "ms_gameroom_get_info")]
	internal class MsGameRoomGetInfoQuery : ManualRoomQuery
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00004592 File Offset: 0x00002992
		public MsGameRoomGetInfoQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000459C File Offset: 0x0000299C
		public override void SendRequest(string online_id, XmlElement request, params object[] args)
		{
			RoomReference roomReference = (RoomReference)args[0];
			request.SetAttribute("room_ref", roomReference.Reference);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000045C4 File Offset: 0x000029C4
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			return this.m_manualRoomService.GetRoomInfo(Resources.ServerName, roomRef);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000045F3 File Offset: 0x000029F3
		protected override string GetQueryName()
		{
			return "ms_gameroom_get_info";
		}

		// Token: 0x04000005 RID: 5
		public const string QueryName = "ms_gameroom_get_info";
	}
}
