using System;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.ItemsSystem.Regular;
using MasterServer.GameLogic.ItemsSystem.RegularItem.Exceptions;

namespace MasterServer.GameLogic.ItemsSystem.RegularItem
{
	// Token: 0x02000073 RID: 115
	[Service]
	[Singleton]
	internal class RegularItemConfigParser : IRegularItemConfigParser
	{
		// Token: 0x060001BD RID: 445 RVA: 0x0000B15C File Offset: 0x0000955C
		public RegularItemConfig Parse(ConfigSection configSection)
		{
			RegularItemConfig regularItemConfig = new RegularItemConfig();
			uint num;
			if (!configSection.TryGet("max_amount", out num, 0U))
			{
				throw new RegularItemConfigException("max_amount", num);
			}
			regularItemConfig.MaxAmount = num;
			bool flag;
			if (!configSection.TryGet("stacking_enabled", out flag, false))
			{
				throw new RegularItemConfigException("stacking_enabled", flag);
			}
			regularItemConfig.StackingEnabled = Convert.ToBoolean(flag);
			return regularItemConfig;
		}

		// Token: 0x040000CD RID: 205
		public const string MaxAmountParam = "max_amount";

		// Token: 0x040000CE RID: 206
		public const string StackingEnabledParam = "stacking_enabled";
	}
}
