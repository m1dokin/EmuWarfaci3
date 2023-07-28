using System;
using MasterServer.Core.Configs;
using Network.Amqp;

namespace MasterServer.Core.MessageBus
{
	// Token: 0x02000127 RID: 295
	[ConsoleCmdAttributes(CmdName = "get_amqp_prefetch_count", ArgsSize = 0, Help = "Returns AMQP prefetch count")]
	public class GetAmqpPrefetchCountCmd : IConsoleCmd
	{
		// Token: 0x060004DA RID: 1242 RVA: 0x00014FCB File Offset: 0x000133CB
		public GetAmqpPrefetchCountCmd(IConfigProvider<AmqpConfig> configProvider)
		{
			this.m_config = configProvider.Get();
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00014FDF File Offset: 0x000133DF
		public void ExecuteCmd(string[] args)
		{
			Log.Info<ushort>("AMQP prefetch count = {0}", this.m_config.PrefetchCount);
		}

		// Token: 0x04000208 RID: 520
		private readonly AmqpConfig m_config;
	}
}
