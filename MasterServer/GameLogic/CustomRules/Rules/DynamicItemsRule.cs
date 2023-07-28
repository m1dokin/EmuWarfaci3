using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C9 RID: 713
	[CustomRule("dynamic_items")]
	internal class DynamicItemsRule : CustomRule, IProfileItemsComposer
	{
		// Token: 0x06000F2E RID: 3886 RVA: 0x0003CF50 File Offset: 0x0003B350
		public DynamicItemsRule(XmlElement config, IProfileItems profileItems, IItemCache itemCache, IUserRepository userRepository, ILogService logService, INotificationService notificationService, ITagService tagService) : base(config, userRepository, logService, notificationService, tagService, DynamicItemsRule.RULE_CFG_ATTRS)
		{
			this.m_profileItems = profileItems;
			this.m_itemCache = itemCache;
			this.m_tagFilter = new TagsFilter(config.GetAttribute("tag_filter"));
			this.m_dynamicItems = this.GetItems(config);
		}

		// Token: 0x06000F2F RID: 3887 RVA: 0x0003CFA2 File Offset: 0x0003B3A2
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000F30 RID: 3888 RVA: 0x0003CFAA File Offset: 0x0003B3AA
		public override void Activate()
		{
			this.m_resolvedItemsCache = this.ResolveItems(this.m_dynamicItems);
			this.m_profileItems.RegisterProfileItemsComposer(this);
		}

		// Token: 0x06000F31 RID: 3889 RVA: 0x0003CFCA File Offset: 0x0003B3CA
		public override void Dispose()
		{
			this.m_profileItems.UnregisterProfileItemsComposer(this);
		}

		// Token: 0x06000F32 RID: 3890 RVA: 0x0003CFD8 File Offset: 0x0003B3D8
		public void Compose(ulong profileId, EquipOptions options, List<SEquipItem> composedEquip)
		{
			if (!this.CheckConditions(profileId))
			{
				return;
			}
			UserInfo.User user = this.UserRepository.GetUser(profileId);
			base.ReportCustomRuleTriggered(user.UserID, user.ProfileID);
			composedEquip.AddRange(this.m_resolvedItemsCache);
		}

		// Token: 0x06000F33 RID: 3891 RVA: 0x0003D020 File Offset: 0x0003B420
		private bool CheckConditions(ulong profileId)
		{
			UserInfo.User user = this.UserRepository.GetUser(profileId);
			return user != null && this.m_tagFilter.Check(this.m_tagService.GetUserTags(user.UserID));
		}

		// Token: 0x06000F34 RID: 3892 RVA: 0x0003D05F File Offset: 0x0003B45F
		private List<string> GetItems(XmlElement config)
		{
			return (from XmlElement i in config.ChildNodes
			select i.GetAttribute("name")).ToList<string>();
		}

		// Token: 0x06000F35 RID: 3893 RVA: 0x0003D094 File Offset: 0x0003B494
		private List<SEquipItem> ResolveItems(IEnumerable<string> items)
		{
			List<SEquipItem> list = new List<SEquipItem>();
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			foreach (string text in items)
			{
				SItem sitem;
				if (!allItemsByName.TryGetValue(text, out sitem))
				{
					throw new ApplicationException(string.Format("Dynamic item '{0}' not found", text));
				}
				list.Add(new SEquipItem
				{
					ItemID = sitem.ID,
					ProfileItemID = ulong.MaxValue - sitem.ID,
					Config = string.Empty,
					Status = EProfileItemStatus.BOUGHT
				});
			}
			return list;
		}

		// Token: 0x06000F36 RID: 3894 RVA: 0x0003D158 File Offset: 0x0003B558
		protected override ulong GetRuleID(XmlElement config)
		{
			ulong num = (ulong)((long)"dynamic_items".GetHashCode());
			num ^= (ulong)((long)config.GetAttribute("tag_filter").GetHashCode());
			int i = 0;
			return config.ChildNodes.Cast<XmlElement>().Aggregate(num, (ulong current, XmlElement itemCfg) => current ^ (ulong)((ulong)((long)itemCfg.GetAttribute("name").GetHashCode()) << 32 * (++i % 2)));
		}

		// Token: 0x06000F37 RID: 3895 RVA: 0x0003D1B0 File Offset: 0x0003B5B0
		public override string ToString()
		{
			return string.Format("{0} id={1} tag_filter='{2}' message='{3}' items='{4}'", new object[]
			{
				"dynamic_items",
				base.RuleID,
				this.m_tagFilter,
				base.Message,
				string.Join(",", this.m_dynamicItems.ToArray())
			});
		}

		// Token: 0x04000702 RID: 1794
		private const string RULE_NAME = "dynamic_items";

		// Token: 0x04000703 RID: 1795
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"tag_filter",
			"message",
			"use_notification"
		};

		// Token: 0x04000704 RID: 1796
		private readonly TagsFilter m_tagFilter;

		// Token: 0x04000705 RID: 1797
		private readonly IProfileItems m_profileItems;

		// Token: 0x04000706 RID: 1798
		private readonly IItemCache m_itemCache;

		// Token: 0x04000707 RID: 1799
		private readonly List<string> m_dynamicItems;

		// Token: 0x04000708 RID: 1800
		private List<SEquipItem> m_resolvedItemsCache;
	}
}
