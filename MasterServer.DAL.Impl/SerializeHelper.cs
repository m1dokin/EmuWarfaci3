using System;
using System.Collections.Generic;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000010 RID: 16
	internal static class SerializeHelper
	{
		// Token: 0x06000085 RID: 133 RVA: 0x00004EA4 File Offset: 0x000030A4
		public static IEnumerable<T> Deserialize<T>(IDataReaderEx reader, IDBSerializer<T> serializer)
		{
			List<T> list = new List<T>();
			while (reader.Read())
			{
				T item;
				serializer.Deserialize(reader, out item);
				list.Add(item);
			}
			return list;
		}
	}
}
