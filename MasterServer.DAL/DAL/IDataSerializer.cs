using System;
using System.IO;
using MasterServer.DAL.Utils;

namespace MasterServer.DAL
{
	// Token: 0x02000069 RID: 105
	public interface IDataSerializer<T>
	{
		// Token: 0x060000FD RID: 253
		void Serialize(T data, TextWriter wr);

		// Token: 0x060000FE RID: 254
		T Deserialize(TextReader rd, DBVersion version);
	}
}
