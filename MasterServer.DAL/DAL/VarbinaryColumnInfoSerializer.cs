using System;

namespace MasterServer.DAL
{
	// Token: 0x02000021 RID: 33
	public class VarbinaryColumnInfoSerializer : IDBSerializer<SFixedSizeColumnInfo>
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00002F7C File Offset: 0x0000137C
		public void Deserialize(IDataReaderEx reader, out SFixedSizeColumnInfo ret)
		{
			ret = default(SFixedSizeColumnInfo);
			SFixedSizeColumnInfo sfixedSizeColumnInfo = ret;
			sfixedSizeColumnInfo.TableName = reader["table_name"].ToString();
			sfixedSizeColumnInfo.Name = reader["column_name"].ToString();
			sfixedSizeColumnInfo.MaxLength = int.Parse(reader["character_maximum_length"].ToString());
			ret = sfixedSizeColumnInfo;
		}
	}
}
