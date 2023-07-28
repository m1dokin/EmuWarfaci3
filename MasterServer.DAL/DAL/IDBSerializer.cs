using System;

namespace MasterServer.DAL
{
	// Token: 0x02000068 RID: 104
	public interface IDBSerializer<T>
	{
		// Token: 0x060000FC RID: 252
		void Deserialize(IDataReaderEx reader, out T ret);
	}
}
