using System;
using System.Xml;
using MasterServer.DAL;

namespace MasterServer.GameLogic.SpecialProfileRewards.Actions
{
	// Token: 0x020005C3 RID: 1475
	public struct SMoneyRewardNotification
	{
		// Token: 0x06001F9A RID: 8090 RVA: 0x00080FC4 File Offset: 0x0007F3C4
		public XmlElement ToXml()
		{
			XmlElement xmlElement = new XmlDocument().CreateElement("give_money");
			xmlElement.SetAttribute("currency", GiveMoneyAction.CurrencyToStr(this.curr));
			XmlElement xmlElement2 = xmlElement;
			string name = "type";
			int num = (int)this.curr;
			xmlElement2.SetAttribute(name, num.ToString());
			xmlElement.SetAttribute("amount", this.amount.ToString());
			xmlElement.SetAttribute("notify", (!this.notify) ? "0" : "1");
			if (this.user_data != null)
			{
				XmlNode newChild = xmlElement.OwnerDocument.ImportNode(this.user_data, true);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x04000F6E RID: 3950
		public Currency curr;

		// Token: 0x04000F6F RID: 3951
		public ulong amount;

		// Token: 0x04000F70 RID: 3952
		public bool notify;

		// Token: 0x04000F71 RID: 3953
		public XmlElement user_data;
	}
}
