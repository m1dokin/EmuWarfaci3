using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.ManualRoomService
{
	// Token: 0x02000498 RID: 1176
	[Contract]
	internal interface IManualRoomService
	{
		// Token: 0x060018F8 RID: 6392
		string CreateRoom(string masterId, RoomReference roomRef, CreateRoomParam param);

		// Token: 0x060018F9 RID: 6393
		string GetRoomInfo(string masterId, RoomReference roomRef);

		// Token: 0x060018FA RID: 6394
		string AddPlayer(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> playersInfos);

		// Token: 0x060018FB RID: 6395
		string RemovePlayer(string masterId, RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> players);

		// Token: 0x060018FC RID: 6396
		string StartSession(string masterId, RoomReference roomRef, int team1Score, int team2Score);

		// Token: 0x060018FD RID: 6397
		string PauseSession(string masterId, RoomReference roomRef);

		// Token: 0x060018FE RID: 6398
		string ResumeSession(string masterId, RoomReference roomRef);

		// Token: 0x060018FF RID: 6399
		string StopSession(string masterId, RoomReference roomRef);
	}
}
