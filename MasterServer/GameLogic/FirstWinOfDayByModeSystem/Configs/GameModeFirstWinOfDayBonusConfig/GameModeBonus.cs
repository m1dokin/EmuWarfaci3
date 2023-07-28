using System;
using System.Runtime.InteropServices;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig
{
	// Token: 0x02000076 RID: 118
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct GameModeBonus
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001C8 RID: 456 RVA: 0x0000B323 File Offset: 0x00009723
		// (set) Token: 0x060001C9 RID: 457 RVA: 0x0000B32B File Offset: 0x0000972B
		public uint Bonus { get; set; }

		// Token: 0x060001CA RID: 458 RVA: 0x0000B334 File Offset: 0x00009734
		public override string ToString()
		{
			return string.Format("Bonus = {0}", this.Bonus);
		}
	}
}
