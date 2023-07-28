using System;
using System.Runtime.Serialization;

namespace MasterServer.DAL
{
	// Token: 0x0200006F RID: 111
	[Serializable]
	public class TransactionError : Exception
	{
		// Token: 0x0600011C RID: 284 RVA: 0x00004753 File Offset: 0x00002B53
		public TransactionError()
		{
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000475B File Offset: 0x00002B5B
		public TransactionError(string message) : base(message)
		{
		}

		// Token: 0x0600011E RID: 286 RVA: 0x00004764 File Offset: 0x00002B64
		public TransactionError(TransactionError.ErrorState state, Exception inner) : base((state != TransactionError.ErrorState.Aborted) ? "Transaction failed" : "Transaction aborted", inner)
		{
			this.State = state;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000478C File Offset: 0x00002B8C
		protected TransactionError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.State = (TransactionError.ErrorState)info.GetValue("state", this.State.GetType());
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000047CC File Offset: 0x00002BCC
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("state", this.State, this.State.GetType());
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000480B File Offset: 0x00002C0B
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00004813 File Offset: 0x00002C13
		public TransactionError.ErrorState State { get; private set; }

		// Token: 0x02000070 RID: 112
		public enum ErrorState
		{
			// Token: 0x04000125 RID: 293
			Aborted,
			// Token: 0x04000126 RID: 294
			Deadlocked,
			// Token: 0x04000127 RID: 295
			Other
		}
	}
}
