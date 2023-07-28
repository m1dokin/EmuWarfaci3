using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x020004A8 RID: 1192
	[QueryAttributes(TagName = "ms_gameroom_stop")]
	internal class MsGameRoomStopQuery : ManualRoomQuery
	{
		// Token: 0x0600195F RID: 6495 RVA: 0x00067142 File Offset: 0x00065542
		public MsGameRoomStopQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x06001960 RID: 6496 RVA: 0x0006714C File Offset: 0x0006554C
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			RoomReference roomReference = (RoomReference)args[0];
			request.SetAttribute("room_ref", roomReference.Reference);
		}

		// Token: 0x06001961 RID: 6497 RVA: 0x00067174 File Offset: 0x00065574
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			return this.m_manualRoomService.StopSession(Resources.ServerName, roomRef);
		}

		// Token: 0x06001962 RID: 6498 RVA: 0x000671A3 File Offset: 0x000655A3
		protected override string GetQueryName()
		{
			return "ms_gameroom_stop";
		}

		// Token: 0x04000C26 RID: 3110
		public const string QueryName = "ms_gameroom_stop";
	}
}
