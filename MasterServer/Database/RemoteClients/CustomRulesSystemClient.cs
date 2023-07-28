using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MasterServer.DAL.CustomRules;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001FE RID: 510
	internal class CustomRulesSystemClient : DALCacheProxy<IDALService>, ICustomRulesSystemClient
	{
		// Token: 0x06000A46 RID: 2630 RVA: 0x000266CB File Offset: 0x00024ACB
		internal void Reset(ICustomRulesSystem customRulesSystem)
		{
			this.m_customRulesSystem = customRulesSystem;
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x000266D4 File Offset: 0x00024AD4
		public IEnumerable<CustomRuleInfo> GetRules()
		{
			DALCacheProxy<IDALService>.Options<CustomRuleInfo> options = new DALCacheProxy<IDALService>.Options<CustomRuleInfo>
			{
				get_data_stream = (() => this.m_customRulesSystem.GetRules())
			};
			return base.GetDataStream<CustomRuleInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x00026708 File Offset: 0x00024B08
		public void UpdateRules(IEnumerable<CustomRuleInfo> rules)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_customRulesSystem.UpdateRules(rules))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x00026750 File Offset: 0x00024B50
		public IEnumerable<CustomRuleInfo> AddRule(CustomRuleInfo rule)
		{
			DALCacheProxy<IDALService>.Options<CustomRuleInfo> options = new DALCacheProxy<IDALService>.Options<CustomRuleInfo>
			{
				get_data_stream = (() => this.m_customRulesSystem.AddRule(rule))
			};
			return base.GetDataStream<CustomRuleInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x00026798 File Offset: 0x00024B98
		public IEnumerable<CustomRuleInfo> UpdateRule(CustomRuleInfo rule)
		{
			DALCacheProxy<IDALService>.Options<CustomRuleInfo> options = new DALCacheProxy<IDALService>.Options<CustomRuleInfo>
			{
				get_data_stream = (() => this.m_customRulesSystem.UpdateRule(rule))
			};
			return base.GetDataStream<CustomRuleInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A4B RID: 2635 RVA: 0x000267E0 File Offset: 0x00024BE0
		public IEnumerable<CustomRuleInfo> DeleteRule(ulong ruleId)
		{
			DALCacheProxy<IDALService>.Options<CustomRuleInfo> options = new DALCacheProxy<IDALService>.Options<CustomRuleInfo>
			{
				get_data_stream = (() => this.m_customRulesSystem.DeleteRule(ruleId))
			};
			return base.GetDataStream<CustomRuleInfo>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A4C RID: 2636 RVA: 0x00026828 File Offset: 0x00024C28
		public CustomRuleRawState GetRuleState(ulong profileId, ulong ruleId)
		{
			DALCacheProxy<IDALService>.Options<CustomRuleRawState> options = new DALCacheProxy<IDALService>.Options<CustomRuleRawState>
			{
				cache_domain = cache_domains.profile[profileId].custom_rules_state,
				get_data_stream = (() => this.m_customRulesSystem.GetRulesState(profileId))
			};
			CustomRuleRawState customRuleRawState = base.GetDataStream<CustomRuleRawState>(MethodBase.GetCurrentMethod(), options).FirstOrDefault((CustomRuleRawState x) => x.Key.RuleID == ruleId);
			CustomRuleRawState result;
			if ((result = customRuleRawState) == null)
			{
				result = new CustomRuleRawState
				{
					Key = new CustomRuleRawState.KeyData
					{
						ProfileID = profileId,
						RuleID = ruleId
					}
				};
			}
			return result;
		}

		// Token: 0x06000A4D RID: 2637 RVA: 0x000268E4 File Offset: 0x00024CE4
		public KeyValuePair<bool, CustomRuleRawState> CompareAndSwapState(CustomRuleRawState rawState)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<KeyValuePair<bool, CustomRuleRawState>> options = new DALCacheProxy<IDALService>.SetOptionsScalar<KeyValuePair<bool, CustomRuleRawState>>
			{
				query_retry = base.DAL.Config.QueryRetry,
				cache_domain = cache_domains.profile[rawState.Key.ProfileID].custom_rules_state,
				set_func = (() => this.m_customRulesSystem.CompareAndSwapState(rawState))
			};
			return base.SetAndClearScalar<KeyValuePair<bool, CustomRuleRawState>>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A4E RID: 2638 RVA: 0x00026970 File Offset: 0x00024D70
		public void CleanupInactiveRulesState(IEnumerable<ulong> activeRuleIDs)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_customRulesSystem.CleanupInactiveRulesState(activeRuleIDs))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A4F RID: 2639 RVA: 0x000269B8 File Offset: 0x00024DB8
		public void CleanupRuleState(ulong ruleId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_customRulesSystem.CleanupRuleState(ruleId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x00026A00 File Offset: 0x00024E00
		public void SetUpdateTime(ulong profileId, ulong ruleID, DateTime updateTime)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.profile[profileId].custom_rules_state,
				set_func = (() => this.m_customRulesSystem.SetUpdateTime(profileId, ruleID, updateTime))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000554 RID: 1364
		private ICustomRulesSystem m_customRulesSystem;
	}
}
