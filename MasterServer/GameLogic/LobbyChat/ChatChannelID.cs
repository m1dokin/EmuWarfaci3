using System;

namespace MasterServer.GameLogic.LobbyChat
{
	// Token: 0x02000399 RID: 921
	public struct ChatChannelID
	{
		// Token: 0x0600147F RID: 5247 RVA: 0x00053157 File Offset: 0x00051557
		public bool IsEmpty()
		{
			return string.IsNullOrEmpty(this.ChannelID) || string.IsNullOrEmpty(this.ConferenceID);
		}

		// Token: 0x0400099C RID: 2460
		public string ChannelID;

		// Token: 0x0400099D RID: 2461
		public string ConferenceID;
	}
}
