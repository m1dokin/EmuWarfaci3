using System;

namespace MasterServer.DAL
{
	// Token: 0x02000082 RID: 130
	[Serializable]
	public struct SPersistentSettings
	{
		// Token: 0x0600018A RID: 394 RVA: 0x00004EF6 File Offset: 0x000032F6
		public override string ToString()
		{
			return string.Format("Group: {0}, Settings: {1}", this.Group, this.Settings);
		}

		// Token: 0x04000156 RID: 342
		public ulong ProfileID;

		// Token: 0x04000157 RID: 343
		public string Group;

		// Token: 0x04000158 RID: 344
		public string Settings;
	}
}
