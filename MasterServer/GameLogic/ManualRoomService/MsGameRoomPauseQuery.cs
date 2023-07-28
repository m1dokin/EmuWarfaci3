using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x020004A6 RID: 1190
	[QueryAttributes(TagName = "ms_gameroom_pause")]
	internal class MsGameRoomPauseQuery : ManualRoomQuery
	{
		// Token: 0x06001957 RID: 6487 RVA: 0x00067073 File Offset: 0x00065473
		public MsGameRoomPauseQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x06001958 RID: 6488 RVA: 0x0006707C File Offset: 0x0006547C
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			RoomReference roomReference = (RoomReference)args[0];
			request.SetAttribute("room_ref", roomReference.Reference);
		}

		// Token: 0x06001959 RID: 6489 RVA: 0x000670A4 File Offset: 0x000654A4
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			return this.m_manualRoomService.PauseSession(Resources.ServerName, roomRef);
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x000670D3 File Offset: 0x000654D3
		protected override string GetQueryName()
		{
			return "ms_gameroom_pause";
		}

		// Token: 0x04000C24 RID: 3108
		public const string QueryName = "ms_gameroom_pause";
	}
}
