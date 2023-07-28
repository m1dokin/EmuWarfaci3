using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002B9 RID: 697
	internal abstract class CustomRule : ICustomRule, IDisposable
	{
		// Token: 0x06000EEF RID: 3823 RVA: 0x0000D25C File Offset: 0x0000B65C
		protected CustomRule(XmlElement config, IUserRepository userRepository, ILogService logService, INotificationService notificationService, ITagService tagService, IEnumerable<string> ruleCfgAttrs)
		{
			this.UserRepository = userRepository;
			this.LogService = logService;
			this.m_notificationService = notificationService;
			this.m_tagService = tagService;
			IEnumerator enumerator = config.Attributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlAttribute xmlAttribute = (XmlAttribute)obj;
					if (!ruleCfgAttrs.Contains(xmlAttribute.Name))
					{
						throw new ApplicationException(string.Format("Unknown attribute '{0}'", xmlAttribute.Name));
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.Enabled = (int.Parse(config.GetAttribute("enabled")) > 0);
			this.RuleID = this.GetRuleID(config);
			this.Config = config;
			if (config.HasAttribute("message"))
			{
				this.Message = config.GetAttribute("message");
			}
			this.m_useNotification = (!config.HasAttribute("use_notification") || int.Parse(config.GetAttribute("use_notification")) > 0);
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000EF0 RID: 3824 RVA: 0x0000D380 File Offset: 0x0000B780
		// (set) Token: 0x06000EF1 RID: 3825 RVA: 0x0000D388 File Offset: 0x0000B788
		public ulong RuleID { get; private set; }

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000EF2 RID: 3826 RVA: 0x0000D391 File Offset: 0x0000B791
		// (set) Token: 0x06000EF3 RID: 3827 RVA: 0x0000D399 File Offset: 0x0000B799
		public XmlElement Config { get; private set; }

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000EF4 RID: 3828 RVA: 0x0000D3A2 File Offset: 0x0000B7A2
		// (set) Token: 0x06000EF5 RID: 3829 RVA: 0x0000D3AA File Offset: 0x0000B7AA
		public bool Enabled { get; private set; }

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000EF6 RID: 3830 RVA: 0x0000D3B3 File Offset: 0x0000B7B3
		// (set) Token: 0x06000EF7 RID: 3831 RVA: 0x0000D3BB File Offset: 0x0000B7BB
		private protected string Message { protected get; private set; }

		// Token: 0x06000EF8 RID: 3832
		public abstract bool IsActive();

		// Token: 0x06000EF9 RID: 3833
		public abstract void Activate();

		// Token: 0x06000EFA RID: 3834
		public abstract void Dispose();

		// Token: 0x06000EFB RID: 3835 RVA: 0x0000D3C4 File Offset: 0x0000B7C4
		protected void SendNotifications(ulong profileId, IEnumerable<SNotification> notifications)
		{
			if (!this.m_useNotification)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.Message))
			{
				foreach (SNotification snotification in notifications)
				{
					snotification.Message = this.Message;
				}
			}
			if (notifications.Any<SNotification>())
			{
				this.m_notificationService.AddNotifications(profileId, notifications, EDeliveryType.SendNowOrLater);
			}
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x0000D454 File Offset: 0x0000B854
		protected void ReportCustomRuleTriggered(ulong userId, ulong profileId)
		{
			this.ReportCustomRuleTriggered(userId, profileId, this.LogService.Event);
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x0000D46C File Offset: 0x0000B86C
		protected void ReportCustomRuleTriggered(ulong userId, ulong profileId, ILogGroup group)
		{
			Log.Verbose(Log.Group.CustomRules, "Rule '{0}' is triggered for user '{1}'", new object[]
			{
				this.RuleID,
				userId
			});
			string tags = this.m_tagService.GetUserTags(userId).ToString();
			group.CustomRuleTriggeredLog(this.RuleID, userId, profileId, tags);
		}

		// Token: 0x06000EFE RID: 3838
		protected abstract ulong GetRuleID(XmlElement config);

		// Token: 0x040006DB RID: 1755
		protected readonly IUserRepository UserRepository;

		// Token: 0x040006DC RID: 1756
		protected readonly ILogService LogService;

		// Token: 0x040006DD RID: 1757
		private readonly INotificationService m_notificationService;

		// Token: 0x040006DE RID: 1758
		protected ITagService m_tagService;

		// Token: 0x040006DF RID: 1759
		private const EDeliveryType DefaultDeliveryType = EDeliveryType.SendNowOrLater;

		// Token: 0x040006E0 RID: 1760
		private readonly bool m_useNotification;
	}
}
