using System;
using MasterServer.Common;
using MasterServer.DAL.VoucherSystem;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000027 RID: 39
	public class DalVoucherSerializer : IDBSerializer<DalVoucher>
	{
		// Token: 0x0600019F RID: 415 RVA: 0x0000EF1C File Offset: 0x0000D11C
		public void Deserialize(IDataReaderEx reader, out DalVoucher ret)
		{
			ret = default(DalVoucher);
			ret.Id = ulong.Parse(reader["id"].ToString());
			ret.UserId = ulong.Parse(reader["user_id"].ToString());
			ret.Data = reader["data"].ToString();
			ret.Status = Utils.ParseEnum<VoucherStatus>(reader["status"].ToString());
		}
	}
}
