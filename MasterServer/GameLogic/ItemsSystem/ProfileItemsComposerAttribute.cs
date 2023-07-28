using System;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000377 RID: 887
	public class ProfileItemsComposerAttribute : Attribute
	{
		// Token: 0x06001428 RID: 5160 RVA: 0x00051FFD File Offset: 0x000503FD
		public ProfileItemsComposerAttribute()
		{
			this.Priority = 50;
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06001429 RID: 5161 RVA: 0x0005200D File Offset: 0x0005040D
		// (set) Token: 0x0600142A RID: 5162 RVA: 0x00052015 File Offset: 0x00050415
		public int Priority { get; set; }

		// Token: 0x04000952 RID: 2386
		public const int DEFAULT_PRIORITY = 50;
	}
}
