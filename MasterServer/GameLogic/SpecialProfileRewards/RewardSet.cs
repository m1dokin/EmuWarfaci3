using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.NotificationSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards
{
	// Token: 0x020000F4 RID: 244
	internal class RewardSet
	{
		// Token: 0x060003FD RID: 1021 RVA: 0x000114E8 File Offset: 0x0000F8E8
		public RewardSet(string name, IEnumerable<ISpecialRewardAction> actions)
		{
			this.Name = name;
			this.m_actions = actions;
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060003FE RID: 1022 RVA: 0x000114FE File Offset: 0x0000F8FE
		// (set) Token: 0x060003FF RID: 1023 RVA: 0x00011506 File Offset: 0x0000F906
		public string Name { get; private set; }

		// Token: 0x06000400 RID: 1024 RVA: 0x0001150F File Offset: 0x0000F90F
		public IEnumerable<string> Prizes()
		{
			return from a in this.m_actions
			select a.PrizeName;
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x0001153C File Offset: 0x0000F93C
		public void Validate()
		{
			try
			{
				this.m_actions.ForEachAggregate(delegate(ISpecialRewardAction a)
				{
					a.Validate();
				});
			}
			catch (AggregateException ex)
			{
				throw new AggregateException(string.Format("Validation of reward set '{0}' failed", this.Name), ex.InnerExceptions);
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x000115A4 File Offset: 0x0000F9A4
		public List<SNotification> Activate(ulong profileId, ILogGroup logGroup, XmlElement userData)
		{
			List<SNotification> list = new List<SNotification>();
			foreach (ISpecialRewardAction specialRewardAction in this.m_actions)
			{
				try
				{
					SNotification snotification = specialRewardAction.Activate(profileId, logGroup, userData);
					if (snotification != null)
					{
						list.Add(snotification);
					}
				}
				catch (Exception e)
				{
					Log.Warning(string.Format("Failed to execute special reward action for profile {0}", profileId));
					Log.Warning(e);
				}
			}
			return list;
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00011648 File Offset: 0x0000FA48
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("  {0}:", this.Name));
			foreach (ISpecialRewardAction arg in this.m_actions)
			{
				stringBuilder.AppendLine(string.Format("\t{0}", arg));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000116D0 File Offset: 0x0000FAD0
		public int Count()
		{
			return this.m_actions.Count<ISpecialRewardAction>();
		}

		// Token: 0x040001AF RID: 431
		private readonly IEnumerable<ISpecialRewardAction> m_actions;
	}
}
