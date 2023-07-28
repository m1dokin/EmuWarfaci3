using System;

namespace MasterServer.GameLogic.ItemsSystem.RandomBoxChoiceLimitation
{
	// Token: 0x02000089 RID: 137
	internal class RandomBoxChoiceLimitationConfig
	{
		// Token: 0x06000209 RID: 521 RVA: 0x0000BBE0 File Offset: 0x00009FE0
		internal RandomBoxChoiceLimitationConfig(bool enabled, uint maxAmount)
		{
			this.Enabled = enabled;
			this.MaxAmount = maxAmount;
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000BBF6 File Offset: 0x00009FF6
		// (set) Token: 0x0600020B RID: 523 RVA: 0x0000BBFE File Offset: 0x00009FFE
		public bool Enabled { get; private set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0000BC07 File Offset: 0x0000A007
		// (set) Token: 0x0600020D RID: 525 RVA: 0x0000BC0F File Offset: 0x0000A00F
		public uint MaxAmount { get; private set; }
	}
}
