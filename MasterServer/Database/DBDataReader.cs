using System;
using System.Collections.Generic;
using System.Data;
using MasterServer.Core.Diagnostics.Profiler;
using MasterServer.DAL;
using MySql.Data.MySqlClient;

namespace MasterServer.Database
{
	// Token: 0x02000829 RID: 2089
	public class DBDataReader : IDataReaderEx, IDataReader, IDisposable, IDataRecord
	{
		// Token: 0x06002B09 RID: 11017 RVA: 0x000B9E51 File Offset: 0x000B8251
		public DBDataReader(MySqlAccessor acc, MySqlDataReader r, TimeExecution sw)
		{
			this.accessor = acc;
			this.reader = r;
			this.time = sw;
			if (this.time != null)
			{
				this.time.Stop();
			}
		}

		// Token: 0x170003ED RID: 1005
		// (get) Token: 0x06002B0A RID: 11018 RVA: 0x000B9E88 File Offset: 0x000B8288
		private Dictionary<string, int> attr_dict
		{
			get
			{
				if (this._attr_dict == null)
				{
					this._attr_dict = new Dictionary<string, int>();
					for (int num = 0; num != this.reader.FieldCount; num++)
					{
						this._attr_dict[this.reader.GetName(num)] = num;
					}
				}
				return this._attr_dict;
			}
		}

		// Token: 0x06002B0B RID: 11019 RVA: 0x000B9EE8 File Offset: 0x000B82E8
		public void Dispose()
		{
			if (this.reader != null)
			{
				this.reader.Dispose();
				this.reader = null;
				if (this.time != null)
				{
					using (new TimeDB(this.accessor, this.time))
					{
					}
				}
			}
		}

		// Token: 0x06002B0C RID: 11020 RVA: 0x000B9F54 File Offset: 0x000B8354
		public bool ContainsColumn(string name)
		{
			return this.attr_dict.ContainsKey(name);
		}

		// Token: 0x06002B0D RID: 11021 RVA: 0x000B9F64 File Offset: 0x000B8364
		public bool Read()
		{
			if (this.time != null)
			{
				this.time.Start();
			}
			bool result = this.reader.Read();
			if (this.time != null)
			{
				this.time.Stop();
			}
			return result;
		}

		// Token: 0x170003EE RID: 1006
		public object this[int i]
		{
			get
			{
				return this.reader[i];
			}
		}

		// Token: 0x170003EF RID: 1007
		public object this[string name]
		{
			get
			{
				return this.reader[name];
			}
		}

		// Token: 0x170003F0 RID: 1008
		// (get) Token: 0x06002B10 RID: 11024 RVA: 0x000B9FC7 File Offset: 0x000B83C7
		public int FieldCount
		{
			get
			{
				return this.reader.FieldCount;
			}
		}

		// Token: 0x06002B11 RID: 11025 RVA: 0x000B9FD4 File Offset: 0x000B83D4
		public string GetName(int i)
		{
			return this.reader.GetName(i);
		}

		// Token: 0x06002B12 RID: 11026 RVA: 0x000B9FE2 File Offset: 0x000B83E2
		public void Close()
		{
			this.reader.Close();
		}

		// Token: 0x170003F1 RID: 1009
		// (get) Token: 0x06002B13 RID: 11027 RVA: 0x000B9FEF File Offset: 0x000B83EF
		public int Depth
		{
			get
			{
				return this.reader.Depth;
			}
		}

		// Token: 0x06002B14 RID: 11028 RVA: 0x000B9FFC File Offset: 0x000B83FC
		public DataTable GetSchemaTable()
		{
			throw new NotImplementedException();
		}

		// Token: 0x170003F2 RID: 1010
		// (get) Token: 0x06002B15 RID: 11029 RVA: 0x000BA003 File Offset: 0x000B8403
		public bool IsClosed
		{
			get
			{
				return this.reader.IsClosed;
			}
		}

		// Token: 0x06002B16 RID: 11030 RVA: 0x000BA010 File Offset: 0x000B8410
		public bool NextResult()
		{
			return this.reader.NextResult();
		}

		// Token: 0x170003F3 RID: 1011
		// (get) Token: 0x06002B17 RID: 11031 RVA: 0x000BA01D File Offset: 0x000B841D
		public int RecordsAffected
		{
			get
			{
				return this.reader.RecordsAffected;
			}
		}

		// Token: 0x06002B18 RID: 11032 RVA: 0x000BA02A File Offset: 0x000B842A
		public bool GetBoolean(int i)
		{
			return this.reader.GetBoolean(i);
		}

		// Token: 0x06002B19 RID: 11033 RVA: 0x000BA038 File Offset: 0x000B8438
		public byte GetByte(int i)
		{
			return this.reader.GetByte(i);
		}

		// Token: 0x06002B1A RID: 11034 RVA: 0x000BA046 File Offset: 0x000B8446
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return this.reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		// Token: 0x06002B1B RID: 11035 RVA: 0x000BA05A File Offset: 0x000B845A
		public char GetChar(int i)
		{
			return this.reader.GetChar(i);
		}

		// Token: 0x06002B1C RID: 11036 RVA: 0x000BA068 File Offset: 0x000B8468
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return this.reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		// Token: 0x06002B1D RID: 11037 RVA: 0x000BA07C File Offset: 0x000B847C
		public IDataReader GetData(int i)
		{
			return this.reader.GetData(i);
		}

		// Token: 0x06002B1E RID: 11038 RVA: 0x000BA08A File Offset: 0x000B848A
		public string GetDataTypeName(int i)
		{
			return this.reader.GetDataTypeName(i);
		}

		// Token: 0x06002B1F RID: 11039 RVA: 0x000BA098 File Offset: 0x000B8498
		public DateTime GetDateTime(int i)
		{
			return this.reader.GetDateTime(i);
		}

		// Token: 0x06002B20 RID: 11040 RVA: 0x000BA0A6 File Offset: 0x000B84A6
		public decimal GetDecimal(int i)
		{
			return this.reader.GetDecimal(i);
		}

		// Token: 0x06002B21 RID: 11041 RVA: 0x000BA0B4 File Offset: 0x000B84B4
		public double GetDouble(int i)
		{
			return this.reader.GetDouble(i);
		}

		// Token: 0x06002B22 RID: 11042 RVA: 0x000BA0C2 File Offset: 0x000B84C2
		public Type GetFieldType(int i)
		{
			return this.reader.GetFieldType(i);
		}

		// Token: 0x06002B23 RID: 11043 RVA: 0x000BA0D0 File Offset: 0x000B84D0
		public float GetFloat(int i)
		{
			return this.reader.GetFloat(i);
		}

		// Token: 0x06002B24 RID: 11044 RVA: 0x000BA0DE File Offset: 0x000B84DE
		public Guid GetGuid(int i)
		{
			return this.reader.GetGuid(i);
		}

		// Token: 0x06002B25 RID: 11045 RVA: 0x000BA0EC File Offset: 0x000B84EC
		public short GetInt16(int i)
		{
			return this.reader.GetInt16(i);
		}

		// Token: 0x06002B26 RID: 11046 RVA: 0x000BA0FA File Offset: 0x000B84FA
		public int GetInt32(int i)
		{
			return this.reader.GetInt32(i);
		}

		// Token: 0x06002B27 RID: 11047 RVA: 0x000BA108 File Offset: 0x000B8508
		public long GetInt64(int i)
		{
			return this.reader.GetInt64(i);
		}

		// Token: 0x06002B28 RID: 11048 RVA: 0x000BA116 File Offset: 0x000B8516
		public int GetOrdinal(string name)
		{
			return this.reader.GetOrdinal(name);
		}

		// Token: 0x06002B29 RID: 11049 RVA: 0x000BA124 File Offset: 0x000B8524
		public string GetString(int i)
		{
			return this.reader.GetString(i);
		}

		// Token: 0x06002B2A RID: 11050 RVA: 0x000BA132 File Offset: 0x000B8532
		public object GetValue(int i)
		{
			return this.reader.GetValue(i);
		}

		// Token: 0x06002B2B RID: 11051 RVA: 0x000BA140 File Offset: 0x000B8540
		public int GetValues(object[] values)
		{
			return this.reader.GetValues(values);
		}

		// Token: 0x06002B2C RID: 11052 RVA: 0x000BA14E File Offset: 0x000B854E
		public bool IsDBNull(int i)
		{
			return this.reader.IsDBNull(i);
		}

		// Token: 0x040016FE RID: 5886
		private MySqlAccessor accessor;

		// Token: 0x040016FF RID: 5887
		private TimeExecution time;

		// Token: 0x04001700 RID: 5888
		private MySqlDataReader reader;

		// Token: 0x04001701 RID: 5889
		private Dictionary<string, int> _attr_dict;
	}
}
