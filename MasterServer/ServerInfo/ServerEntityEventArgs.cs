using System;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C6 RID: 1734
	public class ServerEntityEventArgs : EventArgs
	{
		// Token: 0x06002468 RID: 9320 RVA: 0x00097AAD File Offset: 0x00095EAD
		public ServerEntityEventArgs()
		{
			this.ServerId = string.Empty;
			this.Entity = new ServerEntity();
		}

		// Token: 0x1700038C RID: 908
		// (get) Token: 0x06002469 RID: 9321 RVA: 0x00097ACB File Offset: 0x00095ECB
		// (set) Token: 0x0600246A RID: 9322 RVA: 0x00097AD3 File Offset: 0x00095ED3
		public string ServerId { get; set; }

		// Token: 0x1700038D RID: 909
		// (get) Token: 0x0600246B RID: 9323 RVA: 0x00097ADC File Offset: 0x00095EDC
		// (set) Token: 0x0600246C RID: 9324 RVA: 0x00097AE4 File Offset: 0x00095EE4
		public ServerEntityState State { get; set; }

		// Token: 0x1700038E RID: 910
		// (get) Token: 0x0600246D RID: 9325 RVA: 0x00097AED File Offset: 0x00095EED
		// (set) Token: 0x0600246E RID: 9326 RVA: 0x00097AF5 File Offset: 0x00095EF5
		public ServerEntity Entity { get; set; }
	}
}
