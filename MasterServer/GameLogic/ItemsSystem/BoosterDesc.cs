using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000367 RID: 871
	public class BoosterDesc
	{
		// Token: 0x06001378 RID: 4984 RVA: 0x0004F8A4 File Offset: 0x0004DCA4
		public BoosterDesc()
		{
			this.Params = new Dictionary<BoosterType, float>
			{
				{
					BoosterType.XPBooster,
					0f
				},
				{
					BoosterType.VPBooster,
					0f
				},
				{
					BoosterType.GMBooster,
					0f
				},
				{
					BoosterType.ICBooster,
					0f
				}
			};
			this.StackOption = BoosterStackOption.SumStackOption;
		}

		// Token: 0x06001379 RID: 4985 RVA: 0x0004F908 File Offset: 0x0004DD08
		public BoosterDesc(float xp, float vp, float gm, float ic, BoosterStackOption so)
		{
			this.Params[BoosterType.XPBooster] = xp;
			this.Params[BoosterType.VPBooster] = vp;
			this.Params[BoosterType.GMBooster] = gm;
			this.Params[BoosterType.ICBooster] = ic;
			this.StackOption = so;
		}

		// Token: 0x04000919 RID: 2329
		public BoosterStackOption StackOption;

		// Token: 0x0400091A RID: 2330
		public Dictionary<BoosterType, float> Params = new Dictionary<BoosterType, float>();
	}
}
