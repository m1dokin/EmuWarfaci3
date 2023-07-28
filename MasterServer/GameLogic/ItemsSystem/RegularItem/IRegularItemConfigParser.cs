using System;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.ItemsSystem.Regular;

namespace MasterServer.GameLogic.ItemsSystem.RegularItem
{
	// Token: 0x02000072 RID: 114
	[Contract]
	internal interface IRegularItemConfigParser
	{
		// Token: 0x060001BB RID: 443
		RegularItemConfig Parse(ConfigSection configSection);
	}
}
