using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000523 RID: 1315
	[QueryAttributes(TagName = "ms_gameroom_start")]
	internal class MsGameRoomStartQuery : ManualRoomQuery
	{
		// Token: 0x06001C9C RID: 7324 RVA: 0x00072A46 File Offset: 0x00070E46
		public MsGameRoomStartQuery(IManualRoomService manualRoomService) : base(manualRoomService)
		{
		}

		// Token: 0x06001C9D RID: 7325 RVA: 0x00072A50 File Offset: 0x00070E50
		public override void SendRequest(string onlineId, XmlElement request, params object[] args)
		{
			RoomReference roomReference = (RoomReference)args[0];
			int num = int.Parse(args[1].ToString());
			int num2 = int.Parse(args[2].ToString());
			request.SetAttribute("room_ref", roomReference.Reference);
			request.SetAttribute("team1_score", num.ToString());
			request.SetAttribute("team2_score", num2.ToString());
		}

		// Token: 0x06001C9E RID: 7326 RVA: 0x00072AC4 File Offset: 0x00070EC4
		protected override string ActivateRequest(XmlElement request)
		{
			RoomReference roomRef = new RoomReference(request.GetAttribute("room_ref"));
			int team1Score = int.Parse(request.GetAttribute("team1_score"));
			int team2Score = int.Parse(request.GetAttribute("team2_score"));
			return this.m_manualRoomService.StartSession(Resources.ServerName, roomRef, team1Score, team2Score);
		}

		// Token: 0x06001C9F RID: 7327 RVA: 0x00072B17 File Offset: 0x00070F17
		protected override string GetQueryName()
		{
			return "ms_gameroom_start";
		}

		// Token: 0x04000D99 RID: 3481
		public const string QueryName = "ms_gameroom_start";
	}
}
