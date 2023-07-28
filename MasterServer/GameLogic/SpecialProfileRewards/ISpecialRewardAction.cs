using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005C7 RID: 1479
	internal interface ISpecialRewardAction
	{
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001FA5 RID: 8101
		string PrizeName { get; }

		// Token: 0x06001FA6 RID: 8102
		SNotification Activate(ulong profileID, ILogGroup logGroup, XmlElement userData);

		// Token: 0x06001FA7 RID: 8103
		void Validate();
	}
}
