using System;
using System.Xml;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002AA RID: 682
	public interface ICustomRule : IDisposable
	{
		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000E92 RID: 3730
		ulong RuleID { get; }

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000E93 RID: 3731
		XmlElement Config { get; }

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000E94 RID: 3732
		bool Enabled { get; }

		// Token: 0x06000E95 RID: 3733
		bool IsActive();

		// Token: 0x06000E96 RID: 3734
		void Activate();
	}
}
