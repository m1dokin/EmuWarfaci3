using System;
using MasterServer.Core.Services.Regions;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x020004F5 RID: 1269
	[Serializable]
	internal class MMRegionDistanceDTO
	{
		// Token: 0x06001B4A RID: 6986 RVA: 0x0006F142 File Offset: 0x0006D542
		public MMRegionDistanceDTO(RegionsDistance distance)
		{
			this.From = distance.From;
			this.To = distance.To;
			this.Value = distance.Value;
		}

		// Token: 0x170002C6 RID: 710
		// (get) Token: 0x06001B4B RID: 6987 RVA: 0x0006F16E File Offset: 0x0006D56E
		// (set) Token: 0x06001B4C RID: 6988 RVA: 0x0006F176 File Offset: 0x0006D576
		public string From { get; private set; }

		// Token: 0x170002C7 RID: 711
		// (get) Token: 0x06001B4D RID: 6989 RVA: 0x0006F17F File Offset: 0x0006D57F
		// (set) Token: 0x06001B4E RID: 6990 RVA: 0x0006F187 File Offset: 0x0006D587
		public string To { get; private set; }

		// Token: 0x170002C8 RID: 712
		// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0006F190 File Offset: 0x0006D590
		// (set) Token: 0x06001B50 RID: 6992 RVA: 0x0006F198 File Offset: 0x0006D598
		public uint Value { get; private set; }
	}
}
