using System;

namespace MasterServer.Core
{
	// Token: 0x02000799 RID: 1945
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	internal class ConsoleCmdAttributes : Attribute
	{
		// Token: 0x04001525 RID: 5413
		public string CmdName;

		// Token: 0x04001526 RID: 5414
		[Obsolete]
		public int ArgsSize;

		// Token: 0x04001527 RID: 5415
		public string Help;
	}
}
