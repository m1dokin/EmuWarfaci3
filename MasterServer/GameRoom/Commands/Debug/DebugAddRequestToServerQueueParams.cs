using System;
using CommandLine;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000102 RID: 258
	internal class DebugAddRequestToServerQueueParams
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x06000436 RID: 1078 RVA: 0x0001240D File Offset: 0x0001080D
		// (set) Token: 0x06000437 RID: 1079 RVA: 0x00012415 File Offset: 0x00010815
		[Option('c', "count", Required = true, HelpText = "count of request to add")]
		public ulong Count { get; set; }
	}
}
