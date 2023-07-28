using System;

namespace MasterServer.DAL
{
	// Token: 0x02000083 RID: 131
	public class PersistentSettingsSerializer : IDBSerializer<SPersistentSettings>
	{
		// Token: 0x0600018C RID: 396 RVA: 0x00004F18 File Offset: 0x00003318
		public void Deserialize(IDataReaderEx reader, out SPersistentSettings ret)
		{
			ret = default(SPersistentSettings);
			ret.ProfileID = ulong.Parse(reader["profile_id"].ToString());
			ret.Group = reader["setting_group"].ToString();
			ret.Settings = reader["settings"].ToString();
		}
	}
}
