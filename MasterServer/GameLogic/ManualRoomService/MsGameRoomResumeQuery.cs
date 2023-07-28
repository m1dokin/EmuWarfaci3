using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x020004A7 RID: 1191
	[QueryAttributes(TagName = "ms_gameroom_resume")]
	internal class MsGameRoomResumeQuery : ManualRoomQuery
	{
		// Token: 0x0600195B RID: 6491 RVA: 0x000670DA File Offset: 0x000654DA
		public MsGameRoomResumeQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x0600195C RID: 6492 RVA: 0x000670E4 File Offset: 0x000654E4
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			RoomReference roomReference = (RoomReference)args[0];
			request.SetAttribute("room_ref", roomReference.Reference);
		}

		// Token: 0x0600195D RID: 6493 RVA: 0x0006710C File Offset: 0x0006550C
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			return this.m_manualRoomService.ResumeSession(Resources.ServerName, roomRef);
		}

		// Token: 0x0600195E RID: 6494 RVA: 0x0006713B File Offset: 0x0006553B
		protected override string GetQueryName()
		{
			return "ms_gameroom_resume";
		}

		// Token: 0x04000C25 RID: 3109
		public const string QueryName = "ms_gameroom_resume";
	}
}
