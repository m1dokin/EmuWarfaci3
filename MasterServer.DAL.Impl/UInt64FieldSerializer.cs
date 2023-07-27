using System;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000014 RID: 20
	public class UInt64FieldSerializer : IDBSerializer<ulong>
	{
		// Token: 0x060000B5 RID: 181 RVA: 0x000073AE File Offset: 0x000055AE
		public UInt64FieldSerializer(string f)
		{
			this.m_field = f;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x000073BD File Offset: 0x000055BD
		public void Deserialize(IDataReaderEx reader, out ulong ret)
		{
			ret = ulong.Parse(reader[this.m_field].ToString());
		}

		// Token: 0x0400004A RID: 74
		private string m_field;
	}
}
