using System;

namespace MasterServer.DAL
{
	// Token: 0x0200001B RID: 27
	public class ClanMemberSerializer : IDBSerializer<ClanMember>
	{
		// Token: 0x0600004A RID: 74 RVA: 0x00002C78 File Offset: 0x00001078
		public void Deserialize(IDataReaderEx reader, out ClanMember ret)
		{
			ret = new ClanMember();
			ret.ProfileID = ulong.Parse(reader["profile_id"].ToString());
			ret.ClanID = ulong.Parse(reader["clan_id"].ToString());
			ret.Expirience = ulong.Parse(reader["experience"].ToString());
			ret.Nickname = reader["nickname"].ToString();
			ret.ClanPoints = ulong.Parse(reader["clan_points"].ToString());
			ret.InviteDate = ulong.Parse(reader["invite_date"].ToString());
			ret.ClanRole = (EClanRole)Enum.Parse(ret.ClanRole.GetType(), reader["clan_role"].ToString());
		}
	}
}
