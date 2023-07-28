using System;

namespace MasterServer.DAL
{
	// Token: 0x02000018 RID: 24
	public class ClanInfoSerializer : IDBSerializer<ClanInfo>
	{
		// Token: 0x06000046 RID: 70 RVA: 0x00002AA0 File Offset: 0x00000EA0
		public void Deserialize(IDataReaderEx reader, out ClanInfo ret)
		{
			ret = new ClanInfo();
			ret.ClanID = ulong.Parse(reader["clan_id"].ToString());
			ret.Name = reader["name"].ToString();
			ret.Description = reader["descr"].ToString();
			ret.CreationDate = ulong.Parse(reader["creation_date"].ToString());
			ret.ClanPoints = ulong.Parse(reader["clan_points_count"].ToString());
			ret.MasterNickname = reader["nickname"].ToString();
			ret.MembersCount = int.Parse(reader["clan_members_count"].ToString());
			ret.MasterBanner.Badge = uint.Parse(reader["banner_badge"].ToString());
			ret.MasterBanner.Mark = uint.Parse(reader["banner_mark"].ToString());
			ret.MasterBanner.Stripe = uint.Parse(reader["banner_stripe"].ToString());
			ret.MasterId = ulong.Parse(reader["master_pid"].ToString());
		}
	}
}
