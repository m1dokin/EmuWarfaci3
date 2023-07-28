using System;
using System.Xml;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x02000291 RID: 657
	[Serializable]
	internal class ContractNotification
	{
		// Token: 0x06000E36 RID: 3638 RVA: 0x00038DCB File Offset: 0x000371CB
		public ContractNotification(string name, bool success) : this(name, success, 0U)
		{
		}

		// Token: 0x06000E37 RID: 3639 RVA: 0x00038DD6 File Offset: 0x000371D6
		public ContractNotification(string name, bool success, uint gameMoneyReward)
		{
			this.m_itemName = name;
			this.m_success = success;
			this.m_gameMoneyReward = gameMoneyReward;
		}

		// Token: 0x06000E38 RID: 3640 RVA: 0x00038DF3 File Offset: 0x000371F3
		public override string ToString()
		{
			return string.Format("Name: {0}, Success: {1}", this.m_itemName, this.m_success);
		}

		// Token: 0x06000E39 RID: 3641 RVA: 0x00038E10 File Offset: 0x00037210
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("contract");
			xmlElement.SetAttribute("name", this.m_itemName);
			xmlElement.SetAttribute("success", (!this.m_success) ? "0" : "1");
			xmlElement.SetAttribute("game_money_reward", this.m_gameMoneyReward.ToString());
			return xmlElement;
		}

		// Token: 0x04000687 RID: 1671
		private string m_itemName;

		// Token: 0x04000688 RID: 1672
		private bool m_success;

		// Token: 0x04000689 RID: 1673
		private uint m_gameMoneyReward;
	}
}
