using System;
using System.Collections.Generic;
using System.Text;
using NCrontab;

namespace MasterServer.GameLogic.FirstWinOfDayByModeSystem.Configs.GameModeFirstWinOfDayBonusConfig
{
	// Token: 0x02000085 RID: 133
	public class GameModeFirstWinOfDayBonusConfig
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001ED RID: 493 RVA: 0x0000B622 File Offset: 0x00009A22
		// (set) Token: 0x060001EE RID: 494 RVA: 0x0000B62A File Offset: 0x00009A2A
		public bool Enabled { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000B633 File Offset: 0x00009A33
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000B63B File Offset: 0x00009A3B
		public CrontabSchedule ResetSchedule { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000B644 File Offset: 0x00009A44
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000B64C File Offset: 0x00009A4C
		public IDictionary<string, GameModeBonus> ModesBonus { get; set; }

		// Token: 0x060001F3 RID: 499 RVA: 0x0000B658 File Offset: 0x00009A58
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Enabled = {0}", this.Enabled));
			stringBuilder.AppendLine(string.Format("ResetSchedule = {0}", this.ResetSchedule));
			foreach (KeyValuePair<string, GameModeBonus> keyValuePair in this.ModesBonus)
			{
				stringBuilder.AppendLine(string.Format("Mode = {0}, Bonus = {1}", keyValuePair.Key, keyValuePair.Value.Bonus));
			}
			return stringBuilder.ToString();
		}
	}
}
