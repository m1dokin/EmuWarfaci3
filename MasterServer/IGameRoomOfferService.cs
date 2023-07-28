using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

// Token: 0x02000527 RID: 1319
[Contract]
internal interface IGameRoomOfferService
{
	// Token: 0x06001CA4 RID: 7332
	bool OfferRoom(string fromNickname, UserInfo.User toUser, string token, IGameRoom room);

	// Token: 0x06001CA5 RID: 7333
	bool OfferRoom(string fromNickname, IEnumerable<UserInfo.User> toUsers, string token, IGameRoom room);

	// Token: 0x06001CA6 RID: 7334
	void OnResponse(Guid token, bool accepted);

	// Token: 0x06001CA7 RID: 7335
	void OfferRoomByRoomRef(RoomReference roomRef, IEnumerable<PlayerInfoForRoomOffer> players);

	// Token: 0x06001CA8 RID: 7336
	void OfferRoomForReconnect(IGameRoom room, ulong profileId);
}
