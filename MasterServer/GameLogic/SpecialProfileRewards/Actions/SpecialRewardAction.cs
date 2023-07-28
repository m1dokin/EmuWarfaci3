using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards.Actions
{
	// Token: 0x02000440 RID: 1088
	internal abstract class SpecialRewardAction : ISpecialRewardAction
	{
		// Token: 0x06001740 RID: 5952 RVA: 0x00060C82 File Offset: 0x0005F082
		protected SpecialRewardAction(ConfigSection config)
		{
			config.TryGet("use_notification", out this.m_useNotification, true);
		}

		// Token: 0x17000226 RID: 550
		// (get) Token: 0x06001741 RID: 5953
		public abstract string PrizeName { get; }

		// Token: 0x06001742 RID: 5954
		public abstract SNotification Activate(ulong profileId, ILogGroup logGroup, XmlElement userData);

		// Token: 0x06001743 RID: 5955 RVA: 0x00060C9D File Offset: 0x0005F09D
		public virtual void Validate()
		{
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00060CA0 File Offset: 0x0005F0A0
		protected SNotification CreateNotification<T>(ENotificationType type, T data, TimeSpan expiration, EConfirmationType confirmation)
		{
			SNotification result = null;
			if (this.m_useNotification)
			{
				result = NotificationFactory.CreateNotification<T>(type, data, expiration, confirmation);
			}
			return result;
		}

		// Token: 0x04000B39 RID: 2873
		protected readonly bool m_useNotification;
	}
}
