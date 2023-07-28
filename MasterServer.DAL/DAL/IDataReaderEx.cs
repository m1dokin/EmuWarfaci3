using System;
using System.Data;

namespace MasterServer.DAL
{
	// Token: 0x0200009E RID: 158
	public interface IDataReaderEx : IDataReader, IDisposable, IDataRecord
	{
		// Token: 0x06000204 RID: 516
		bool ContainsColumn(string name);
	}
}
