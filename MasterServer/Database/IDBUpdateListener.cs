using System;

namespace MasterServer.Database
{
	// Token: 0x020001D5 RID: 469
	public interface IDBUpdateListener
	{
		// Token: 0x060008E0 RID: 2272
		bool OnDBUpdateStage(IDBUpdateService updater, DBUpdateStage stage);
	}
}
