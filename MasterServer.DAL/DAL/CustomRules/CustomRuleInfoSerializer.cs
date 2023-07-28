using System;
using Util.Common;

namespace MasterServer.DAL.CustomRules
{
	// Token: 0x02000028 RID: 40
	public class CustomRuleInfoSerializer : IDBSerializer<CustomRuleInfo>
	{
		// Token: 0x06000061 RID: 97 RVA: 0x000032B0 File Offset: 0x000016B0
		public void Deserialize(IDataReaderEx reader, out CustomRuleInfo ret)
		{
			ret = new CustomRuleInfo();
			ret.RuleID = ulong.Parse(reader["rule_id"].ToString());
			ret.Source = (CustomRuleInfo.RuleSource)Enum.Parse(typeof(CustomRuleInfo.RuleSource), reader["source"].ToString(), true);
			ulong utc = ulong.Parse(reader["created_at"].ToString());
			ret.CreatedAtUTC = TimeUtils.UTCTimestampToUTCTime(utc);
			ret.Enabled = ParseUtils.ParseBool(reader["enabled"].ToString());
			ret.Data = reader["data"].ToString();
		}
	}
}
