using System;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x02000434 RID: 1076
	public class Skill
	{
		// Token: 0x06001700 RID: 5888 RVA: 0x0005FE3D File Offset: 0x0005E23D
		public Skill(SkillType type, double value, double curveCoef = 0.0)
		{
			this.Type = type;
			this.Value = value;
			this.CurveCoef = curveCoef;
		}

		// Token: 0x1700021D RID: 541
		// (get) Token: 0x06001701 RID: 5889 RVA: 0x0005FE5A File Offset: 0x0005E25A
		// (set) Token: 0x06001702 RID: 5890 RVA: 0x0005FE62 File Offset: 0x0005E262
		public SkillType Type { get; private set; }

		// Token: 0x1700021E RID: 542
		// (get) Token: 0x06001703 RID: 5891 RVA: 0x0005FE6B File Offset: 0x0005E26B
		// (set) Token: 0x06001704 RID: 5892 RVA: 0x0005FE73 File Offset: 0x0005E273
		public double Value { get; private set; }

		// Token: 0x1700021F RID: 543
		// (get) Token: 0x06001705 RID: 5893 RVA: 0x0005FE7C File Offset: 0x0005E27C
		// (set) Token: 0x06001706 RID: 5894 RVA: 0x0005FE84 File Offset: 0x0005E284
		public double CurveCoef { get; private set; }

		// Token: 0x04000B19 RID: 2841
		public static readonly Skill Empty = new Skill(SkillType.None, 0.0, 0.0);
	}
}
