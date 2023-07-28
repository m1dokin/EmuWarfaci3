using System;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F1 RID: 753
	internal class GameMode
	{
		// Token: 0x06001177 RID: 4471 RVA: 0x00045214 File Offset: 0x00043614
		public GameMode(string mode)
		{
			this.Mode = mode;
		}

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06001178 RID: 4472 RVA: 0x00045223 File Offset: 0x00043623
		// (set) Token: 0x06001179 RID: 4473 RVA: 0x0004522B File Offset: 0x0004362B
		public string Mode { get; private set; }

		// Token: 0x0600117A RID: 4474 RVA: 0x00045234 File Offset: 0x00043634
		public override bool Equals(object obj)
		{
			GameMode gameMode = obj as GameMode;
			return gameMode != null && gameMode.Mode.Equals(this.Mode);
		}

		// Token: 0x0600117B RID: 4475 RVA: 0x00045262 File Offset: 0x00043662
		public override int GetHashCode()
		{
			return this.Mode.GetHashCode();
		}

		// Token: 0x0600117C RID: 4476 RVA: 0x0004526F File Offset: 0x0004366F
		public override string ToString()
		{
			return this.Mode;
		}
	}
}
