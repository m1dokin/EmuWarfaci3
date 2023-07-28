using System;
using Util.Common;

namespace MasterServer.DAL.CustomRules
{
	// Token: 0x0200002B RID: 43
	public class CustomRuleStateSerializer : IDBSerializer<CustomRuleRawState>
	{
		// Token: 0x0600006D RID: 109 RVA: 0x000033D0 File Offset: 0x000017D0
		public void Deserialize(IDataReaderEx reader, out CustomRuleRawState ret)
		{
			ulong profileId = ulong.Parse(reader["profile_id"].ToString());
			ulong ruleId = ulong.Parse(reader["rule_id"].ToString());
			uint version = uint.Parse(reader["version"].ToString());
			ret = new CustomRuleRawState();
			ret.Key = new CustomRuleRawState.KeyData(profileId, ruleId, version);
			ulong utc = ulong.Parse(reader["last_update"].ToString());
			ret.LastUpdateTimeUtc = TimeUtils.UTCTimestampToUTCTime(utc);
			ret.RuleType = byte.Parse(reader["rule_type"].ToString());
			ret.Data = (byte[])reader["data"];
		}
	}
}
