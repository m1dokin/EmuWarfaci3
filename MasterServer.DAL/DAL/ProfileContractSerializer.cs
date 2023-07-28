using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000025 RID: 37
	public class ProfileContractSerializer : IDBSerializer<ProfileContract>
	{
		// Token: 0x0600005E RID: 94 RVA: 0x000031C4 File Offset: 0x000015C4
		public void Deserialize(IDataReaderEx reader, out ProfileContract ret)
		{
			ret = new ProfileContract();
			ret.ProfileId = ulong.Parse(reader["profile_id"].ToString());
			ret.RotationId = uint.Parse(reader["rotation_id"].ToString());
			ret.ProfileItemId = ulong.Parse(reader["profile_item_id"].ToString());
			ret.ContractName = reader["contract_name"].ToString();
			ret.CurrentProgress = uint.Parse(reader["progress_current"].ToString());
			ret.TotalProgress = uint.Parse(reader["progress_total"].ToString());
			ulong utc = ulong.Parse(reader["rotation_time"].ToString());
			ret.RotationTimeUTC = TimeUtils.UTCTimestampToUTCTime(utc);
		}
	}
}
