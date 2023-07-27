using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.DAL.CustomRules;
using MasterServer.Database;
using Util.Common;

namespace MasterServer.DAL.Impl.CustomRules
{
	// Token: 0x0200000E RID: 14
	internal class CustomRulesSystem : ICustomRulesSystem
	{
		// Token: 0x06000058 RID: 88 RVA: 0x00004290 File Offset: 0x00002490
		public CustomRulesSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000042B8 File Offset: 0x000024B8
		public DALResultMulti<CustomRuleInfo> GetRules()
		{
			CacheProxy.Options<CustomRuleInfo> options = new CacheProxy.Options<CustomRuleInfo>
			{
				db_serializer = this.m_ruleInfoSerializer
			};
			options.query("CALL GetCustomRules()", new object[0]);
			return this.m_dal.CacheProxy.GetStream<CustomRuleInfo>(options);
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000042FC File Offset: 0x000024FC
		public DALResultVoid UpdateRules(IEnumerable<CustomRuleInfo> rules)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL ClearCustomRules()", new object[0]);
				foreach (CustomRuleInfo customRuleInfo in rules)
				{
					this.m_dal.ValidateFixedSizeColumnData("custom_rules", "data", customRuleInfo.Data.Length);
					mySqlAccessor.ExecuteNonQuery("CALL AddCustomRule(?rule_id, ?source, ?created_at, ?enabled, ?data)", new object[]
					{
						"?rule_id",
						customRuleInfo.RuleID,
						"?source",
						customRuleInfo.Source.ToString().ToLower(),
						"?created_at",
						TimeUtils.LocalTimeToUTCTimestamp(customRuleInfo.CreatedAtUTC),
						"?enabled",
						customRuleInfo.Enabled,
						"?data",
						customRuleInfo.Data
					});
				}
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x0000443C File Offset: 0x0000263C
		public DALResultMulti<CustomRuleInfo> AddRule(CustomRuleInfo rule)
		{
			CacheProxy.Options<CustomRuleInfo> options = new CacheProxy.Options<CustomRuleInfo>
			{
				db_serializer = this.m_ruleInfoSerializer
			};
			this.m_dal.ValidateFixedSizeColumnData("custom_rules", "data", rule.Data.Length);
			options.query("CALL AddCustomRule(?rule_id, ?source, ?created_at, ?enabled, ?data)", new object[]
			{
				"?rule_id",
				rule.RuleID,
				"?source",
				rule.Source.ToString().ToLower(),
				"?created_at",
				TimeUtils.LocalTimeToUTCTimestamp(rule.CreatedAtUTC),
				"?enabled",
				rule.Enabled,
				"?data",
				rule.Data
			});
			return this.m_dal.CacheProxy.GetStream<CustomRuleInfo>(options);
		}

		// Token: 0x0600005C RID: 92 RVA: 0x0000451C File Offset: 0x0000271C
		public DALResultMulti<CustomRuleInfo> UpdateRule(CustomRuleInfo rule)
		{
			CacheProxy.Options<CustomRuleInfo> options = new CacheProxy.Options<CustomRuleInfo>
			{
				db_serializer = this.m_ruleInfoSerializer
			};
			this.m_dal.ValidateFixedSizeColumnData("custom_rules", "data", rule.Data.Length);
			options.query("CALL UpdateCustomRule(?rule_id, ?created_at, ?enabled, ?data)", new object[]
			{
				"?rule_id",
				rule.RuleID,
				"?created_at",
				TimeUtils.LocalTimeToUTCTimestamp(rule.CreatedAtUTC),
				"?enabled",
				rule.Enabled,
				"?data",
				rule.Data
			});
			return this.m_dal.CacheProxy.GetStream<CustomRuleInfo>(options);
		}

		// Token: 0x0600005D RID: 93 RVA: 0x000045D8 File Offset: 0x000027D8
		public DALResultMulti<CustomRuleInfo> DeleteRule(ulong ruleID)
		{
			CacheProxy.Options<CustomRuleInfo> options = new CacheProxy.Options<CustomRuleInfo>
			{
				db_serializer = this.m_ruleInfoSerializer
			};
			options.query("CALL DeleteCustomRule(?rule_id)", new object[]
			{
				"?rule_id",
				ruleID
			});
			return this.m_dal.CacheProxy.GetStream<CustomRuleInfo>(options);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000462C File Offset: 0x0000282C
		public DALResultMulti<CustomRuleRawState> GetRulesState(ulong profileId)
		{
			CacheProxy.Options<CustomRuleRawState> options = new CacheProxy.Options<CustomRuleRawState>
			{
				db_serializer = this.m_ruleStateSerializer
			};
			options.query("CALL GetCustomRulesState(?pid)", new object[]
			{
				"?pid",
				profileId
			});
			return this.m_dal.CacheProxy.GetStream<CustomRuleRawState>(options);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00004680 File Offset: 0x00002880
		public DALResult<KeyValuePair<bool, CustomRuleRawState>> CompareAndSwapState(CustomRuleRawState rawState)
		{
			if (rawState.Data == null)
			{
				rawState.Data = new byte[0];
			}
			if (rawState.Key.Version == 0U)
			{
				rawState.Key = new CustomRuleRawState.KeyData(rawState.Key.ProfileID, rawState.Key.RuleID, 1U);
			}
			this.m_dal.ValidateFixedSizeColumnData("custom_rules_state", "data", rawState.Data.Length);
			CacheProxy.Options<CustomRuleRawState> options = new CacheProxy.Options<CustomRuleRawState>
			{
				db_serializer = this.m_ruleStateSerializer,
				db_transaction = true
			};
			options.query("CALL UpdateCustomRuleState(?pid, ?rid, ?vsn, ?rt, ?data)", new object[]
			{
				"?pid",
				rawState.Key.ProfileID,
				"?rid",
				rawState.Key.RuleID,
				"?vsn",
				rawState.Key.Version,
				"?rt",
				rawState.RuleType,
				"?data",
				rawState.Data
			});
			DALResult<CustomRuleRawState> dalresult = this.m_dal.CacheProxy.Get<CustomRuleRawState>(options);
			bool flag = dalresult.Value == null;
			if (flag)
			{
				dalresult.Value = rawState;
				dalresult.Value.Key = new CustomRuleRawState.KeyData(rawState.Key.ProfileID, rawState.Key.RuleID, dalresult.Value.Key.Version + 1U);
			}
			return new DALResult<KeyValuePair<bool, CustomRuleRawState>>(new KeyValuePair<bool, CustomRuleRawState>(flag, dalresult.Value), dalresult.Stats);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00004840 File Offset: 0x00002A40
		public DALResultVoid CleanupInactiveRulesState(IEnumerable<ulong> activeRuleIDs)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query(activeRuleIDs.Any<ulong>() ? string.Format("DELETE FROM custom_rules_state WHERE rule_id NOT IN ({0})", string.Join<ulong>(",", activeRuleIDs.ToArray<ulong>())) : "DELETE FROM custom_rules_state", new object[0]);
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000048A0 File Offset: 0x00002AA0
		public DALResultVoid CleanupRuleState(ulong ruleID)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query(string.Format("DELETE FROM custom_rules_state WHERE rule_id = {0}", ruleID), new object[0]);
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000048E0 File Offset: 0x00002AE0
		public DALResultVoid SetUpdateTime(ulong profileId, ulong ruleID, DateTime updateTime)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL SetUpdateTimeCustomRuleState(?pid, ?rid, ?time)", new object[]
			{
				"?pid",
				profileId,
				"?rid",
				ruleID,
				"?time",
				TimeUtils.LocalTimeToUTCTimestamp(updateTime)
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0400001E RID: 30
		private readonly DAL m_dal;

		// Token: 0x0400001F RID: 31
		private readonly CustomRuleInfoSerializer m_ruleInfoSerializer = new CustomRuleInfoSerializer();

		// Token: 0x04000020 RID: 32
		private readonly CustomRuleStateSerializer m_ruleStateSerializer = new CustomRuleStateSerializer();
	}
}
