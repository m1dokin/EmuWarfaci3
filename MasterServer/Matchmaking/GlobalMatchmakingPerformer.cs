using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Timers;
using MasterServer.Matchmaking.Data;
using MasterServer.Matchmaking.MessagesBuilders;
using Network.Amqp;
using Network.Amqp.Interfaces;
using Network.Amqp.RabbitMq;
using Network.Interfaces;

namespace MasterServer.Matchmaking
{
	// Token: 0x020004FA RID: 1274
	[Service]
	[Singleton]
	internal class GlobalMatchmakingPerformer : ServiceModule, IMatchmakingPerformer
	{
		// Token: 0x06001B79 RID: 7033 RVA: 0x0006F47C File Offset: 0x0006D87C
		public GlobalMatchmakingPerformer(ITimerFactory timerFactory, IMatchmakingSystem matchmakingSystem, ITypeNameSerializer typeNameSerializer, IAdvancedBus bus, IMatchmakingConfigProvider matchmakingConfigProvider, IEnumerable<IMessageBuilder> messageBuilders)
		{
			typeNameSerializer.AddMappingFor<GlobalMatchmakingPerformer.GlobalMatchmakingReply>();
			this.m_matchmakingSystem = matchmakingSystem;
			this.m_timerFactory = timerFactory;
			this.m_bus = bus;
			this.m_matchmakingConfigProvider = matchmakingConfigProvider;
			this.m_messageBuilders = from mb in messageBuilders
			where mb.IsQueueAvailableByChannelType
			select mb;
			this.m_bus.Connected += this.OnBusConnected;
		}

		// Token: 0x06001B7A RID: 7034 RVA: 0x0006F4F4 File Offset: 0x0006D8F4
		private void OnBusConnected()
		{
			foreach (IMessageBuilder messageBuilder in this.m_messageBuilders)
			{
				this.m_bus.QueueDeclare(messageBuilder.QueueName);
			}
			this.m_receivingQueue = this.m_bus.QueueDeclare(Resources.MMQueuesNames.ReceivingRoomUpdatesQueueName);
			MatchmakingConfig matchmakingConfig = this.m_matchmakingConfigProvider.Get();
			this.m_timer = this.m_timerFactory.CreateTimer(new TimerCallback(this.OnTick), null, matchmakingConfig.QueueInterval, matchmakingConfig.QueueInterval);
		}

		// Token: 0x14000069 RID: 105
		// (add) Token: 0x06001B7B RID: 7035 RVA: 0x0006F5A8 File Offset: 0x0006D9A8
		// (remove) Token: 0x06001B7C RID: 7036 RVA: 0x0006F5E0 File Offset: 0x0006D9E0
		public event MatchmakingPerformerStatsDeleg OnMatchmakingPerformerStats;

		// Token: 0x06001B7D RID: 7037 RVA: 0x0006F616 File Offset: 0x0006DA16
		public override void Start()
		{
			base.Start();
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
			this.m_matchmakingConfigProvider.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06001B7E RID: 7038 RVA: 0x0006F648 File Offset: 0x0006DA48
		public override void Stop()
		{
			ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
			this.m_matchmakingConfigProvider.OnConfigChanged -= this.OnConfigChanged;
			if (this.m_consumer != null)
			{
				this.m_consumer.Dispose();
				this.m_consumer = null;
			}
			if (this.m_bus != null)
			{
				this.m_bus.Dispose();
			}
			if (this.m_timer != null)
			{
				this.m_timer.Dispose();
				this.m_timer = null;
			}
			base.Stop();
		}

		// Token: 0x06001B7F RID: 7039 RVA: 0x0006F6D4 File Offset: 0x0006DAD4
		private void OnTick(object obj)
		{
			foreach (IMessageBuilder messageBuilder in this.m_messageBuilders)
			{
				string queueName = messageBuilder.QueueName;
				IMessage<GlobalMatchmakingData> message = messageBuilder.BuildMessage();
				this.m_bus.Publish<GlobalMatchmakingData>(Exchange.Default, queueName, false, false, message);
				Log.Verbose(Log.Group.Matchmaking, "MM message sent into {0}. {1} entities sent", new object[]
				{
					queueName,
					message.Body.Entities.Count<MMEntityDTO>()
				});
			}
		}

		// Token: 0x06001B80 RID: 7040 RVA: 0x0006F77C File Offset: 0x0006DB7C
		private void OnExecutionPhaseChanged(ExecutionPhase phase)
		{
			if (phase == ExecutionPhase.Started)
			{
				this.m_consumer = this.m_bus.Consume<GlobalMatchmakingPerformer.GlobalMatchmakingReply>(this.m_receivingQueue, delegate(IMessage<GlobalMatchmakingPerformer.GlobalMatchmakingReply> res, MessageReceivedInfo info)
				{
					this.OnUpdateReceived(res.Body);
				});
			}
		}

		// Token: 0x06001B81 RID: 7041 RVA: 0x0006F7A8 File Offset: 0x0006DBA8
		private void OnUpdateReceived(GlobalMatchmakingPerformer.GlobalMatchmakingReply result)
		{
			Log.Verbose(Log.Group.Matchmaking, "MM message received, {0} entities to be placed", new object[]
			{
				result.MatchmakingResult.SuccededEntities.Count<MMResultEntity>()
			});
			this.m_matchmakingSystem.OnMatchmakingResult(result.MatchmakingResult);
		}

		// Token: 0x06001B82 RID: 7042 RVA: 0x0006F7E8 File Offset: 0x0006DBE8
		private void OnConfigChanged(MatchmakingConfig matchmakingConfig)
		{
			this.m_timer.Change(matchmakingConfig.QueueInterval, matchmakingConfig.QueueInterval);
		}

		// Token: 0x04000D26 RID: 3366
		public const string Type = "global";

		// Token: 0x04000D27 RID: 3367
		private readonly IAdvancedBus m_bus;

		// Token: 0x04000D28 RID: 3368
		private readonly ITimerFactory m_timerFactory;

		// Token: 0x04000D29 RID: 3369
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x04000D2A RID: 3370
		private readonly IMatchmakingConfigProvider m_matchmakingConfigProvider;

		// Token: 0x04000D2B RID: 3371
		private readonly IEnumerable<IMessageBuilder> m_messageBuilders;

		// Token: 0x04000D2C RID: 3372
		private IQueue m_receivingQueue;

		// Token: 0x04000D2D RID: 3373
		private IDisposable m_consumer;

		// Token: 0x04000D2E RID: 3374
		private ITimer m_timer;

		// Token: 0x020004FB RID: 1275
		internal class GlobalMatchmakingReply
		{
			// Token: 0x170002D8 RID: 728
			// (get) Token: 0x06001B86 RID: 7046 RVA: 0x0006F821 File Offset: 0x0006DC21
			// (set) Token: 0x06001B87 RID: 7047 RVA: 0x0006F829 File Offset: 0x0006DC29
			public MatchmakingResult MatchmakingResult { get; set; }
		}
	}
}
