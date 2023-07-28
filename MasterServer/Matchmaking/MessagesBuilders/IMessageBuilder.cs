using System;
using HK2Net;
using MasterServer.Matchmaking.Data;
using Network.Amqp.Interfaces;

namespace MasterServer.Matchmaking.MessagesBuilders
{
	// Token: 0x02000509 RID: 1289
	[Contract]
	internal interface IMessageBuilder
	{
		// Token: 0x170002E5 RID: 741
		// (get) Token: 0x06001BE3 RID: 7139
		string QueueName { get; }

		// Token: 0x170002E6 RID: 742
		// (get) Token: 0x06001BE4 RID: 7140
		bool IsQueueAvailableByChannelType { get; }

		// Token: 0x06001BE5 RID: 7141
		IMessage<GlobalMatchmakingData> BuildMessage();
	}
}
