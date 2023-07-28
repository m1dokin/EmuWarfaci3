using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020005CA RID: 1482
	[Service]
	[Singleton]
	internal class SpecialProfileRewardService : ServiceModule, ISpecialProfileRewardServiceDebug, ISpecialProfileRewardService
	{
		// Token: 0x06001FAD RID: 8109 RVA: 0x00081368 File Offset: 0x0007F768
		public SpecialProfileRewardService(ISpecialRewardActionFactory actionFactory)
		{
			this.m_actionFactory = actionFactory;
		}

		// Token: 0x06001FAE RID: 8110 RVA: 0x00081377 File Offset: 0x0007F777
		public override void Init()
		{
			this.m_rewardSets = this.LoadRewardSets();
		}

		// Token: 0x06001FAF RID: 8111 RVA: 0x00081385 File Offset: 0x0007F785
		public override void Start()
		{
			base.Start();
			this.ValidateRewardSets();
		}

		// Token: 0x06001FB0 RID: 8112 RVA: 0x00081394 File Offset: 0x0007F794
		private Dictionary<string, RewardSet> LoadRewardSets()
		{
			Dictionary<string, RewardSet> dictionary = new Dictionary<string, RewardSet>();
			List<ConfigSection> sections = Resources.SpecialRewardSettings.GetSections("event");
			foreach (ConfigSection configSection in sections)
			{
				RewardSet value = this.LoadRewardSet(configSection);
				dictionary.Add(configSection.Get("name"), value);
			}
			return dictionary;
		}

		// Token: 0x06001FB1 RID: 8113 RVA: 0x00081418 File Offset: 0x0007F818
		private void ValidateRewardSets()
		{
			this.m_rewardSets.Values.ForEachAggregate(delegate(RewardSet set)
			{
				set.Validate();
			});
		}

		// Token: 0x06001FB2 RID: 8114 RVA: 0x00081448 File Offset: 0x0007F848
		private RewardSet LoadRewardSet(ConfigSection set_section)
		{
			List<ISpecialRewardAction> list = new List<ISpecialRewardAction>();
			bool enableNotifs = !set_section.HasValue("use_notification") || set_section.Get("use_notification") == "1";
			foreach (KeyValuePair<string, List<ConfigSection>> keyValuePair in set_section.GetAllSections())
			{
				foreach (ConfigSection config in keyValuePair.Value)
				{
					list.Add(this.m_actionFactory.CreateAction(keyValuePair.Key, config, enableNotifs));
				}
			}
			return new RewardSet(set_section.Get("name"), list);
		}

		// Token: 0x06001FB3 RID: 8115 RVA: 0x00081540 File Offset: 0x0007F940
		public List<SNotification> ProcessEvent(string setName, ulong profileId, ILogGroup logGroup)
		{
			return this.ProcessEvent(setName, profileId, logGroup, null);
		}

		// Token: 0x06001FB4 RID: 8116 RVA: 0x0008154C File Offset: 0x0007F94C
		public List<SNotification> ProcessEvent(string setName, ulong profileId, ILogGroup logGroup, XmlElement userData)
		{
			List<SNotification> result = new List<SNotification>();
			RewardSet rewardSet;
			if (!this.m_rewardSets.TryGetValue(setName, out rewardSet))
			{
				Log.Warning(string.Format("Reward set '{0}' does not exist. Requested for profile: {1}", setName, profileId));
				return result;
			}
			return rewardSet.Activate(profileId, logGroup, userData);
		}

		// Token: 0x06001FB5 RID: 8117 RVA: 0x00081598 File Offset: 0x0007F998
		public RewardSet GetRewardSet(string setName)
		{
			RewardSet result;
			this.m_rewardSets.TryGetValue(setName, out result);
			return result;
		}

		// Token: 0x06001FB6 RID: 8118 RVA: 0x000815B8 File Offset: 0x0007F9B8
		public void DumpRewardSets()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Reward sets:");
			foreach (RewardSet rewardSet in this.m_rewardSets.Values)
			{
				stringBuilder.AppendFormat(rewardSet.ToString(), new object[0]);
			}
			Log.Info<StringBuilder>("{0}", stringBuilder);
		}

		// Token: 0x04000F78 RID: 3960
		private Dictionary<string, RewardSet> m_rewardSets;

		// Token: 0x04000F79 RID: 3961
		private readonly ISpecialRewardActionFactory m_actionFactory;
	}
}
