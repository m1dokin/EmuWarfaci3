using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000819 RID: 2073
	[Contract]
	internal interface IGameRoomManager
	{
		// Token: 0x06002A84 RID: 10884
		IGameRoom GetRoom(string idString);

		// Token: 0x06002A85 RID: 10885
		IGameRoom GetRoom(ulong room_id);

		// Token: 0x06002A86 RID: 10886
		List<IGameRoom> GetRooms(Predicate<IGameRoom> pred);

		// Token: 0x06002A87 RID: 10887
		int TotalPlayersCount(Predicate<IGameRoom> pred);

		// Token: 0x06002A88 RID: 10888
		IGameRoom GetRoomByPlayer(ulong profile_id);

		// Token: 0x06002A89 RID: 10889
		IGameRoom GetRoomByServer(string server_id);

		// Token: 0x06002A8A RID: 10890
		IGameRoom GetRoomByRoomRef(RoomReference roomRef);

		// Token: 0x06002A8B RID: 10891
		void RemovePlayerByRoomRef(RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> players);

		// Token: 0x06002A8C RID: 10892
		void StartRoomByRoomRef(RoomReference roomRef, int team1Score, int team2Score);

		// Token: 0x06002A8D RID: 10893
		void CleanEmptyRooms();

		// Token: 0x06002A8E RID: 10894
		bool SetObserver(ulong profileId, bool enable);

		// Token: 0x140000B6 RID: 182
		// (add) Token: 0x06002A8F RID: 10895
		// (remove) Token: 0x06002A90 RID: 10896
		event OnRoomOpenedDeleg RoomOpened;

		// Token: 0x140000B7 RID: 183
		// (add) Token: 0x06002A91 RID: 10897
		// (remove) Token: 0x06002A92 RID: 10898
		event OnRoomClosedDeleg RoomClosed;

		// Token: 0x140000B8 RID: 184
		// (add) Token: 0x06002A93 RID: 10899
		// (remove) Token: 0x06002A94 RID: 10900
		event OnSessionStartedDeleg SessionStarted;

		// Token: 0x140000B9 RID: 185
		// (add) Token: 0x06002A95 RID: 10901
		// (remove) Token: 0x06002A96 RID: 10902
		event OnSessionEndedDeleg SessionEnded;
	}
}
