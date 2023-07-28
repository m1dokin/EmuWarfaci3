using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.VoucherSystem;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;

namespace MasterServer.GameLogic.CustomRules.Rules.CheckVoucher
{
	// Token: 0x020002B7 RID: 695
	[CustomRule("check_voucher")]
	internal class CheckVoucherRule : CustomRule
	{
		// Token: 0x06000EE6 RID: 3814 RVA: 0x0003BC00 File Offset: 0x0003A000
		public CheckVoucherRule(XmlElement config, ILogService logService, IUserRepository userRepository, INotificationService notificationService, IVoucherService voucherService, ITagService tagService, IProcessingQueueMetricsTracker processingQueueMetricsTracker) : base(config, userRepository, logService, notificationService, tagService, CheckVoucherRule.RULE_CFG_ATTRS)
		{
			this.m_voucherService = voucherService;
			this.m_processingQueueMetricsTracker = processingQueueMetricsTracker;
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000EE7 RID: 3815 RVA: 0x0003BC24 File Offset: 0x0003A024
		protected virtual string RuleName
		{
			get
			{
				return "check_voucher";
			}
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0003BC2B File Offset: 0x0003A02B
		public override bool IsActive()
		{
			return base.Enabled;
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0003BC33 File Offset: 0x0003A033
		public override void Activate()
		{
			this.m_processingQueue = new VoucherProcessingQueue(this.m_processingQueueMetricsTracker);
			this.UserRepository.UserLoggingIn += this.OnUserLoggingIn;
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x0003BC60 File Offset: 0x0003A060
		public override void Dispose()
		{
			this.UserRepository.UserLoggingIn -= this.OnUserLoggingIn;
			VoucherProcessingQueue processingQueue = this.m_processingQueue;
			this.m_processingQueue = null;
			if (processingQueue != null)
			{
				processingQueue.Stop();
			}
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x0003BC9E File Offset: 0x0003A09E
		public override string ToString()
		{
			return string.Format("{0} id={1}", this.RuleName, base.RuleID);
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x0003BCBB File Offset: 0x0003A0BB
		protected override ulong GetRuleID(XmlElement config)
		{
			return (ulong)((long)this.RuleName.GetHashCode());
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x0003BCCC File Offset: 0x0003A0CC
		private void OnUserLoggingIn(UserInfo.User user, ELoginType type, DateTime lastSeen)
		{
			Log.Verbose(Log.Group.CustomRules, "Checking user '{0}' for eligibility by rule '{1}'", new object[]
			{
				user,
				this
			});
			if (type != ELoginType.SwitchChannel)
			{
				Log.Verbose(Log.Group.CustomRules, "Rule '{0}' is triggered for user '{1}'", new object[]
				{
					base.RuleID,
					user.UserID
				});
				Task<IEnumerable<GiveItemResponse>> task = this.m_voucherService.ProccessVoucher(user);
				Task item = task.ContinueWith(delegate(Task<IEnumerable<GiveItemResponse>> t)
				{
					if (t.IsFaulted)
					{
						Log.Error(t.Exception);
						return;
					}
					IEnumerable<GiveItemResponse> result = t.Result;
					if (result.Any<GiveItemResponse>())
					{
						using (ILogGroup logGroup = this.LogService.CreateGroup())
						{
							this.ReportCustomRuleTriggered(user.UserID, user.ProfileID, logGroup);
						}
						List<SNotification> notifications = (from item in result
						select (!(item.ItemGiven.Item.Type == "random_box")) ? ItemGivenNotificationFactory.CreateItemGivenNotification(item, item.ItemGiven.ExpirationTime, item.Message, true, null) : item.PurchaseListener.CreateNotification(item.ItemGiven, item.Message, true)).ToList<SNotification>();
						this.SendNotifications(user.ProfileID, notifications);
					}
				});
				this.m_processingQueue.Add(item);
			}
		}

		// Token: 0x040006D1 RID: 1745
		public const string RULE_NAME = "check_voucher";

		// Token: 0x040006D2 RID: 1746
		private static readonly string[] RULE_CFG_ATTRS = new string[]
		{
			"enabled",
			"use_notification"
		};

		// Token: 0x040006D3 RID: 1747
		private readonly IVoucherService m_voucherService;

		// Token: 0x040006D4 RID: 1748
		private readonly IProcessingQueueMetricsTracker m_processingQueueMetricsTracker;

		// Token: 0x040006D5 RID: 1749
		private VoucherProcessingQueue m_processingQueue;
	}
}
