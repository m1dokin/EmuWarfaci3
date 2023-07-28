using System;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000290 RID: 656
	internal class ContractDesc
	{
		// Token: 0x06000E27 RID: 3623 RVA: 0x00038D18 File Offset: 0x00037118
		public ContractDesc(uint id, ulong itemId, string name, uint totalProgress, EContractType type, IContractReward reward)
		{
			this.Id = id;
			this.ItemId = itemId;
			this.Name = name;
			this.TotalProgress = totalProgress;
			this.Type = type;
			this.Reward = reward;
			this.IsActive = true;
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000E28 RID: 3624 RVA: 0x00038D54 File Offset: 0x00037154
		// (set) Token: 0x06000E29 RID: 3625 RVA: 0x00038D5C File Offset: 0x0003715C
		public uint Id { get; private set; }

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000E2A RID: 3626 RVA: 0x00038D65 File Offset: 0x00037165
		// (set) Token: 0x06000E2B RID: 3627 RVA: 0x00038D6D File Offset: 0x0003716D
		public ulong ItemId { get; private set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000E2C RID: 3628 RVA: 0x00038D76 File Offset: 0x00037176
		// (set) Token: 0x06000E2D RID: 3629 RVA: 0x00038D7E File Offset: 0x0003717E
		public string Name { get; private set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000E2E RID: 3630 RVA: 0x00038D87 File Offset: 0x00037187
		// (set) Token: 0x06000E2F RID: 3631 RVA: 0x00038D8F File Offset: 0x0003718F
		public uint TotalProgress { get; private set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000E30 RID: 3632 RVA: 0x00038D98 File Offset: 0x00037198
		// (set) Token: 0x06000E31 RID: 3633 RVA: 0x00038DA0 File Offset: 0x000371A0
		public bool IsActive { get; set; }

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000E32 RID: 3634 RVA: 0x00038DA9 File Offset: 0x000371A9
		// (set) Token: 0x06000E33 RID: 3635 RVA: 0x00038DB1 File Offset: 0x000371B1
		public EContractType Type { get; private set; }

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000E34 RID: 3636 RVA: 0x00038DBA File Offset: 0x000371BA
		// (set) Token: 0x06000E35 RID: 3637 RVA: 0x00038DC2 File Offset: 0x000371C2
		public IContractReward Reward { get; private set; }
	}
}
