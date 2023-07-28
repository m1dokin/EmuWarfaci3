using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL.CustomRules;
using MasterServer.Database;
using MasterServer.GameLogic.CustomRules.Rules;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002B4 RID: 692
	[Service]
	[Singleton]
	internal class CustomRulesStateStorage : ServiceModule, ICustomRulesStateStorage, IDebugCustomRulesStateStorage
	{
		// Token: 0x06000ED5 RID: 3797 RVA: 0x0003B692 File Offset: 0x00039A92
		public CustomRulesStateStorage(ICustomRulesService customRulesService, ICustomRuleStateSerializerFactory serializerFactory, IDALService dalService)
		{
			this.m_customRulesService = customRulesService;
			this.m_serializerFactory = serializerFactory;
			this.m_dalService = dalService;
		}

		// Token: 0x06000ED6 RID: 3798 RVA: 0x0003B6AF File Offset: 0x00039AAF
		public override void Init()
		{
			if (Resources.DBUpdaterPermission)
			{
				ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
			}
		}

		// Token: 0x06000ED7 RID: 3799 RVA: 0x0003B6CC File Offset: 0x00039ACC
		public override void Stop()
		{
			if (Resources.DBUpdaterPermission)
			{
				ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
			}
		}

		// Token: 0x06000ED8 RID: 3800 RVA: 0x0003B6EC File Offset: 0x00039AEC
		public CustomRuleState GetState(ulong profileID, ulong ruleID)
		{
			ICustomRule rule = this.m_customRulesService.GetRule(ruleID);
			return this.GetState(profileID, rule);
		}

		// Token: 0x06000ED9 RID: 3801 RVA: 0x0003B710 File Offset: 0x00039B10
		public CustomRuleState GetState(ulong profileID, ICustomRule rule)
		{
			CustomRuleRawState ruleState = this.m_dalService.CustomRulesSystem.GetRuleState(profileID, rule.RuleID);
			return this.DeserializeState(ruleState, rule);
		}

		// Token: 0x06000EDA RID: 3802 RVA: 0x0003B740 File Offset: 0x00039B40
		private KeyValuePair<bool, CustomRuleState> CompareAndSwapState(CustomRuleState state, ICustomRule rule)
		{
			CustomRuleRawState rawState = this.SerializeState<CustomRuleState>(state, rule);
			KeyValuePair<bool, CustomRuleRawState> keyValuePair = this.m_dalService.CustomRulesSystem.CompareAndSwapState(rawState);
			CustomRuleState value = this.DeserializeState(keyValuePair.Value, rule);
			return new KeyValuePair<bool, CustomRuleState>(keyValuePair.Key, value);
		}

		// Token: 0x06000EDB RID: 3803 RVA: 0x0003B784 File Offset: 0x00039B84
		public bool UpdateState(ulong profileID, ICustomRule rule, Func<CustomRuleState, bool> updateFunc)
		{
			int num = 0;
			KeyValuePair<bool, CustomRuleState> keyValuePair = new KeyValuePair<bool, CustomRuleState>(false, this.GetState(profileID, rule));
			while (updateFunc(keyValuePair.Value))
			{
				keyValuePair = this.CompareAndSwapState(keyValuePair.Value, rule);
				if (++num > 5)
				{
					throw new ApplicationException(string.Format("CAS attempt limit reached while updating '{0}' state", rule.GetType()));
				}
				if (keyValuePair.Key)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000EDC RID: 3804 RVA: 0x0003B7F4 File Offset: 0x00039BF4
		public void CleanupInactiveRulesState()
		{
			Log.Info("Cleaning up inactive custom rules state...");
			List<ulong> activeRuleIDs = (from r in this.m_customRulesService.GetActiveRules()
			select r.RuleID).Concat(from r in this.m_customRulesService.GetDisabledRules()
			select r.RuleID).ToList<ulong>();
			this.m_dalService.CustomRulesSystem.CleanupInactiveRulesState(activeRuleIDs);
			Log.Info("Cleanup completed");
		}

		// Token: 0x06000EDD RID: 3805 RVA: 0x0003B88C File Offset: 0x00039C8C
		public CustomRuleState SetUpdateTime(ulong profileID, ulong ruleID, DateTime updateTime)
		{
			ICustomRule rule = this.m_customRulesService.GetRule(ruleID);
			this.UpdateState(profileID, rule, delegate(CustomRuleState state)
			{
				state.LastActivationTime = updateTime.ToUniversalTime();
				return true;
			});
			this.m_dalService.CustomRulesSystem.SetUpdateTime(profileID, ruleID, updateTime);
			return this.GetState(profileID, rule);
		}

		// Token: 0x06000EDE RID: 3806 RVA: 0x0003B8E8 File Offset: 0x00039CE8
		private CustomRuleState DeserializeState(CustomRuleRawState rawState, ICustomRule rule)
		{
			Type type = rule.GetType();
			if (rawState.Key.Version != 0U)
			{
				CustomRuleStateInfo info = this.m_serializerFactory.GetInfo(type);
				if (info == null)
				{
					throw new ApplicationException(string.Format("Unregistered state type '{0}'", type));
				}
				if (info.TypeID != rawState.RuleType)
				{
					throw new ApplicationException(string.Format("Type error: requested type has id '{0}' ({1}) while stored state id is '{2}'", info.TypeID, info.StateType, rawState.RuleType));
				}
			}
			ICustomRuleStateSerializer serializer = this.m_serializerFactory.GetSerializer(type);
			return serializer.Deserialize(rawState);
		}

		// Token: 0x06000EDF RID: 3807 RVA: 0x0003B984 File Offset: 0x00039D84
		private CustomRuleRawState SerializeState<TState>(TState state, ICustomRule rule)
		{
			Type type = rule.GetType();
			ICustomRuleStateSerializer serializer = this.m_serializerFactory.GetSerializer(type);
			CustomRuleRawState customRuleRawState = serializer.Serialize(state);
			CustomRuleStateInfo info = this.m_serializerFactory.GetInfo(type);
			customRuleRawState.RuleType = info.TypeID;
			return customRuleRawState;
		}

		// Token: 0x06000EE0 RID: 3808 RVA: 0x0003B9CC File Offset: 0x00039DCC
		private void OnExecutionPhaseChanged(ExecutionPhase execution_phase)
		{
			if (execution_phase == ExecutionPhase.PostUpdate)
			{
				this.CleanupInactiveRulesState();
			}
		}

		// Token: 0x040006C9 RID: 1737
		private const int CAS_ATTEMPT_LIMIT = 5;

		// Token: 0x040006CA RID: 1738
		private readonly ICustomRulesService m_customRulesService;

		// Token: 0x040006CB RID: 1739
		private readonly ICustomRuleStateSerializerFactory m_serializerFactory;

		// Token: 0x040006CC RID: 1740
		private readonly IDALService m_dalService;
	}
}
