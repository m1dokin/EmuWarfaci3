using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL.CustomRules;
using MasterServer.Database;
using MasterServer.GameLogic.CustomRules.Rules;
using Util.Common;

namespace MasterServer.GameLogic.CustomRules
{
	// Token: 0x020002AC RID: 684
	[Service]
	[Singleton]
	internal class CustomRulesService : ServiceModule, ICustomRulesService, IDebugCustomRulesService, IDBUpdateListener
	{
		// Token: 0x06000E99 RID: 3737 RVA: 0x0003A715 File Offset: 0x00038B15
		public CustomRulesService(ICustomRulesFactory customRulesFactory, IDBUpdateService dbUpdater, IDALService dalService, IJobSchedulerService jobSchedulerService, IEnumerable<ICustomRulesValidator> validators)
		{
			this.m_customRulesFactory = customRulesFactory;
			this.m_dbUpdater = dbUpdater;
			this.m_dalService = dalService;
			this.m_validators = validators;
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x06000E9A RID: 3738 RVA: 0x0003A750 File Offset: 0x00038B50
		// (remove) Token: 0x06000E9B RID: 3739 RVA: 0x0003A788 File Offset: 0x00038B88
		public event RuleSetUpdatedDeleg RuleSetUpdated;

		// Token: 0x06000E9C RID: 3740 RVA: 0x0003A7BE File Offset: 0x00038BBE
		public override void Init()
		{
			base.Init();
			this.m_dbUpdater.RegisterListener(this);
		}

		// Token: 0x06000E9D RID: 3741 RVA: 0x0003A7D2 File Offset: 0x00038BD2
		public override void Start()
		{
			base.Start();
			this.m_jobSchedulerService.AddJob("custom_rules_reload");
			this.ReloadRules();
		}

		// Token: 0x06000E9E RID: 3742 RVA: 0x0003A7F0 File Offset: 0x00038BF0
		public override void Stop()
		{
			this.m_dbUpdater.UnregisterListener(this);
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.ClearRules();
			}
			base.Stop();
		}

		// Token: 0x06000E9F RID: 3743 RVA: 0x0003A848 File Offset: 0x00038C48
		public IEnumerable<ICustomRule> GetActiveRules()
		{
			object @lock = this.m_lock;
			IEnumerable<ICustomRule> result;
			lock (@lock)
			{
				result = new List<ICustomRule>(this.m_activeRules);
			}
			return result;
		}

		// Token: 0x06000EA0 RID: 3744 RVA: 0x0003A894 File Offset: 0x00038C94
		public IEnumerable<ICustomRule> GetDisabledRules()
		{
			object @lock = this.m_lock;
			IEnumerable<ICustomRule> result;
			lock (@lock)
			{
				result = new List<ICustomRule>(this.m_disabledRules);
			}
			return result;
		}

		// Token: 0x06000EA1 RID: 3745 RVA: 0x0003A8E0 File Offset: 0x00038CE0
		public void ReloadRules()
		{
			this.ReloadRules(this.m_dalService.CustomRulesSystem.GetRules());
		}

		// Token: 0x06000EA2 RID: 3746 RVA: 0x0003A8F8 File Offset: 0x00038CF8
		public ICustomRule GetRule(ulong ruleId)
		{
			object @lock = this.m_lock;
			ICustomRule result;
			lock (@lock)
			{
				result = this.m_activeRules.Concat(this.m_disabledRules).FirstOrDefault((ICustomRule x) => x.RuleID == ruleId);
			}
			return result;
		}

		// Token: 0x06000EA3 RID: 3747 RVA: 0x0003A968 File Offset: 0x00038D68
		public void EnableRule(ulong ruleId, bool enabled)
		{
			List<CustomRuleInfo> source = this.m_dalService.CustomRulesSystem.GetRules().ToList<CustomRuleInfo>();
			CustomRuleInfo ruleInfo = source.First((CustomRuleInfo r) => r.RuleID == ruleId);
			if (this.EnableRuleImpl(ruleInfo, enabled) == null)
			{
				return;
			}
			List<ICustomRule> source2 = this.GetRulesByType(new Type[]
			{
				typeof(ConsecutiveLoginBonusRule)
			}).ToList<ICustomRule>();
			if (enabled && source2.Any((ICustomRule r) => r.RuleID == ruleId))
			{
				using (IEnumerator<ICustomRule> enumerator = (from r in source2
				where r.RuleID != ruleId
				select r).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ICustomRule rule = enumerator.Current;
						CustomRuleInfo ruleInfo2 = source.First((CustomRuleInfo r) => r.RuleID == rule.RuleID);
						this.EnableRuleImpl(ruleInfo2, false);
					}
				}
			}
			this.ReloadRules();
		}

		// Token: 0x06000EA4 RID: 3748 RVA: 0x0003AA84 File Offset: 0x00038E84
		public void EnableAllCustomRules(bool enabled)
		{
			IEnumerable<CustomRuleInfo> enumerable = this.m_dalService.CustomRulesSystem.GetRules();
			if (enabled)
			{
				List<ICustomRule> list = this.GetRulesByType(new Type[]
				{
					typeof(ConsecutiveLoginBonusRule)
				}).ToList<ICustomRule>();
				if (list.Any<ICustomRule>())
				{
					if (!list.Any((ICustomRule x) => x.Enabled))
					{
						list.RemoveAt(0);
					}
				}
				IEnumerable<ulong> clbRuleIds = from x in list
				select x.RuleID;
				enumerable = from x in enumerable
				where !clbRuleIds.Contains(x.RuleID)
				select x;
			}
			foreach (CustomRuleInfo ruleInfo in enumerable)
			{
				this.EnableRuleImpl(ruleInfo, enabled);
			}
			this.ReloadRules();
		}

		// Token: 0x06000EA5 RID: 3749 RVA: 0x0003AB98 File Offset: 0x00038F98
		public ICustomRule AddRule(string config, bool enabled)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(config);
			XmlElement xmlElement = (XmlElement)xmlDocument.FirstChild;
			xmlElement.SetAttribute("enabled", (!enabled) ? "0" : "1");
			CustomRuleInfo ruleInfo = new CustomRuleInfo
			{
				Source = CustomRuleInfo.RuleSource.Dynamic,
				CreatedAtUTC = DateTime.UtcNow,
				Enabled = enabled,
				Data = xmlDocument.InnerXml
			};
			return this.AddRule(ruleInfo);
		}

		// Token: 0x06000EA6 RID: 3750 RVA: 0x0003AC12 File Offset: 0x00039012
		public void RemoveRule(ulong ruleId)
		{
			this.RemoveRule(ruleId, CustomRuleInfo.RuleSource.Dynamic);
		}

		// Token: 0x06000EA7 RID: 3751 RVA: 0x0003AC1C File Offset: 0x0003901C
		public void CleanRuleState(ulong ruleId)
		{
			this.m_dalService.CustomRulesSystem.CleanupRuleState(ruleId);
		}

		// Token: 0x06000EA8 RID: 3752 RVA: 0x0003AC30 File Offset: 0x00039030
		public ICustomRule AddStaticRule(string config, bool enabled)
		{
			CustomRuleInfo ruleInfo = new CustomRuleInfo
			{
				Source = CustomRuleInfo.RuleSource.Static,
				CreatedAtUTC = DateTime.UtcNow,
				Enabled = enabled,
				Data = config
			};
			return this.AddRule(ruleInfo);
		}

		// Token: 0x06000EA9 RID: 3753 RVA: 0x0003AC6C File Offset: 0x0003906C
		public void RemoveStaticRule(ulong ruleId)
		{
			this.RemoveRule(ruleId, CustomRuleInfo.RuleSource.Static);
		}

		// Token: 0x06000EAA RID: 3754 RVA: 0x0003AC78 File Offset: 0x00039078
		public bool OnDBUpdateStage(IDBUpdateService updater, DBUpdateStage stage)
		{
			if (stage != DBUpdateStage.Data)
			{
				return true;
			}
			XmlDocument factory = new XmlDocument();
			XmlNode rulesXml = Resources.CustomRulesConfig.ToXmlNode(factory);
			this.UpdateRuleSet(rulesXml);
			this.ReloadRules();
			return true;
		}

		// Token: 0x06000EAB RID: 3755 RVA: 0x0003ACAE File Offset: 0x000390AE
		public void UpdateCache()
		{
		}

		// Token: 0x06000EAC RID: 3756 RVA: 0x0003ACB0 File Offset: 0x000390B0
		private ICustomRule AddRule(CustomRuleInfo ruleInfo)
		{
			ICustomRule customRule = this.m_customRulesFactory.CreateRule(ruleInfo);
			ruleInfo.RuleID = customRule.RuleID;
			IEnumerable<CustomRuleInfo> rules = this.m_dalService.CustomRulesSystem.AddRule(ruleInfo);
			this.ReloadRules(rules);
			return customRule;
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0003ACF0 File Offset: 0x000390F0
		private void RemoveRule(ulong ruleId, CustomRuleInfo.RuleSource source)
		{
			CustomRuleInfo customRuleInfo = this.m_dalService.CustomRulesSystem.GetRules().FirstOrDefault((CustomRuleInfo r) => r.RuleID == ruleId);
			if (customRuleInfo == null)
			{
				throw new KeyNotFoundException(string.Format("Can't find custom rule with ruleId {0}", ruleId));
			}
			if (customRuleInfo.Source != source)
			{
				throw new InvalidOperationException(string.Format("You can only remove {0} custom rules", source));
			}
			IEnumerable<CustomRuleInfo> rules = this.m_dalService.CustomRulesSystem.DeleteRule(ruleId);
			this.ReloadRules(rules);
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0003AD8D File Offset: 0x0003918D
		private ICollection<ICustomRule> CreateRuleSet(IEnumerable<CustomRuleInfo> rules)
		{
			return (from x in rules
			select this.m_customRulesFactory.CreateRule(x)).ToList<ICustomRule>();
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0003ADA8 File Offset: 0x000391A8
		private void ClearRules()
		{
			if (this.m_activeRules == null)
			{
				return;
			}
			foreach (ICustomRule customRule in this.m_activeRules)
			{
				customRule.Dispose();
			}
			foreach (ICustomRule customRule2 in this.m_disabledRules)
			{
				customRule2.Dispose();
			}
			this.m_activeRules.Clear();
			this.m_disabledRules.Clear();
		}

		// Token: 0x06000EB0 RID: 3760 RVA: 0x0003AE70 File Offset: 0x00039270
		private void UpdateRuleSet(XmlNode rulesXml)
		{
			ICollection<ICustomRule> collection = this.LoadStaticRuleSet(rulesXml);
			IEnumerable<CustomRuleInfo> ruleInfo = this.GetRuleInfo(collection);
			this.m_dalService.CustomRulesSystem.UpdateRules(ruleInfo);
			foreach (ICustomRule customRule in collection)
			{
				customRule.Dispose();
			}
		}

		// Token: 0x06000EB1 RID: 3761 RVA: 0x0003AEE4 File Offset: 0x000392E4
		private ICollection<ICustomRule> LoadStaticRuleSet(XmlNode config)
		{
			List<ICustomRule> list = (from XmlNode el in config.ChildNodes
			where el.NodeType == XmlNodeType.Element
			select this.m_customRulesFactory.CreateRule(el.Name, (XmlElement)el)).ToList<ICustomRule>();
			using (List<ICustomRule>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ICustomRule rule = enumerator.Current;
					ICustomRule customRule = list.FirstOrDefault((ICustomRule r) => !object.ReferenceEquals(rule, r) && r.RuleID == rule.RuleID);
					if (customRule != null)
					{
						throw new ApplicationException(string.Format("Rule {0} duplicates id of rule {1}", rule, customRule));
					}
				}
			}
			return list;
		}

		// Token: 0x06000EB2 RID: 3762 RVA: 0x0003AFB4 File Offset: 0x000393B4
		private IEnumerable<ICustomRule> GetRulesByType(Type[] type)
		{
			object @lock = this.m_lock;
			IEnumerable<ICustomRule> seq;
			lock (@lock)
			{
				seq = this.m_activeRules.Concat(this.m_disabledRules).ToList<ICustomRule>();
			}
			return seq.OfType(type);
		}

		// Token: 0x06000EB3 RID: 3763 RVA: 0x0003B010 File Offset: 0x00039410
		private IEnumerable<CustomRuleInfo> EnableRuleImpl(CustomRuleInfo ruleInfo, bool enabled)
		{
			if (ruleInfo.Enabled == enabled)
			{
				return null;
			}
			ruleInfo.Enabled = enabled;
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(ruleInfo.Data);
			XmlElement xmlElement = (XmlElement)xmlDocument.FirstChild;
			xmlElement.SetAttribute("enabled", (!enabled) ? "0" : "1");
			ruleInfo.Data = xmlDocument.InnerXml;
			return this.m_dalService.CustomRulesSystem.UpdateRule(ruleInfo);
		}

		// Token: 0x06000EB4 RID: 3764 RVA: 0x0003B090 File Offset: 0x00039490
		private void ReloadRules(IEnumerable<CustomRuleInfo> rules)
		{
			ulong num = this.CalculateHash(rules);
			object @lock = this.m_lock;
			List<ICustomRule> list;
			List<ICustomRule> list2;
			lock (@lock)
			{
				if (num == this.m_rulesHash)
				{
					return;
				}
				this.ClearRules();
				this.m_rulesHash = num;
				try
				{
					ICollection<ICustomRule> source = this.CreateRuleSet(rules);
					list = (from x in source
					where x.IsActive()
					select x).ToList<ICustomRule>();
					list2 = (from x in source
					where !x.IsActive()
					select x).ToList<ICustomRule>();
					foreach (ICustomRulesValidator customRulesValidator in this.m_validators)
					{
						customRulesValidator.Validate(list);
					}
					foreach (ICustomRule customRule in list.ToList<ICustomRule>())
					{
						try
						{
							customRule.Activate();
						}
						catch (Exception e)
						{
							list.Remove(customRule);
							Log.Error<ulong>("Failed to activate rule {0}", customRule.RuleID);
							Log.Error(e);
						}
					}
					this.m_activeRules = list;
					this.m_disabledRules = list2;
					Log.Info("Custom rules reload, state was updated");
				}
				catch
				{
					this.m_rulesHash = 0UL;
					throw;
				}
			}
			if (this.RuleSetUpdated != null)
			{
				this.RuleSetUpdated(list, list2);
			}
		}

		// Token: 0x06000EB5 RID: 3765 RVA: 0x0003B2B0 File Offset: 0x000396B0
		private IEnumerable<CustomRuleInfo> GetRuleInfo(IEnumerable<ICustomRule> rules)
		{
			return from r in rules
			select new CustomRuleInfo
			{
				RuleID = r.RuleID,
				Source = CustomRuleInfo.RuleSource.Static,
				CreatedAtUTC = TimeUtils.UTCZero,
				Enabled = r.Enabled,
				Data = r.Config.OuterXml
			};
		}

		// Token: 0x06000EB6 RID: 3766 RVA: 0x0003B2D8 File Offset: 0x000396D8
		private ulong CalculateHash(IEnumerable<CustomRuleInfo> rules)
		{
			List<CustomRuleInfo> list = (from x in rules
			orderby x.RuleID
			select x).ToList<CustomRuleInfo>();
			ulong num = 0UL;
			for (int i = 0; i < list.Count; i++)
			{
				CustomRuleInfo customRuleInfo = list[i];
				num ^= (customRuleInfo.RuleID ^ (ulong)((ulong)((long)customRuleInfo.Enabled.ToString().GetHashCode()) << i));
			}
			return num;
		}

		// Token: 0x040006B2 RID: 1714
		private readonly object m_lock = new object();

		// Token: 0x040006B3 RID: 1715
		private readonly ICustomRulesFactory m_customRulesFactory;

		// Token: 0x040006B4 RID: 1716
		private readonly IDBUpdateService m_dbUpdater;

		// Token: 0x040006B5 RID: 1717
		private readonly IDALService m_dalService;

		// Token: 0x040006B6 RID: 1718
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040006B7 RID: 1719
		private readonly IEnumerable<ICustomRulesValidator> m_validators;

		// Token: 0x040006B8 RID: 1720
		private List<ICustomRule> m_activeRules;

		// Token: 0x040006B9 RID: 1721
		private List<ICustomRule> m_disabledRules;

		// Token: 0x040006BA RID: 1722
		private ulong m_rulesHash;
	}
}
