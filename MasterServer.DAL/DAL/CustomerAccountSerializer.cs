using System;

namespace MasterServer.DAL
{
	// Token: 0x0200005E RID: 94
	public class CustomerAccountSerializer : IDBSerializer<CustomerAccount>
	{
		// Token: 0x060000BD RID: 189 RVA: 0x00003FE8 File Offset: 0x000023E8
		public void Deserialize(IDataReaderEx reader, out CustomerAccount ret)
		{
			ret = default(CustomerAccount);
			ret.CustomerId = ulong.Parse(reader["customer_id"].ToString());
			ret.Currency = (Currency)uint.Parse(reader["currency"].ToString());
			ret.Money = ulong.Parse(reader["money"].ToString());
		}
	}
}
